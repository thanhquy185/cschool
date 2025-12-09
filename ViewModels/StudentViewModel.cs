using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using Utils;
using ClosedXML.Excel;
using System.Reactive.Threading.Tasks;
using Services;

namespace ViewModels
{
    public partial class StudentViewModel : ViewModelBase
    {
        // Tiêu đề trang
        public string TitlePage { get; } = "Quản lý học sinh";
        // Mô tả trang
        public string DescriptionPage { get; } = "Quản lý thông tin học sinh";
        // Danh sách học sinh
        public ObservableCollection<StudentModel> Students { get; }
        // Dữ liệu hiển thị (lọc / tìm kiếm)
        public ObservableCollection<StudentModel> StudentsTemp { get; }
        public ObservableCollection<StudentModel> AllStudents { get; }
        public StudentModel? StudentDetails { get; set; }
        public string FilterKeyword { get; set; } = "";
        public string FilterStatus { get; set; } = "";

        // Các command thao tác với học sinh
        public ReactiveCommand<Unit, ObservableCollection<StudentModel>> GetStudentsCommand { get; }
        public ReactiveCommand<int, StudentModel?> GetStudentByIdCommand { get; }
        public ReactiveCommand<StudentModel, bool> CreateStudentCommand { get; }
        public ReactiveCommand<StudentModel, bool> UpdateStudentCommand { get; }
        public ReactiveCommand<StudentModel, bool> LockStudentCommand { get; }
        public List<string> LearnStatusOptions { get; } = new()
        {
            "--- Chọn Tình trạng học ---",
            "Đang học",
            "Tốt nghiệp",
            "Bảo lưu",
            "Nghỉ học"
        };
        public List<string> GenderOptions { get; } = new()
        {
            "--- Chọn Giới tính ---",
            "Nam",
            "Nữ"
        };

        // Hàm lọc dữ liệu dựa trên Students
        public void ApplyFilter()
        {
            if (Students == null || Students.Count == 0)
                return;

            var filtered = Students.Where(s =>
            {
                bool matchKeyword =
                    string.IsNullOrWhiteSpace(FilterKeyword) ||
                    s.Fullname.Contains(FilterKeyword, StringComparison.OrdinalIgnoreCase) ||
                    s.Id.ToString().Contains(FilterKeyword);

                bool matchStatus =
                    string.IsNullOrWhiteSpace(FilterStatus) ||
                    FilterStatus == "--- Chọn Tình trạng học ---" ||
                    s.LearnStatus.Equals(FilterStatus, StringComparison.OrdinalIgnoreCase);

                return matchKeyword && matchStatus;
            }).ToList();

            // Cập nhật StudentsTemp hiển thị
            StudentsTemp.Clear();
            foreach (var s in filtered)
                StudentsTemp.Add(s);
        }
        public StudentViewModel()
        {
            // Load danh sách ban đầu
            Students = new ObservableCollection<StudentModel>();
            StudentsTemp = new ObservableCollection<StudentModel>();
            AllStudents = new ObservableCollection<StudentModel>();

            var students = AppService.StudentService.GetStudents();
            foreach (var student in students)
            {
                Students.Add(student);
                StudentsTemp.Add(student);
            }

            // Lấy danh sách học sinh
            GetStudentsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var students = AppService.StudentService.GetStudents();
                Students.Clear();
                foreach (var student in students)
                    Students.Add(student);

                // Reset lại danh sách hiển thị
                StudentsTemp.Clear();
                foreach (var s in Students)
                    StudentsTemp.Add(s);

                return StudentsTemp;
            });

            // Lấy học sinh theo ID
            GetStudentByIdCommand = ReactiveCommand.Create<int, StudentModel?>(id =>
            {
                var student = AppService.StudentService.GetStudentById(id);
                StudentDetails = student;
                return student;
            });

            // Tạo học sinh mới
            CreateStudentCommand = ReactiveCommand.CreateFromTask<StudentModel, bool>(async (student) =>
            {
                string newAvatarFile = await UploadService.SaveImageAsync(student.AvatarFile, "student", AppService.StudentService.GetIdLastStudent() + 1);
                student.Avatar = newAvatarFile;
                var result = AppService.StudentService.CreateStudent(student);
                if (result)
                {
                    return true;
                }
                return false;
            });

            // Cập nhật thông tin học sinh
            UpdateStudentCommand = ReactiveCommand.CreateFromTask<StudentModel, bool>(async (student) =>
            {
                // Nếu người dùng có chọn ảnh mới (AvatarFile không rỗng)
                if (!string.IsNullOrEmpty(student.AvatarFile) && File.Exists(student.AvatarFile))
                {
                    string? newAvatarFile = await UploadService.SaveImageAsync(student.AvatarFile, "student", student.Id);
                    student.Avatar = newAvatarFile;
                }
                var result = AppService.StudentService.UpdateStudent(student);
                if (result)
                {
                    return true;
                }
                return false;
            });

