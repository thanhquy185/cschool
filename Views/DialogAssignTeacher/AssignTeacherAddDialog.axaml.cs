using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels;

namespace Views.DialogAssignTeacher
{
    public partial class AssignTeacherAddDialog : Window
    {

        public AssignTeacherAddDialog()
        {
            InitializeComponent();
        }

        private void OnSaveButtonClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is AssignTeacherViewModel vm)
            {
                vm.SaveAddCommand.Execute(null);
                //  Close(true);
            }
        }

        private void OnCancelButtonClick(object? sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }

}