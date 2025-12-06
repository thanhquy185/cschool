using System.Threading.Tasks;
using Avalonia.Controls;
using Utils;
using Views.Exam;
using ViewModels;
using System.Reactive.Threading.Tasks;
using Avalonia.Interactivity;

namespace Views;

public partial class ExamView : UserControl
{
    public ExamView()
    {
        InitializeComponent();
        DataContext = new ExamViewModel();

        InfoButton.Click += async (_, _) => await ShowExamDialog(DialogModeEnum.Info);
        CreateButton.Click += async (_, _) => await ShowExamDialog(DialogModeEnum.Create);
        UpdateButton.Click += async (_, _) => await ShowExamDialog(DialogModeEnum.Update);
        LockButton.Click += async (_, _) => await ShowExamDialog(DialogModeEnum.Lock);
    }

    private async Task ShowExamDialog(DialogModeEnum mode)
    {
        var vm = DataContext as ExamViewModel;
        var selectedExam = ExamDataGrid.SelectedItem as ExamModel;

        if (selectedExam == null && mode != DialogModeEnum.Create)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn người dùng để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                if (vm != null && selectedExam != null)
                {
                    // Lấy thông tin chi tiết lịch thi
                    vm.GetExamByIdCommand.Execute(selectedExam.Id).ToTask();

                    // Lấy danh sách phòng thi
                    vm.GetRoomExamByIdCommand.Execute(selectedExam.Id).ToTask();
                }
                dialog = new ExamInfoDialog(vm);
                break;

            case DialogModeEnum.Create:
                dialog = new ExamCreateDialog (vm);
                break;

            case DialogModeEnum.Update:
                if (vm != null && selectedExam != null)
                {
                    // Lấy thông tin chi tiết lịch thi
                    vm.GetExamByIdCommand.Execute(selectedExam.Id).ToTask();

                    // Lấy danh sách phân công
                    vm.GetRoomUpdateByIdCommand.Execute(selectedExam.Id).ToTask();

                    // Lấy danh sách phòng thi
                    vm.GetRoomListUpdateCommand.Execute(selectedExam.Id).ToTask();

                    // Lấy danh sách giáo viên
                    vm.GetTeacherListUpdateCommand.Execute(selectedExam.Id).ToTask();
                }
                dialog = new ExamUpdateDialog(vm);
                break;

            case DialogModeEnum.Lock:
                if (vm != null && selectedExam != null)
                {
                    vm.GetExamByIdCommand.Execute(selectedExam.Id).ToTask();
                }
                dialog = new ExamLockDialog(vm);
                break;
        }


        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DataContext is ExamViewModel vm)
        {
            var textBox = sender as TextBox;
            vm.FilterKeyword = textBox?.Text ?? "";
            vm.ApplyFilter();
        }
    }

    private void OnStatusFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ExamViewModel vm)
        {
            var combo = sender as ComboBox;
            var selected = combo?.SelectedItem as TermModel;

            if (selected != null)
                vm.FilterStatus = selected.TermName.ToString();
            else
                vm.FilterStatus = "";

            vm.ApplyFilter();
        }
    }

    private void OnResetFilterClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ExamViewModel vm)
        {
            // reset ViewModel
            vm.FilterKeyword = "";
            vm.FilterStatus = "";
            vm.ApplyFilter();

            // reset UI
            SearchBox.Text = "";

            StatusFilterBox.SelectedIndex = -1; 
            StatusFilterBox.PlaceholderText = "Chọn học kỳ";
        }
    }

}
