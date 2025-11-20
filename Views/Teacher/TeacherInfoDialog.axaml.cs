using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using cschool.ViewModels;

namespace cschool.Views.Teacher
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
