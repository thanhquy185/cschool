using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using cschool.ViewModels;

namespace cschool.Views.Exam
{
    public partial class ExamInfoDialog : Window
    {
        public ExamViewModel examViewModel;
        public ExamInfoDialog(ExamViewModel vm)
        {
            InitializeComponent();
            examViewModel = vm;
            DataContext = vm;

            examViewModel.StudentDetails.Clear();
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void RoomSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (examViewModel == null) return;
            if (sender is not DataGrid dg) return;
            if (dg.SelectedItem is not RoomExamModel selectedRoom) return;

            int detailId = examViewModel.ExamDetails.Id;
            int roomId = selectedRoom.Id;
            
            var students = await Task.Run(() => 
                AppService.ExamService.GetStudentExamById(detailId, roomId)
            );

            // Cập nhật danh sách học sinh
            examViewModel.StudentDetails.Clear();
            foreach (var s in students)
                examViewModel.StudentDetails.Add(s);
        }
    }
}