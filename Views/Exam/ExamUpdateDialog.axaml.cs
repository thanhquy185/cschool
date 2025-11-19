using Avalonia.Controls;
using Avalonia.Interactivity;
using cschool.ViewModels;
using cschool.Utils;
using System.Linq;

namespace cschool.Views.Exam
{
    public partial class ExamUpdateDialog : Window
    {
        public ExamViewModel examViewModel { get; set; }
        public ExamUpdateDialog(ExamViewModel vm)
        {
            InitializeComponent();
            examViewModel = vm;
            DataContext = vm;
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnGradeChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (Grade == null) return; // tránh crash

            if (Grade.SelectedItem is ComboBoxItem cbi && int.TryParse(cbi.Tag?.ToString(), out int grade))
            {
                examViewModel.SelectedGrade = grade;
                RemainingStudents.Text = examViewModel.RemainingStudentsText;
            }
            else
            {
                examViewModel.SelectedGrade = null;
                RemainingStudents.Text = "";
            }
        }

        // Thêm phân công mới
        private async void AddAssignment_Click(object? sender, RoutedEventArgs e)
        {
            // Lấy đối tượng được chọn
            var room = Room.SelectedItem as RoomModel;
            var teacher = Teacher.SelectedItem as TeacherModel;

            if (room == null || teacher == null)
            {
                await MessageBoxUtil.ShowWarning("Vui lòng chọn phòng thi và giáo viên.", "Thiếu thông tin", this);
                return;
            }

            // Kiểm tra trùng (nếu cùng phòng hoặc cùng giáo viên đã được chọn)
            bool alreadyExists = examViewModel.ExamAssignments
                .Any(a => a.RoomName == room.RoomName || a.TeacherName == teacher.TeacherName);

            if (alreadyExists)
            {
                await MessageBoxUtil.ShowWarning("Phòng hoặc giáo viên này đã được phân công.", "Trùng lặp", this);
                return;
            }

            // Tạo phân công mới
            var newAssignment = new ExamAssignment
            {
                RoomName = room.RoomName,
                TeacherName = teacher.TeacherName,
                RoomQuantity = room.Quantity,
                AssignedStudents = 0,
                Room = room,
                Teacher = teacher
            };

            newAssignment.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ExamAssignment.AssignedStudents))
                {
                    examViewModel.RecalculateRemainingStudents();
                    RemainingStudents.Text = examViewModel.RemainingStudentsText;
                }
            };

            examViewModel.ExamAssignments.Add(newAssignment);

            // Xóa khỏi danh sách để ẩn khỏi ComboBox
            var existingRoom = examViewModel.RoomList.FirstOrDefault(r => r.Id == room.Id);
            if (existingRoom != null)
                examViewModel.RoomList.Remove(existingRoom);

            var existingTeacher = examViewModel.TeacherList.FirstOrDefault(t => t.Id == teacher.Id);
            if (existingTeacher != null)
                examViewModel.TeacherList.Remove(existingTeacher);

            // Reset lựa chọn
            Room.SelectedItem = null;
            Teacher.SelectedItem = null;

            examViewModel.RecalculateRemainingStudents();
            RemainingStudents.Text = examViewModel.RemainingStudentsText;
        }

        // Xóa phân công
        private async void DeleteAssignment_Click(object? sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is ExamAssignment item)
            {
                bool confirm = await MessageBoxUtil.ShowConfirm(
                    $"Bạn có chắc muốn xóa phòng '{item.RoomName}' được phân cho giáo viên '{item.TeacherName}' không?"
                );

                if (!confirm)
                    return;

                // Thêm lại phòng và giáo viên vừa xóa
                if (item.Room != null && !examViewModel.RoomList.Any(r => r.Id == item.Room.Id))
                    examViewModel.RoomList.Add(item.Room);

                if (item.Teacher != null && !examViewModel.TeacherList.Any(t => t.Id == item.Teacher.Id))
                    examViewModel.TeacherList.Add(item.Teacher);

                // Xóa phân công khỏi danh sách
                examViewModel.ExamAssignments.Remove(item);

                examViewModel.RecalculateRemainingStudents();
                RemainingStudents.Text = examViewModel.RemainingStudentsText;
            }
        }


        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            
        }
    }
}