            // Khóa học sinh
            LockStudentCommand = ReactiveCommand.Create<StudentModel, bool>(student =>
            {
                var result = AppService.StudentService.LockStudent(student);
                if (result)
                {
                    return true;
                }
                return false;
            });
        }

        public async Task ImportExcel(string filePath)
        {
            try
            {
                var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn nhập danh sách học sinh từ file Excel này?");
                if (!confirm)
                    return;

                var allstudents = AppService.StudentService.GetAllStudents();
                AllStudents.Clear();
                foreach (var student in allstudents)
                    AllStudents.Add(student);

                int successCount = 0;
                int failCount = 0;
                int duplicateCount = 0;

                using (var workbook = new XLWorkbook(filePath))
                {
                    var ws = workbook.Worksheet(1);
                    var rows = ws.RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        try
                        {
                            // đọc dữ liệu như cũ
                            string fullname = row.Cell(1).GetString()?.Trim() ?? "";
                            string avatar = row.Cell(2).GetString();
                            string birthdayRaw = row.Cell(3).GetString();
                            string birthday = birthdayRaw;

                            if (row.Cell(3).TryGetValue(out DateTime dt))
                                birthday = dt.ToString("yyyy-MM-dd");
                            else if (DateTime.TryParse(birthdayRaw, out var parsed))
                                birthday = parsed.ToString("yyyy-MM-dd");

                            var gender = row.Cell(4).GetString();
                            var ethnicity = row.Cell(5).GetString();
                            var religion = row.Cell(6).GetString();
                            var address = row.Cell(7).GetString();
                            var phone = row.Cell(8).GetString();
                            var email = row.Cell(9).GetString();
                            var learnYear = row.Cell(10).GetString();
                            var learnStatus = row.Cell(11).GetString();

                            // check trùng
                            bool exists = AllStudents.Any(s =>
                                string.Equals(s.Fullname, fullname, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(s.Gender, gender, StringComparison.OrdinalIgnoreCase) &&
                                DateTime.TryParse(s.BirthDay, out var bDate) &&
                                DateTime.TryParse(birthday, out var bNew) &&
                                bDate.Date == bNew.Date &&
                                string.Equals(s.Ethnicity, ethnicity, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(s.Religion, religion, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(s.Phone, phone, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(s.Email, email, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(s.Address, address, StringComparison.OrdinalIgnoreCase));

                            if (exists)
                            {
                                duplicateCount++;
                                continue;
                            }

                            var student = new StudentModel
                            {
                                Fullname = fullname,
                                Avatar = string.IsNullOrWhiteSpace(avatar) ? null : avatar,
                                BirthDay = birthday,
                                Gender = gender,
                                Ethnicity = ethnicity,
                                Religion = religion,
                                Address = address,
                                Phone = phone,
                                Email = email,
                                LearnYear = learnYear,
                                LearnStatus = learnStatus
                            };

                            bool isSuccess = await CreateStudentCommand.Execute(student).ToTask();
                            if (isSuccess)
                            {
                                successCount++;
                                int newId = AppService.StudentService.GetIdLastStudent();
                                var createdStudent = AppService.StudentService.GetStudentById(newId);
                                StudentsTemp.Add(createdStudent);
                                AllStudents.Add(createdStudent);
                            }
                            else failCount++;
                        }
                        catch
                        {
                            failCount++;
                        }
                    }
                }

                // await GetStudentsCommand.Execute().ToTask();

                await MessageBoxUtil.ShowSuccess(
                    $"Nhập Excel hoàn tất!\n" +
                    $"Nhập thành công: {successCount} học sinh\n" +
                    $"Trùng thông tin: {duplicateCount} học sinh\n" +
                    $"Lỗi khi nhập: {failCount} học sinh"
                );
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowError("Lỗi khi nhập Excel: " + ex.Message);
            }
        }

        public async Task ExportExcel(string filePath)
        {
            try
            {
                var allstudents = AppService.StudentService.GetAllStudents();
                AllStudents.Clear();
                foreach (var student in allstudents)
                    AllStudents.Add(student);
                if (AllStudents == null || !AllStudents.Any())
                    throw new InvalidOperationException("Không có dữ liệu để xuất!");

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Students");

                    // ===== HEADER =====
                    string[] headers =
                    {
                        "ID", "Họ và tên", "Ảnh đại diện", "Ngày sinh", "Giới tính",
                        "Dân tộc", "Tôn giáo", "Số điện thoại", "Email", "Địa chỉ",
                        "Năm học", "Tình trạng học", "Trạng thái"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    // ===== DATA =====
                    int row = 2;
                    foreach (var s in AllStudents)
                    {
                        worksheet.Cell(row, 1).Value = s.Id;
                        worksheet.Cell(row, 2).Value = s.Fullname;
                        worksheet.Cell(row, 3).Value = "Hình đại diện";
                        worksheet.Cell(row, 4).Value = s.BirthDayDisplay ?? s.BirthDay;
                        worksheet.Cell(row, 5).Value = s.Gender;
                        worksheet.Cell(row, 6).Value = s.Ethnicity;
                        worksheet.Cell(row, 7).Value = s.Religion;
                        worksheet.Cell(row, 8).Value = s.Phone;
                        worksheet.Cell(row, 9).Value = s.Email;
                        worksheet.Cell(row, 10).Value = s.Address;
                        worksheet.Cell(row, 11).Value = s.LearnYear;
                        worksheet.Cell(row, 12).Value = s.LearnStatus;
                        worksheet.Cell(row, 13).Value = s.Status == 1 ? "Hoạt động" : "Đã khóa";

                        // Viền mỏng quanh ô
                        for (int col = 1; col <= headers.Length; col++)
                            worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row++;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xuất Excel: " + ex.Message, ex);
            }
        }
    }
}
