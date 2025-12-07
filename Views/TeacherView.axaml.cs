using Avalonia.Controls;
using ViewModels;
using System.Threading.Tasks;
using Utils;
using Views.Teacher;
using System.Reactive.Threading.Tasks;
using Avalonia.Interactivity;


namespace Views;

public partial class TeacherView : UserControl
{
    public TeacherView()
    {
        InitializeComponent();
        DataContext = new TeacherViewModel();

        InfoButton.Click += async (_, _) => await ShowTeacherDialog(DialogModeEnum.Info);
        CreateButton.Click += async (_, _) => await ShowTeacherDialog(DialogModeEnum.Create);
        UpdateButton.Click += async (_, _) => await ShowTeacherDialog(DialogModeEnum.Update);
        LockButton.Click += async (_, _) => await ShowTeacherDialog(DialogModeEnum.Lock);
        ImportExcelButton.Click += (_, _) =>
        {
            var vm = DataContext as TeacherViewModel;
            if (vm != null)
            {
                vm.ImportFromExcelCommand.Execute().ToTask();
            }
        };
        ExportExcelButton.Click += (_, _) =>
        {
            var vm = DataContext as TeacherViewModel;
            if (vm != null)
            {
                vm.ExportToExcelCommand.Execute().ToTask();
            }
        };
    }

    private async Task ShowTeacherDialog(DialogModeEnum mode)
    {
        var vm = DataContext as TeacherViewModel;
        if (vm == null)
        {
            await MessageBoxUtil.ShowError("Không tìm thấy ViewModel.");
            return;
        }

        if (vm.SelectedTeacher == null && mode != DialogModeEnum.Create)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn người dùng để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                vm.TeacherDetails = await vm.GetTeacherByIdCommand.Execute().ToTask();
                dialog = new TeacherInfoDialog(vm);
                break;

            case DialogModeEnum.Create:
                vm.ClearTeacherDetails();
                dialog = new TeacherCreateDialog(vm);
                break;
            
            case DialogModeEnum.Update:
                vm.TeacherDetails = await vm.GetTeacherByIdCommand.Execute().ToTask() ?? vm.SelectedTeacher;
                vm.SetTeacherForEdit(vm.TeacherDetails);
                dialog = new TeacherUpdateDialog(vm);
                break;

            case DialogModeEnum.Lock:
                dialog = new TeacherLockDialog(vm);
                break;
        }
        if (dialog == null)
        {
            await MessageBoxUtil.ShowError("Không thể mở dialog. Vui lòng thử lại!");
            return;
        }


        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

    private void OnResetFilters(object? sender, RoutedEventArgs e)
    {
        // Reset SearchBox
        if (SearchBox != null)
            SearchBox.Text = "";

        // Reset ComboBox về item đầu tiên
        if (DepartmentFilterBox != null)
            DepartmentFilterBox.SelectedIndex = 0;

        // Áp dụng filter (sẽ hiển thị tất cả)
        // ApplyFilters();
    }
}