using Avalonia.Controls;
using System;
using cschool.ViewModels;

namespace cschool.Views.Student
{
    public partial class StudentInfoDialog : Window
    {
        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }

        public StudentInfoDialog(StudentViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
