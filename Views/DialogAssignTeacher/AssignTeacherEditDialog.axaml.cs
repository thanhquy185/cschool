using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using cschool.ViewModels;

namespace cschool.Views.DialogAssignTeacher
{
    public partial class AssignTeacherEditDialog : Window
    {
        public AssignTeacherEditDialog()
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