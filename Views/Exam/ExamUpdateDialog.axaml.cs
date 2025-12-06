using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels;
using Utils;
using System;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;

namespace Views.Exam
{
    public partial class ExamUpdateDialog : Window
    {
        public ExamViewModel examViewModel { get; set; }

        public ExamUpdateDialog(ExamViewModel vm)
        {
            InitializeComponent();
            examViewModel = vm;
            DataContext = vm;

            RecalculateRemainingStudentsUpdate();
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
            examViewModel.ExamAssignments.Clear();
        }

        public void RecalculateRemainingStudentsUpdate()
        {
            // Kiểm tra grade
            if (examViewModel.SelectedGrade == null)
                return;

            // Kiểm tra term
            var term = Term.SelectedItem as TermModel;
            if (term == null)
                return;

            int grade = examViewModel.SelectedGrade.Value;

            // Lấy tổng số học sinh trong khối
            int total = AppService.ExamService.GetStudentGrade(grade, term.Id);

            // Tổng số học sinh đã phân công
            int assigned = examViewModel.ExamAssignments.Sum(a => a.AssignedStudents);

            int remaining = Math.Max(total - assigned, 0);

            examViewModel.RemainingStudents = remaining;
            examViewModel.RemainingStudentsText = $"Còn lại {remaining} học sinh chưa được phân vào phòng thi";

            RemainingStudents.Text = examViewModel.RemainingStudentsText;
        }


        private void OnGradeChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (Grade?.SelectedItem is string gradeStr && int.TryParse(gradeStr, out int grade))
            {
                examViewModel.SelectedGrade = grade;
                RecalculateRemainingStudentsUpdate();
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
            if (Term?.SelectedItem is TermModel)
            {
                RecalculateRemainingStudentsUpdate();
                RemainingStudents.Text = examViewModel.RemainingStudentsText;
            }
            else
            {
                examViewModel.SelectedUpdateTerm = null;
                RemainingStudents.Text = "";
            }
        }

        // Thêm phân công
        private async void AddAssignment_Click(object? sender, RoutedEventArgs e)
        {
            var room = Room.SelectedItem as RoomModel;
            var teacher = Teacher.SelectedItem as TeacherModel;

            if (room == null || teacher == null)
            {
                await MessageBoxUtil.ShowWarning("Vui lòng chọn phòng thi và giáo viên.",
                    "Thiếu thông tin", this);
                return;
            }

            // Kiểm tra trùng (nếu cùng phòng hoặc cùng giáo viên đã được chọn)
            bool alreadyExists = examViewModel.ExamAssignments
                .Any(a => a.RoomName == room.RoomName || a.TeacherName == teacher.TeacherName);

            if (alreadyExists)
            {
                await MessageBoxUtil.ShowWarning("Phòng hoặc giáo viên này đã được phân công.",
                    "Trùng lặp", this);
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

            newAssignment.PropertyChanged += (_, e2) =>
            {
                if (e2.PropertyName == nameof(ExamAssignment.AssignedStudents))
                {
                    RecalculateRemainingStudentsUpdate();
                    RemainingStudents.Text = examViewModel.RemainingStudentsText;
                }
            };

            examViewModel.ExamAssignments.Add(newAssignment);

            // Loại khỏi combobox
            examViewModel.RoomListUpdate.Remove(
                examViewModel.RoomListUpdate.FirstOrDefault(r => r.Id == room.Id)
            );

            examViewModel.TeacherListUpdate.Remove(
                examViewModel.TeacherListUpdate.FirstOrDefault(t => t.Id == teacher.Id)
            );

            // Reset lựa chọn
            Room.SelectedItem = null;
            Teacher.SelectedItem = null;

            RecalculateRemainingStudentsUpdate();
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
                if (!confirm) return;

                // thêm lại vào combobox:
                if (item.Room != null && !examViewModel.RoomListUpdate.Any(r => r.Id == item.Room.Id))
                    examViewModel.RoomListUpdate.Add(item.Room);

                if (item.Teacher != null && !examViewModel.TeacherListUpdate.Any(t => t.Id == item.Teacher.Id))
                    examViewModel.TeacherListUpdate.Add(item.Teacher);

                examViewModel.ExamAssignments.Remove(item);

                RecalculateRemainingStudentsUpdate();
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
            var grade = Grade.SelectedItem?.ToString();
            var term = Term.SelectedItem as TermModel;
            if (sender is not TextBox tb || tb.DataContext is not ExamAssignment assignment)
                return;

            int value = int.TryParse(tb.Text, out int v) ? v : 0;
            if (value < 0) value = 0;

            int maxCapacity = assignment.Room?.Quantity ?? 50;

            int totalStudents = AppService.ExamService.GetStudentGrade(Convert.ToInt32(grade),term.Id);

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
            RecalculateRemainingStudentsUpdate();
        }

        // Xác nhận cập nhật
        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            var id = examViewModel.ExamDetails.Id;
            if (id == null)
            {
                await MessageBoxUtil.ShowError("Không tìm thấy dữ liệu lịch thi!", owner: this);
                return;
            }

            var grade = Grade.SelectedItem?.ToString();
            var subject = Subject.SelectedItem as SubjectModel;
            var term = Term.SelectedItem as TermModel;
            var examDate = ExamDate.SelectedDate?.DateTime ?? DateTime.Now;
            var startTime = StartTime.SelectedTime;
            var endTime = EndTime.SelectedTime;
            var assignments = examViewModel.ExamAssignments;

            if (string.IsNullOrWhiteSpace(grade))
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn khối.", owner: this);
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

            if (startTime == null || endTime == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn thời gian thi.", owner: this);
                return;
            }

            if (assignments == null || assignments.Count == 0)
            {
                await MessageBoxUtil.ShowError("Vui lòng phân công ít nhất một phòng thi và giáo viên.", owner: this);
                return;
            }

            if (endTime <= startTime)
            {
                await MessageBoxUtil.ShowError("Giờ kết thúc phải sau giờ bắt đầu!", owner: this);
                return;
            }
            
            if (examViewModel.RemainingStudents > 0)
            {
                await MessageBoxUtil.ShowError("Vui lòng phân công đủ học sinh vào các phòng thi.", owner: this);
                return;
            }

            string startDT = $"{examDate:yyyy-MM-dd} {startTime:hh\\:mm\\:ss}";
            string endDT = $"{examDate:yyyy-MM-dd} {endTime:hh\\:mm\\:ss}";

            var update = new ExamUpdateModel
            {
                ExamDetailId = id,
                SubjectId = subject.Id,
                GradeId = int.Parse(grade),
                TermId = term.Id,
                ExamDate = examDate.ToString("yyyy-MM-dd"),
                StartTime = startDT,
                EndTime = endDT,
                Assignments = examViewModel.ExamAssignments.Select(a => new ExamAssignmentCreateModel
                {
                    RoomId = a.Room?.Id ?? 0,
                    TeacherId = a.Teacher?.Id ?? 0,
                    AssignedStudents = a.AssignedStudents
                })
                .ToList()
            };

            // kiểm tra trùng môn học trong kỳ (không tính chính nó)
            if (await HasSubjectExamInTermAsync(update, examViewModel.Exams, this, id))
                return;

            // kiểm tra trùng lịch thi
            if (await HasExamConflictPerGradeAsync(update, examViewModel.Exams, this, id))
                return;

            bool success = await examViewModel.UpdateExamCommand.Execute(update).ToTask();
            if (success)
            {
                await MessageBoxUtil.ShowSuccess("Cập nhật lịch thi thành công!", owner: this);
                await examViewModel.GetExamsCommand.Execute().ToTask();
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Cập nhật thất bại!", owner: this);
            }
        }

