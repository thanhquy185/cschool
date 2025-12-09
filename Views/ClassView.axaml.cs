using Models;
using Avalonia.Controls;
using System.Threading.Tasks;
using Utils;
using ViewModels;
using Views.Class;
using System.Reactive.Threading.Tasks;
using Services;

namespace Views
{
    public partial class ClassView : UserControl
    {
        private ClassViewModel _classViewModel { get; set; }
        public ClassView()
        {
            InitializeComponent();
            this._classViewModel = new ClassViewModel();
            DataContext = this._classViewModel;

            InfoButton.Click += async (_, _) => await ShowClassDialog(DialogModeEnum.Info);
            CreateButton.Click += async (_, _) => await ShowClassDialog(DialogModeEnum.Create);
            UpdateButton.Click += async (_, _) => await ShowClassDialog(DialogModeEnum.Update);
            LockButton.Click += async (_, _) => await DeleteClass();

            // Phân quyền các nút chức năng
            if (SessionService.currentUserLogin != null && AppService.RoleDetailService != null)
            {
                InfoButton.IsEnabled = AppService.RoleDetailService.HasPermission(
                    SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Xem");
                CreateButton.IsEnabled = AppService.RoleDetailService.HasPermission(
                   SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Thêm");
                UpdateButton.IsEnabled = AppService.RoleDetailService.HasPermission(
                   SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Cập nhật");
                LockButton.IsEnabled = AppService.RoleDetailService.HasPermission(
                   SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Xoá / Khoá");
            }
        }

        // ============================
        //  SHOW DIALOGS (INFO/CREATE/UPDATE)
        // ============================
        private async Task ShowClassDialog(DialogModeEnum mode)
        {
            var vm = DataContext as ClassViewModel;
            var selectedClass = ClassDataGrid.SelectedItem as ClassModel;

            if (vm == null)
                return;

            // Không chọn lớp → không Info/Update
            if (selectedClass == null && mode != DialogModeEnum.Create)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn lớp để thực hiện thao tác!");
                return;
            }

            Window? dialog = null;

            switch (mode)
            {
                case DialogModeEnum.Info:
                    vm.SelectedClass = selectedClass;
                    dialog = new ClassInfoDialog(vm);

                    dialog.Opened += async (_, _) =>
                    {
                        await vm.GetClassByIdCommand.Execute(selectedClass.Id).ToTask();
                        await vm.GetTeacherByIdCommand.Execute(selectedClass.Id).ToTask();
                    };
                    break;

                case DialogModeEnum.Create:
                    dialog = new ClassCreateDialog(new ClassViewModel());
                    break;

                case DialogModeEnum.Update:
                    vm.SelectedClass = selectedClass;
                    dialog = new ClassUpdateDialog(vm);

                    dialog.Opened += async (_, _) =>
                    {
                        await vm.GetClassByIdCommand.Execute(selectedClass.Id).ToTask();
                        await vm.GetTeacherByIdCommand.Execute(selectedClass.Id).ToTask();
                        vm.LoadClassData();
                    };
                    break;
            }

            if (dialog != null)
            {
                var owner = TopLevel.GetTopLevel(this) as Window;
                if (owner != null)
                    await dialog.ShowDialog(owner);
                else
                    dialog.Show();
            }
        }

        // ============================
        //  DELETE (XÓA MỀM) — CHUẨN 100%
        // ============================
        private async Task DeleteClass()
        {
            var vm = DataContext as ClassViewModel;
            var selected = ClassDataGrid.SelectedItem as ClassModel;

            if (vm == null || selected == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn lớp để xoá!");
                return;
            }

            // Confirm
            var confirm = await MessageBoxUtil.ShowConfirm(
                $"Bạn có chắc muốn xóa lớp '{selected.Name}' không?"
            );

            if (!confirm)
                return;

            // Gọi service trong VM
            var result = vm.DeleteClassById(selected.Id);

            // Hiện message từ ViewModel
            await MessageBoxUtil.ShowInfo(result.message);

            // Thành công → reload list
            if (result.success)
            {
                vm.LoadData();
            }
        }
    }
}
