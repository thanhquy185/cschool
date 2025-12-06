using Avalonia.Controls;
using cschool.ViewModels;
namespace cschool.Views;
using cschool.Models;
using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;
using cschool.Utils;
using cschool.ViewModels;
using cschool.Views.Tuition;
using System.Reactive.Threading.Tasks;

public partial class TuitionView : UserControl
{
    public TuitionView()
    {
            InitializeComponent();
            DataContext = new TuitionViewModel();
            InfoButton.Click += async (_, _) => await ShowTuitionDialog(DialogModeEnum.Info);
            CreateButton.Click += async (_, _) => await ShowTuitionDialog(DialogModeEnum.Create);
            UpdateButton.Click += async (_, _) => await ShowTuitionDialog(DialogModeEnum.Update);
            // LockButton.Click += async (_, _) => await ShowClassDialog(DialogModeEnum.Lock);
    }
    private async Task ShowTuitionDialog(DialogModeEnum mode)
    {
        var vm = DataContext as TuitionViewModel;
        var selectedTuition = Tuition_Class.SelectedItem as ClassModel;

        if (selectedTuition == null && mode != DialogModeEnum.Create)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn lớp để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                if (vm != null && selectedTuition != null)
                {
             
                }
                // dialog = new TuitionInfoDialog(vm);
                break;

            case DialogModeEnum.Create:
                dialog = new TuitionCreateDialog (vm);
                break;

            case DialogModeEnum.Update:
                    dialog = new TuitionUpdateDialog (vm);

                // if (vm != null && selectedTuition != null)
                // {
                //     // Lấy thông tin chi tiết lịch thi
                //     vm.GetTuitionByIdCommand.Execute(selectedTuition.Id).ToTask();

                //     // Lấy danh sách phân công
                //     vm.GetRoomUpdateByIdCommand.Execute(selectedTuition.Id).ToTask();

                //     // Lấy danh sách phòng thi
                //     vm.GetRoomListUpdateCommand.Execute(selectedTuition.Id).ToTask();

                //     // Lấy danh sách giáo viên
                //     vm.GetTeacherListUpdateCommand.Execute(selectedTuition.Id).ToTask();
                // }
                // dialog = new TuitionUpdateDialog(vm);
                break;

            case DialogModeEnum.Lock:
                // if (vm != null && selectedTuition != null)
                // {
                //     vm.GetTuitionByIdCommand.Execute(selectedTuition.Id).ToTask();
                // }
                // dialog = new TuitionLockDialog(vm);
                break;
        }


        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

}