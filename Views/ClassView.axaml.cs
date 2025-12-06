using cschool.Models;
using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;
using cschool.Utils;
using cschool.ViewModels;
using cschool.Views.Class;
using System.Reactive.Threading.Tasks;


namespace cschool.Views
{
    public partial class ClassView : UserControl
    {
        public ClassView()
        {
            InitializeComponent();
            DataContext = new ClassViewModel();

            InfoButton.Click += async (_, _) => await ShowClassDialog(DialogModeEnum.Info);
            CreateButton.Click += async (_, _) => await ShowClassDialog(DialogModeEnum.Create);
            UpdateButton.Click += async (_, _) => await ShowClassDialog(DialogModeEnum.Update);
            LockButton.Click += async (_, _) => await ShowClassDialog(DialogModeEnum.Lock);
        }

        private async Task ShowClassDialog(DialogModeEnum mode)
        {
            var vm = DataContext as ClassViewModel;
            var selectedClass = ClassDataGrid.SelectedItem as ClassModel;

            if (selectedClass == null && mode != DialogModeEnum.Create)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn lớp để thực hiện thao tác!");
                return;
            }

            Window? dialog = null;

            switch (mode)
            {
                case DialogModeEnum.Info:
                    if (vm != null && selectedClass != null)
                    {
                        vm.SelectedClass = selectedClass;

                      
                        dialog = new ClassInfoDialog(vm);

                        
                        dialog.Opened += async (_, _) =>
                        {
                            // Load học sinh HK1/HK2
                            await vm.GetClassByIdCommand.Execute(selectedClass.Id).ToTask();
                            // Load giáo viên HK1/HK2
                            await vm.GetTeacherByIdCommand.Execute(selectedClass.Id).ToTask();
                        };
                    }
                    break;

                case DialogModeEnum.Create:
                    dialog = new ClassCreateDialog(vm);
                    break;

                case DialogModeEnum.Update:
                    if (vm != null && selectedClass != null)
                    {
                         vm.SelectedClass = selectedClass;
                          dialog = new ClassUpdateDialog(vm);
                        dialog.Opened += async (_, _) =>
                        {
                            // Load học sinh HK1/HK2
                            await vm.GetClassByIdCommand.Execute(selectedClass.Id).ToTask();
                            // Load giáo viên HK1/HK2
                            await vm.GetTeacherByIdCommand.Execute(selectedClass.Id).ToTask();
                            vm.SelectedClass = selectedClass;
                            vm.LoadClassData();
                        };

                        // Có thể load dữ liệu nếu cần
                    }
                   
                    break;

                case DialogModeEnum.Lock:
                    if (vm != null && selectedClass != null)
                    {
                        await vm.GetClassByIdCommand.Execute(selectedClass.Id).ToTask();
                    }
                    // dialog = new ClassLockDialog(vm);
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
    }
}
