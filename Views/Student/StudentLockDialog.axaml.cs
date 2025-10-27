using System;
using System.Reactive.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using cschool.Utils;
using cschool.ViewModels;

namespace cschool.Views.Student
{
    public partial class StudentLockDialog : Window
    {
        public StudentViewModel studentViewModel { get; set; }
        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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
                await MessageBoxUtil.ShowSuccess("Khóa thông tin học sinh thành công!", owner: this);

                await studentViewModel.GetStudentsCommand.Execute().ToTask();

                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Khóa thông tin học sinh thất bại!", owner: this);
                this.Close();
            }
        }
    }
}
