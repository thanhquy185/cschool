using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using cschool.ViewModels;
using cschool.Utils;
using Avalonia.Data; 
using System.Reactive.Threading.Tasks;
using Avalonia.Input;


namespace cschool.Views.SubjectClass;

public partial class SubjectClassCreateDialog : Window
{
    public SubjectClassCreateDialog(SubjectClassViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
#if DEBUG
        this.AttachDevTools();
#endif
        UpdateScoreColumns();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        var dg = this.FindControl<DataGrid>("ScoreCreateDataGrid");
        if (dg != null)
        {
            dg.PreparingCellForEdit += OnPreparingCellForEdit;
            dg.CellEditEnding += OnCellEditEnding;
        }
    }

    private void UpdateScoreColumns()
    {
        if (DataContext is not SubjectClassViewModel vm || vm.SelectedSubjectClass == null) return;

        var dg = this.FindControl<DataGrid>("ScoreCreateDataGrid");
        if (dg == null) return;

        dg.Columns.Clear();

        dg.Columns.Add(new DataGridTextColumn
        {
            Header = "Họ và tên",
            Binding = new Binding("FullName"),
            IsReadOnly = true,
            Width = new DataGridLength(1.5, DataGridLengthUnitType.Star),
        });

        for (int i = 1; i <= vm.SelectedSubjectClass.OralCount; i++)
        {
            int index = i - 1;
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = $"Điểm miệng {i}",
                Binding = new Binding($"OralScores[{index}]", BindingMode.TwoWay),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            });
        }

        for (int i = 1; i <= vm.SelectedSubjectClass.QuizCount; i++)
        {
            int index = i - 1;
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = $"Điểm 15 phút {i}",
                Binding = new Binding($"Quizzes[{index}]", BindingMode.TwoWay),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            });
        }

        dg.Columns.Add(new DataGridTextColumn
        {
            Header = "Điểm giữa kỳ",
            Binding = new Binding("MidtermScore", BindingMode.TwoWay),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
        });

        dg.Columns.Add(new DataGridTextColumn
        {
            Header = "Điểm cuối kỳ",
            Binding = new Binding("FinalScore", BindingMode.TwoWay),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
        });
    }

    
    private void OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (e.EditingElement is TextBox textBox && e.Column.Header.ToString().Contains("Điểm"))
        {
            textBox.AddHandler(TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
        }
    }

    private void OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditingElement is TextBox textBox)
        {
            textBox.RemoveHandler(TextInputEvent, OnTextInput);
        }
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            var newText = textBox.Text ?? "";
            var caret = textBox.CaretIndex;
            newText = newText.Insert(caret, e.Text ?? "");

            if (!string.IsNullOrEmpty(newText) && !double.TryParse(newText, out _))
            {
                e.Handled = true;
            }
        }
    }

    private async void OnSaveButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SubjectClassViewModel vm)
        {
            if (!await ValidateScores())
            {
                return;
            }
            var success = await vm.SaveStudentScoresCommand.Execute().ToTask();
            if (success)
            {
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Lưu điểm thất bại!");
                this.Close();
            }
        }
    }

     private async System.Threading.Tasks.Task<bool> ValidateScores()
    {
        if (DataContext is not SubjectClassViewModel vm || vm.StudentScores == null)
            return true;

        foreach (var student in vm.StudentScores)
        {
            // Kiểm tra OralScores
            if (student.OralScores != null)
            {
                for (int i = 0; i < student.OralScores.Count; i++)
                {
                    if (!IsValidScore(student.OralScores[i]))
                    {
                        await MessageBoxUtil.ShowError($"Điểm miệng {i + 1} của {student.FullName} không hợp lệ!\nĐiểm phải là số từ 0 đến 10.");
                        return false;
                    }
                }
            }

            // Kiểm tra Quizzes
            if (student.Quizzes != null)
            {
                for (int i = 0; i < student.Quizzes.Count; i++)
                {
                    if (!IsValidScore(student.Quizzes[i]))
                    {
                        await MessageBoxUtil.ShowError($"Điểm 15 phút {i + 1} của {student.FullName} không hợp lệ!\nĐiểm phải là số từ 0 đến 10.");
                        return false;
                    }
                }
            }

            // Kiểm tra MidtermScore
            if (!IsValidScore(student.MidtermScore))
            {
                await MessageBoxUtil.ShowError($"Điểm giữa kỳ của {student.FullName} không hợp lệ!\nĐiểm phải là số từ 0 đến 10.");
                return false;
            }

            // Kiểm tra FinalScore
            if (!IsValidScore(student.FinalScore))
            {
                await MessageBoxUtil.ShowError($"Điểm cuối kỳ của {student.FullName} không hợp lệ!\nĐiểm phải là số từ 0 đến 10.");
                return false;
            }
        }

        return true;
    }

    private bool IsValidScore(double? score)
    {
        // Null là hợp lệ (chưa nhập điểm)
        if (score == null)
            return true;

        // Kiểm tra phạm vi 0-10
        return score >= 0 && score <= 10;
    }


    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}