using System;
using Avalonia.Controls;
using cschool.ViewModels;
using System.Threading.Tasks;
using Avalonia.Media;
using cschool.Utils;
using cschool.Views.Student;
namespace cschool.Views;

public partial class StudentView : UserControl
{
    public StudentView()
    {
        InitializeComponent();
        DataContext = new StudentViewModel();

        InfoButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Info);
        CreateButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Create);
        UpdateButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Update);
        LockButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Lock);
    }

    private async Task ShowStudentDialog(DialogModeEnum mode)
    {
        var vm = DataContext as StudentViewModel;
        var selectedStudent = StudentsDataGrid.SelectedItem as StudentModel;

        if (selectedStudent == null && mode != DialogModeEnum.Create)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn người dùng để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                if (vm != null && selectedStudent != null)
                {
                    var fullStudent = AppService.StudentService.GetStudentById(selectedStudent.Id);
                    vm.StudentDetails = fullStudent ?? selectedStudent;
                }
                dialog = new StudentInfoDialog(vm);
                break;

            case DialogModeEnum.Create:
                dialog = new StudentCreateDialog();
                break;

            case DialogModeEnum.Update:
                dialog = new StudentUpdateDialog(selectedStudent);
                break;

            case DialogModeEnum.Lock:
                dialog = new StudentLockDialog(selectedStudent);
                break;
        }


        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

}

public enum DialogModeEnum
{
    Info,
    Create,
    Update,
    Lock,
    ChangePassword
}