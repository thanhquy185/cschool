using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using cschool.ViewModels;
using cschool.Utils;
using Avalonia.Data; 
using System.Reactive.Threading.Tasks;


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

    private async void OnSaveButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SubjectClassViewModel vm)
        {
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

    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}