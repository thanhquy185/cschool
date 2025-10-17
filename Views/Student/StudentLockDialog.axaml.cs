using Avalonia.Controls;

namespace cschool.Views.Student
{
    public partial class StudentLockDialog : Window
    {
        public StudentLockDialog(StudentModel student)
        {
            InitializeComponent();

            // MessageText.Text = $"Bạn có chắc muốn khoá tài khoản của {student.Fullname}?";
            // ConfirmButton.Click += (_, _) => Close();
        }
    }
}
