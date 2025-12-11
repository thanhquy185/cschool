using System.Reactive.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels;

namespace Views.Class
{
    public partial class ClassInfoDialog : Window
    {
        public ClassInfoDialog(ClassViewModel vm)
        {
            InitializeComponent();

            // Set DataContext để binding hoạt động
            DataContext = vm;

            // Nếu đã có SelectedClass thì load dữ liệu
            if (vm.SelectedClass != null)
            {
                // Load học sinh HK1/HK2
                vm.GetClassByIdCommand.Execute(vm.SelectedClass.Id).ToTask();

                // Load giáo viên HK1/HK2
                vm.GetTeacherByIdCommand.Execute(vm.SelectedClass.Id).ToTask();
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (DataContext is ClassViewModel vm)
            {
                vm.FilterStudentsInClass();
            }
        }
    }
}