        // Kiểm tra đã có môn thi chưa (trừ chính nó)
        private async Task<bool> HasSubjectExamInTermAsync(ExamUpdateModel newExam, IEnumerable<ExamModel> existingExams, Window owner, int currentId)
        {
            foreach (var exam in existingExams)
            {
                if (exam.Id == currentId)
                    continue;

                if (exam.SubjectId == newExam.SubjectId &&
                    Convert.ToInt32(exam.Grade) == newExam.GradeId &&
                    exam.TermId == newExam.TermId)
                {
                    await MessageBoxUtil.ShowError(
                        $"Môn {exam.Subject} của khối {exam.Grade} trong học kỳ {exam.TermName} "
                        + $"đã có lịch thi vào {DateTime.Parse(exam.StartTime):dd/MM/yyyy}."
                        + $"từ {DateTime.Parse(exam.StartTime):HH:mm} đến {DateTime.Parse(exam.EndTime):HH:mm}. "
                        + $"Không thể tạo thêm lịch thi cho môn này!",
                        "Đã tồn tại",
                        owner: owner
                    );

                    return true;
                }
            }
            return false;
        }

        // Kiểm tra trùng lịch thi khi UPDATE (trừ chính nó)
        private async Task<bool> HasExamConflictPerGradeAsync(ExamUpdateModel newExam, IEnumerable<ExamModel> existingExams, Window owner, int currentId)
        {
            DateTime newStart = DateTime.Parse(newExam.StartTime);
            DateTime newEnd = DateTime.Parse(newExam.EndTime);

            foreach (var exam in existingExams)
            {
                if (exam.Id == currentId)
                    continue;

                if (!DateTime.TryParse(exam.StartTime, out var existStart))
                    continue;
                if (!DateTime.TryParse(exam.EndTime, out var existEnd))
                    continue;

                if (existStart.Date != newStart.Date)
                    continue;

                bool separated = newEnd <= existStart || newStart >= existEnd;

                if (!separated)
                {
                    await MessageBoxUtil.ShowError(
                        $"Khối {exam.Grade} đã có lịch thi môn {exam.Subject} "
                        + $"từ {existStart:HH:mm} đến {existEnd:HH:mm}.",
                        "Trùng thời gian",
                        owner: owner
                    );
                    return true;
                }
            }

            return false;
        }
    
    }

}