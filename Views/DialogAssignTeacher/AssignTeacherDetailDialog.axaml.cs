using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels;

namespace Views.DialogAssignTeacher
{
    public partial class AssignTeacherDetailDialog : Window
    {
        public AssignTeacherDetailDialog()
        {
            InitializeComponent();
        }
        private void OnSaveButtonClick(object? sender, RoutedEventArgs e)
        {
      
            if (DataContext is AssignTeacherViewModel vm)
            {
                
                vm.SaveEditCommand.Execute(null);
                // Close(true);
            }
        }
        private void OnCancelButtonClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}