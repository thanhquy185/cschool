using Avalonia.Controls;
using ViewModels;

namespace Views.Student
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
