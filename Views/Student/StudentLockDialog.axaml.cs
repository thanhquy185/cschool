using System;
using System.Linq;
using System.Reactive.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Utils;
using ViewModels;

namespace Views.Student
{
    public partial class StudentLockDialog : Window
    {
        public StudentViewModel studentViewModel { get; set; }
        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public StudentLockDialog(StudentViewModel vm)
        {
            InitializeComponent();
            studentViewModel = vm;
            DataContext = vm;
        }

        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu từ các TextBox, ComboBox, DatePicker
            var id = Convert.ToInt32((DataContext as StudentViewModel)?.StudentDetails?.Id);

            // Gửi dữ liệu tới backend hoặc lưu vào model
            var student = new StudentModel
            {
                Id = id
            };

            // - Xử lý
            bool isSuccess = await studentViewModel.LockStudentCommand.Execute(student).ToTask();

            // Thông báo xử lý, nếu thành công thì ẩn dialog
            if (isSuccess)
            {
                // await studentViewModel.GetStudentsCommand.Execute().ToTask();
                var existing = studentViewModel.StudentsTemp.FirstOrDefault(s => s.Id == id);
                if (existing != null)
                {
                    studentViewModel.StudentsTemp.Remove(existing);
                }
                await MessageBoxUtil.ShowSuccess("Xóa thông tin học sinh thành công!", owner: this);
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Xóa thông tin học sinh thất bại!", owner: this);
                this.Close();
            }
        }
    }
}
