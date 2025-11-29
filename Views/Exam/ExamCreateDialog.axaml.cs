using Avalonia.Controls;
using Avalonia.Interactivity;
using cschool.ViewModels;
using cschool.Utils;
using System;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;

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

        private void OnTermChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (Term?.SelectedItem is TermModel selected)
            {
                examViewModel.SelectedUpdateTerm = selected.Id;
                RemainingStudents.Text = examViewModel.RemainingStudentsText;
            }
            else
            {
                examViewModel.SelectedUpdateTerm = null;
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

        // Chặn gõ ký tự không phải số
        private async void TextBox_TextInput(object? sender, TextInputEventArgs e)
        {
            // Nếu ký tự không phải số -> chặn
            if (!e.Text.All(char.IsDigit))
                await MessageBoxUtil.ShowError("Vui lòng nhập số hợp lệ.", owner: this);
                e.Handled = true;
        }

        // Giới hạn số khi TextBox mất focus
        private async void TextBox_LostFocus(object? sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb || tb.DataContext is not ExamAssignment assignment)
                return;

            int value = int.TryParse(tb.Text, out int v) ? v : 0;
            if (value < 0) value = 0;

            int maxCapacity = assignment.Room?.Quantity ?? 50;

            int totalStudents = AppService.ExamService.GetStudentGrade(
                examViewModel.SelectedGrade ?? 0,
                examViewModel.SelectedUpdateTerm ?? 0);

            int assignedSum = examViewModel.ExamAssignments?.Sum(a => a.AssignedStudents) ?? 0;

            int assignedWithoutCurrent = assignedSum - assignment.AssignedStudents;

            int remainingBefore = Math.Max(totalStudents - assignedWithoutCurrent, 0);

            int limit = Math.Min(maxCapacity, remainingBefore);

            int corrected = Math.Clamp(value, 0, limit);

            if (value > maxCapacity)
            {
                await MessageBoxUtil.ShowWarning(
                    $"Số học sinh phân công vượt quá sức chứa của phòng (sức chứa: {maxCapacity}).",
                    owner: this);
            }
            else if (value > remainingBefore)
            {
                await MessageBoxUtil.ShowWarning(
                    $"Số học sinh phân công vượt quá số học sinh còn lại (còn: {remainingBefore}).",
                    owner: this);
            }

            tb.Text = corrected.ToString();

            assignment.AssignedStudents = corrected;
            examViewModel.RecalculateRemainingStudents();
        }

        // Xác nhận lưu lịch thi
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

            // Kiểm tra xem đã phân hết học sinh vào phòng chưa
            if (examViewModel.RemainingStudents > 0)
            {
                await MessageBoxUtil.ShowError("Vui lòng phân công đủ học sinh vào các phòng thi.", owner: this);
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

            // Kiểm tra đã có môn thi trong học kỳ chưa
            if (await HasSubjectExamInTermAsync(examCreate, examViewModel.Exams, this))
                return;

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
                    // Gộp ngày + giờ của exam cũ
                    if (!DateTime.TryParse($"{exam.StartTime}", out var existStart))
                        continue;

                    if (!DateTime.TryParse($"{exam.EndTime}", out var existEnd))
                        continue;

                    // Kiểm tra cùng ngày
                    if (existStart.Date != newStart.Date)
                        continue;

                    // Chồng lấn thời gian
                    bool isSeparated = newEnd <= existStart || newStart >= existEnd;

                    if (!isSeparated)
                    {
                        // Hai khoảng thời gian bị TRÙNG
                        await MessageBoxUtil.ShowError(
                            $"Khối {exam.Grade} đã có lịch thi môn {exam.Subject} "
                            + $"vào ngày {existStart:dd/MM/yyyy} "
                            + $"từ {existStart:HH:mm} đến {existEnd:HH:mm}. "
                            + $"Không thể thi 2 môn cùng thời gian!",
                            "Trùng lịch thi",
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

        // Hàm kiểm tra đã có môn thi trong học kỳ chưa
        private async Task<bool> HasSubjectExamInTermAsync(ExamCreateModel newExam, IEnumerable<ExamModel> existingExams, Window owner)
        {
            try
            {
                // Lấy dữ liệu từ exam mới
                int newSubject = newExam.SubjectId;
                int newGrade = newExam.GradeId;
                int newTermId = newExam.TermId;

                // Kiểm tra xem đã có lịch thi cho môn này – khối này – học kỳ này chưa
                foreach (var exam in existingExams)
                {
                    if (exam.SubjectId == newSubject &&
                        Convert.ToInt32(exam.Grade) == newGrade &&
                        exam.TermId == newTermId)
                    {
                        await MessageBoxUtil.ShowError(
                            $"Môn {exam.Subject} của khối {exam.Grade} trong {exam.TermName} - {exam.TermYear} "
                            + $"đã có lịch thi vào ngày {DateTime.Parse(exam.StartTime):dd/MM/yyyy} "
                            + $"từ {DateTime.Parse(exam.StartTime):HH:mm} đến {DateTime.Parse(exam.EndTime):HH:mm}. "
                            + $"Không thể tạo thêm lịch thi cho môn này!",
                            "Đã tồn tại lịch thi",
                            owner: owner
                        );

                        return true; // Đã tồn tại → không cho thêm
                    }
                }

                return false; // Không tìm thấy → được phép tạo mới
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowError($"Lỗi kiểm tra môn đã có lịch thi: {ex.Message}", owner: owner);
                return true;
            }
        }

    }
}