using Avalonia.Controls;
using System;
using cschool.ViewModels;

namespace cschool.Views.Student
{
    public partial class StudentUpdateDialog : Window
    {
        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }
        public StudentUpdateDialog(StudentModel student)
        {
            InitializeComponent();
        }
    }
}
