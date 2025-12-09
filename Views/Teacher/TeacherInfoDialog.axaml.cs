using Avalonia.Controls;
using ViewModels;

namespace Views.Teacher
{
    public partial class TeacherInfoDialog : Window
    {
        public TeacherInfoDialog(TeacherViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
