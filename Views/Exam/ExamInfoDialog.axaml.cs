using Avalonia.Controls;
using cschool.ViewModels;

namespace cschool.Views.Exam
{
    public partial class ExamInfoDialog : Window
    {
        public ExamInfoDialog(ExamViewModel vm)
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