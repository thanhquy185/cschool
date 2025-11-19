using Avalonia.Controls;
using Avalonia.Interactivity;
using cschool.ViewModels;
using cschool.Utils;
using System;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cschool.Views.Exam
{
    public partial class ExamCreateDialog : Window
    {
        public ExamViewModel examViewModel { get; set; }
        public ExamCreateDialog(ExamViewModel vm)
        {
            InitializeComponent();
            examViewModel = vm;
            DataContext = vm;
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
            examViewModel.ExamAssignments.Clear();
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

        // private async void AssignedStudents_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        // {
        //     if (sender is NumericUpDown nud && nud.DataContext is ExamAssignment a)
        //     {
        //         int maxAllowed = Math.Min(a.RoomQuantity, examViewModel.RemainingStudents);

        //         if (a.AssignedStudents > maxAllowed)
        //         {
        //             a.AssignedStudents = maxAllowed;
        //             nud.Value = maxAllowed;

        //             await MessageBoxUtil.ShowWarning(
        //                 $"Số học sinh vượt quá giới hạn! (Tối đa: {maxAllowed})",
        //                 "Giới hạn vượt mức",
        //                 this
        //             );
        //         }

        //         examViewModel.RecalculateRemainingStudents();
        //     }
        // }

        // Xác nhận lưu
        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu từ các TextBox, ComboBox, DatePicker
            var grade = (Grade.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var subject = Subject.SelectedItem as SubjectModel;
            var examDate = ExamDate.SelectedDate?.DateTime ?? DateTime.Now;
            var startTime = StartTime.SelectedTime;
            var endTime = EndTime.SelectedTime;
            var term = Term.SelectedItem as TermModel;
            var assignments = examViewModel.ExamAssignments;
            // Gộp ngày + giờ thành datetime string
            var startDateTime = $"{examDate:yyyy-MM-dd} {startTime:hh\\:mm\\:ss}";
            var endDateTime = $"{examDate:yyyy-MM-dd} {endTime:hh\\:mm\\:ss}";

            // Kiểm tra xác nhận
            var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn thêm lịch thi này?");
            if (!confirm)
                return;

            // Kiểm tra dữ liệu hợp lệ
            if (string.IsNullOrWhiteSpace(grade))
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn khối/lớp.", owner: this);
                return;
            }

            if (subject == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn môn học.", owner: this);
                return;
            }

            if (term == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn học kỳ.", owner: this);
                return;
            }

            if (examDate == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn ngày thi.", owner: this);
                return;
            }

            if (startTime == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn giờ bắt đầu.", owner: this);
                return;
            }

            if (endTime == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn giờ kết thúc.", owner: this);
                return;
            }

            if (assignments == null || assignments.Count == 0)
            {
                await MessageBoxUtil.ShowError("Vui lòng phân công ít nhất một phòng thi và giáo viên.", owner: this);
                return;
            }

            // Kiểm tra logic thời gian
            if (endTime <= startTime)
            {
                await MessageBoxUtil.ShowError("Giờ kết thúc phải sau giờ bắt đầu.", owner: this);
                return;
            }

            // Khởi tạo model
            var examCreate = new ExamCreateModel
            {
                SubjectId = subject.Id,
                TermId = term.Id,
                GradeId = int.TryParse(grade, out int gradeId) ? gradeId : 0,
                ExamDate = examDate.ToString("yyyy-MM-dd"),
                StartTime = startDateTime,
                EndTime = endDateTime,
                Assignments = examViewModel.ExamAssignments.Select(a => new ExamAssignmentCreateModel
                {
                    RoomId = a.Room?.Id ?? 0,
                    TeacherId = a.Teacher?.Id ?? 0,
                    AssignedStudents = a.AssignedStudents
                }).ToList()
            };

            // Kiểm tra trùng lịch thi
            if (await HasExamConflictPerGradeAsync(examCreate, examViewModel.Exams, this))
                return;

            // Xử lý lịch thi
            bool isSuccess = await examViewModel.CreateExamCommand.Execute(examCreate).ToTask();

            // Thông báo xử lý, nếu thành công thì ẩn dialog
            if (isSuccess)
            {
                await MessageBoxUtil.ShowSuccess("Thêm lịch thi thành công!", owner: this);
                await examViewModel.GetExamsCommand.Execute().ToTask();
                examViewModel.ExamAssignments.Clear();
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Thêm lịch thi thất bại!", owner: this);
                this.Close();
            }
        }

        // Hàm kiểm tra có trùng lịch thi
        private async Task<bool> HasExamConflictPerGradeAsync(ExamCreateModel newExam, IEnumerable<ExamModel> existingExams, Window owner)
        {
            try
            {
                // Parse lịch thi mới (đã bao gồm ngày + giờ)
                DateTime newStart = DateTime.Parse(newExam.StartTime);
                DateTime newEnd = DateTime.Parse(newExam.EndTime);

                foreach (var exam in existingExams)
                {
                    // 1. Cùng khối
                    if (Convert.ToInt32(exam.Grade) != newExam.GradeId)
                        continue;

                    // 2. Gộp ngày + giờ của exam cũ
                    if (!DateTime.TryParse($"{exam.StartTime}", out var existStart))
                        continue;

                    if (!DateTime.TryParse($"{exam.EndTime}", out var existEnd))
                        continue;

                    // 3. Kiểm tra cùng ngày
                    if (existStart.Date != newStart.Date)
                        continue;

                    // 4. Chồng lấn thời gian
                    bool isSeparated = newEnd <= existStart || newStart >= existEnd;

                    if (!isSeparated)
                    {
                        // Hai khoảng thời gian bị TRÙNG
                        await MessageBoxUtil.ShowError(
                            $"Khối {exam.Grade} đã có lịch thi môn {exam.Subject} "
                            + $"vào ngày {existStart:dd/MM/yyyy} "
                            + $"từ {existStart:HH:mm} đến {existEnd:HH:mm}. "
                            + $"Không thể thi 2 môn cùng thời gian!",
                            owner: owner
                        );
                        return true; // Có trùng -> không được thêm
                    }

                }

                return false;
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowError($"Lỗi kiểm tra trùng lịch thi: {ex.Message}", owner: owner);
                return true;
            }
        }

    }
}
