using System;
using System.Reactive.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using cschool.Utils;
using cschool.ViewModels;

namespace cschool.Views.Exam
{
    public partial class ExamLockDialog : Window
    {
        public ExamViewModel examViewModel { get; set; }
        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public ExamLockDialog(ExamViewModel vm)
        {
            InitializeComponent();
            examViewModel = vm;
            DataContext = vm;
        }

        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu từ các TextBox, ComboBox, DatePicker
            var id = Convert.ToInt32((DataContext as ExamViewModel)?.ExamDetails?.Id);

            // Gửi dữ liệu tới backend hoặc lưu vào model
            var Exam = new ExamModel
            {
                Id = id
            };

            // - Xử lý
            bool isSuccess = await examViewModel.LockExamCommand.Execute(Exam).ToTask();

            // Thông báo xử lý, nếu thành công thì ẩn dialog
            if (isSuccess)
            {
                await examViewModel.GetExamsCommand.Execute().ToTask();
                await MessageBoxUtil.ShowSuccess("Xóa thông tin lịch thi thành công!\n", owner: this);
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Xóa thông tin lịch thi thất bại!\n", owner: this);
                this.Close();
            }
        }
    }
}
