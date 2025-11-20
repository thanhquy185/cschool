using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using cschool.ViewModels;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace cschool.Views.DialogAssignTeacher
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

