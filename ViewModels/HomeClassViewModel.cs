using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Services;
using ClosedXML.Excel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Utils;
using Avalonia.Threading;

namespace ViewModels;

public partial class HomeClassViewModel : ViewModelBase
{
   
    private readonly HomeClassService _service;
    public int CURRENT_TEACHER_ID{get; set;} 

    [ObservableProperty]
    private bool _hasHomeroomClass = true; // Mặc định là có lớp chủ nhiệm

    [ObservableProperty]
    private string _nameTeacher = "";
    
    [ObservableProperty]
    private string _nameClass = "";
    
    [ObservableProperty]
    private string _nameTerm = "";
    
    [ObservableProperty]
    private string _year = "";
    
    [ObservableProperty]
    private string? _searchName;
    
    [ObservableProperty]
    private string? _selectedConductLevel;
    
    [ObservableProperty]
    private HomeClass? _selectedStudent;
    
    [ObservableProperty]
    private TermModel? _selectedTerm;
    
    [ObservableProperty]
    private string _selectedStudentName = "";

    public ObservableCollection<HomeClass> Students { get; } = new();
    public ObservableCollection<Information> Information { get; } = new();
    public ObservableCollection<TermModel> Terms { get; } = new();
    public ObservableCollection<DetailScore> StudentDetailScores { get; } = new();
    
    [ObservableProperty]
    private ObservableCollection<string> _conductOptions = new()
    {
        "Giỏi",
        "Khá", 
        "Trung bình",
        "Yếu"
    };

    #region Constructor và Load dữ liệu ban đầu
    public HomeClassViewModel()
    {
        var currentUserLogin = SessionService.currentUserLogin;
        Console.WriteLine("Trang lớp chủ nhiệm: " + currentUserLogin?.Fullname);

        
        this._service = AppService.HomeClassService;
        
        // Load danh sách học kỳ của giáo viên ID = 3
        LoadTermsCommand.Execute(null);
        
        Console.WriteLine($"Khởi tạo HomeClassViewModel với Teacher ID: {CURRENT_TEACHER_ID}");
    }

    [RelayCommand]
    private void LoadTerms()
    {
        try
        {
            Terms.Clear();
            var terms = _service.GetTerm(CURRENT_TEACHER_ID);
        if (terms == null || !terms.Any())
            {
                HasHomeroomClass = false;
            
                
                var currentUserLogin = SessionService.currentUserLogin;
                NameTeacher = currentUserLogin?.Fullname ?? "Giáo viên";
                
                return;
            }
            
            foreach (var term in terms)
            {
                Terms.Add(term);
            }
            
            Console.WriteLine($"Đã load {Terms.Count} học kỳ cho giáo viên ID: {CURRENT_TEACHER_ID}");
            
            // Tự động chọn học kỳ đầu tiên nếu có
            if (Terms.Count > 0)
            {
                SelectedTerm = Terms[0];
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi load học kỳ: {ex.Message}");
        }
    }
    #endregion
    public HomeClassViewModel(int teacherId)
    {
        
        if (teacherId > 0)
        {
            CURRENT_TEACHER_ID = teacherId;
        }
        else
        {
            CURRENT_TEACHER_ID = 0;
            
        }
        
        this._service = AppService.HomeClassService;
        
        // Load danh sách học kỳ của giáo viên
        LoadTermsCommand.Execute(null);
        
        Console.WriteLine($"Khởi tạo HomeClassViewModel với Teacher ID: {CURRENT_TEACHER_ID}");
    }
    #region Load dữ liệu khi chọn học kỳ
    [RelayCommand]
    private void LoadDataByTerm()
    {
        if (SelectedTerm == null)
        {
            Console.WriteLine("Vui lòng chọn học kỳ");
            return;
        }

        try
        {
            // Load thông tin lớp và danh sách học sinh
            var students = _service.GetStudents(CURRENT_TEACHER_ID, SelectedTerm.Id);
            var information = _service.GetInformation(CURRENT_TEACHER_ID, SelectedTerm.Id);
            
            // Clear dữ liệu cũ
            Students.Clear();
            Information.Clear();
    
            // Thêm dữ liệu mới
            foreach (var student in students)
            {
                Students.Add(student);
            }
            
            foreach (var info in information)
            {
                Information.Add(info);
            }
          
            
            // Cập nhật thông tin hiển thị
            var firstInfo = information.FirstOrDefault();
            if (firstInfo != null)
            {
                NameTeacher = firstInfo.NameTeacher;
                NameClass = firstInfo.NameClass;
                NameTerm = firstInfo.NameTerm;
                Year = firstInfo.Year.ToString();
            }
            else
            {
                // Reset thông tin nếu không có dữ liệu
                NameTeacher = "Chưa có thông tin";
                NameClass = "Chưa có thông tin";
                NameTerm = SelectedTerm.Name;
                Year = SelectedTerm.Year.ToString();
            }
            
            Console.WriteLine($"Đã load {Students.Count} học sinh cho học kỳ {SelectedTerm.Id}  {SelectedTerm.Name} năm {SelectedTerm.Year}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi load dữ liệu: {ex.Message}");
        }
    }

    // Sự kiện khi SelectedTerm thay đổi
    partial void OnSelectedTermChanged(TermModel? value)
    {
        if (value != null)
        {
            Console.WriteLine($"Đã chọn học kỳ: {value.Name} - Năm: {value.Year}");
            LoadDataByTermCommand.Execute(null);
        }
    }
    #endregion

    #region Tìm kiếm học sinh
    [RelayCommand]
    public void Search()
    {
        if (SelectedTerm == null)
        {
            MessageBoxUtil.ShowError("Vui lòng chọn học kỳ trước khi tìm kiếm");
            return;
        }

        // Sửa lại phương thức Search trong service để nhận teacherId và year
        var results = _service.Search(CURRENT_TEACHER_ID, SelectedTerm.Id, SearchName ?? "");
        Dispatcher.UIThread.Post(() =>
        {
            Students.Clear();
            foreach (var student in results)
                Students.Add(student);
            
            Console.WriteLine($"Tìm kiếm được {results.Count} học sinh");
        });
    }

    partial void OnSearchNameChanged(string? value)
    {
        if (SelectedTerm != null)
        {
            Search();
        }
    }

    [RelayCommand]
    public void ResetSearch()
    {
        SearchName = string.Empty;
        LoadDataByTermCommand.Execute(null);
    }
    #endregion

    #region Xem chi tiết học sinh   
    // [RelayCommand]
    // private async Task ShowStudentDetail()
    // {
    //     if (SelectedStudent == null)
    //     {
    //         await MessageBoxUtil.ShowError("Vui lòng chọn học sinh để xem chi tiết");
    //         return;
    //     }

    //     try
    //     {
    //         SelectedStudentName = SelectedStudent.StudentName;
    //         LoadStudentDetailScores(SelectedStudent.StudentId);
            
    //         // Debug log
    //         Console.WriteLine($"=== DEBUG SHOW STUDENT DETAIL ===");
    //         Console.WriteLine($"Student: {SelectedStudentName}");
    //         Console.WriteLine($"Scores Count: {StudentDetailScores.Count}");
            
    //         // Tạo và hiển thị dialog
    //         var dialog = new HomeClassDetailDialog
    //         {
    //             DataContext = this
    //         };
            
    //         await dialog.ShowDialog((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Lỗi khi tải chi tiết điểm: {ex.Message}");
    //         await MessageBoxUtil.ShowError($"Lỗi khi tải chi tiết điểm: {ex.Message}");
    //     }
    // }

    public event EventHandler<Models.HomeClass>? RequestShowStudentDetail;

    [RelayCommand]
    private async Task ShowStudentDetail()
    {
        if (SelectedStudent == null)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn học sinh để xem chi tiết");
            return;
        }

        try
        {
            SelectedStudentName = SelectedStudent.StudentName;
            LoadStudentDetailScores(SelectedStudent.StudentId);

            Console.WriteLine($"=== DEBUG SHOW STUDENT DETAIL ===");
            Console.WriteLine($"Student: {SelectedStudentName}");
            Console.WriteLine($"Scores Count: {StudentDetailScores.Count}");

            // ❌ KHÔNG MỞ DIALOG Ở VIEWMODEL
            // Thay vào đó: gửi tín hiệu cho View
            RequestShowStudentDetail?.Invoke(this, SelectedStudent);        
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi tải chi tiết điểm: {ex.Message}");
            await MessageBoxUtil.ShowError($"Lỗi khi tải chi tiết điểm: {ex.Message}");
        }
    }

    [RelayCommand]
    private void LoadStudentDetailScores(int studentId)
    {
        StudentDetailScores.Clear();

        // Lấy tất cả các loại điểm
        var diemMieng = _service.GetDetailScores1(studentId);
        var diem15p = _service.GetDetailScores2(studentId);
        var diemGK = _service.GetDetailScores3(studentId);
        var diemCK = _service.GetDetailScores4(studentId);
        var diemTB = _service.GetDetailScoresTB(studentId);

        if (diemMieng == null || diem15p == null || diemGK == null || diemCK == null)
        {
            Console.WriteLine("Không có dữ liệu điểm chi tiết");
            return;
        }
        
        // Gom điểm theo môn học
        var allSubjects = diemMieng.Select(d => d.NameSubject)
                                 .Union(diem15p.Select(d => d.NameSubject))
                                 .Union(diemGK.Select(d => d.NameSubject))
                                 .Union(diemCK.Select(d => d.NameSubject))
                                 .Union(diemTB.Select(d => d.NameSubject))
                                 .Distinct();

        foreach (var subject in allSubjects)
        {
            var detailScore = new DetailScore
            {
                NameSubject = subject,
                DiemMieng = diemMieng.FirstOrDefault(d => d.NameSubject == subject)?.DiemMieng ?? new List<float>(),
                Diem15p = diem15p.FirstOrDefault(d => d.NameSubject == subject)?.Diem15p ?? new List<float>(),
                DiemGK = diemGK.FirstOrDefault(d => d.NameSubject == subject)?.DiemGK ?? 0,
                DiemCK = diemCK.FirstOrDefault(d => d.NameSubject == subject)?.DiemCK ?? 0,
                DiemTrungBinh = diemTB.FirstOrDefault(d => d.NameSubject == subject)?.DiemTrungBinh ?? 0
                
            };
            // detailScore.DiemTrungBinh = 
            // detailScore.DiemTrungBinh = CalculateAverageScore(
            //     detailScore.DiemMieng, 
            //     detailScore.Diem15p, 
            //     detailScore.DiemGK, 
            //     detailScore.DiemCK);

            StudentDetailScores.Add(detailScore);
        }
        
        Console.WriteLine($"Đã load {StudentDetailScores.Count} môn học có điểm");
    }

    
    #endregion

    #region Xuất Excel
    [RelayCommand] 
    private async Task ExportToExcelAsync()
    {
        try
        {
            if (Students.Count == 0)
            {
                await MessageBoxUtil.ShowError("Không có dữ liệu để xuất");
                return;
            }

            // Mở hộp thoại lưu file
            var sfd = new SaveFileDialog
            {
                Title = "Chọn nơi lưu file Excel",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Excel Files", Extensions = { "xlsx" } }
                },
                InitialFileName = $"DanhSachHocSinh_Lop{NameClass}_{SelectedTerm?.Name}.xlsx"
            };

            string? path = await sfd.ShowAsync((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
            if (string.IsNullOrWhiteSpace(path)) return;

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Danh sách học sinh");

                // Tiêu đề
                ws.Cell(1, 1).Value = "TRƯỜNG THCS ABC";
                ws.Range("A1:E1").Merge();
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 16;
                ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(2, 1).Value = $"DANH SÁCH HỌC SINH - LỚP {NameClass}";
                ws.Range("A2:E2").Merge();
                ws.Cell(2, 1).Style.Font.Bold = true;
                ws.Cell(2, 1).Style.Font.FontSize = 14;
                ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Thông tin lớp
                ws.Cell(4, 1).Value = "Giáo viên chủ nhiệm:";
                ws.Cell(4, 2).Value = NameTeacher;
                ws.Cell(5, 1).Value = "Kỳ học:";
                ws.Cell(5, 2).Value = $"{NameTerm} - Năm {Year}";

                // Header bảng
                int startRow = 7;
                ws.Cell(startRow, 1).Value = "STT";
                ws.Cell(startRow, 2).Value = "Họ và tên";
                ws.Cell(startRow, 3).Value = "Điểm các môn học";
                ws.Cell(startRow, 4).Value = "GPA tổng";
                ws.Cell(startRow, 5).Value = "Hạnh kiểm";
                ws.Cell(startRow, 6).Value = "Xếp loại";

                // Ghi dữ liệu
                int row = startRow + 1;
                int stt = 1;
                foreach (var st in Students)
                {
                    ws.Cell(row, 1).Value = stt++;
                    ws.Cell(row, 2).Value = st.StudentName;
                    ws.Cell(row, 3).Value = st.SubjectName;
                    ws.Cell(row, 4).Value = st.GpaTotal;
                    ws.Cell(row, 5).Value = st.ConductLevel;
                    ws.Cell(row, 6).Value = st.Academic;
                    row++;
                }

                // Căn chỉnh
                ws.Columns().AdjustToContents();
                workbook.SaveAs(path);
            }

            await MessageBoxUtil.ShowSuccess("✅ Xuất file Excel thành công!");
        }
        catch (Exception ex)
        {
            await MessageBoxUtil.ShowError($"❌ Xuất file Excel thất bại: {ex.Message}");
        }
    }
    #endregion
    [RelayCommand] 
    private async Task ExportStudentDetailToExcel()
    {
        try
        {
            if (SelectedStudent == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn học sinh để xuất điểm chi tiết");
                return;
            }

            if (StudentDetailScores.Count == 0)
            {
                await MessageBoxUtil.ShowError("Không có dữ liệu điểm chi tiết để xuất");
                return;
            }

            // Mở hộp thoại lưu file
            var sfd = new SaveFileDialog
            {
                Title = "Chọn nơi lưu file Excel điểm chi tiết",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Excel Files", Extensions = { "xlsx" } }
                },
                InitialFileName = $"Diem_Chi_Tiet_{SelectedStudent.StudentName.Replace(" ", "_")}.xlsx"
            };

            string? path = await sfd.ShowAsync((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
            if (string.IsNullOrWhiteSpace(path)) return;

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Điểm chi tiết");

                // 🧩 --- Lấy thông tin lớp ---
                var info = Information.FirstOrDefault();
                string teacher = info?.NameTeacher ?? "Chưa rõ";
                string className = info?.NameClass ?? "Chưa rõ";
                string term = info?.NameTerm ?? "Chưa rõ";
                string year = info?.Year.ToString() ?? "Chưa rõ";

                // 🧾 --- Thiết kế phần tiêu đề ---
                ws.Cell(1, 1).Value = "TRƯỜNG THCS ABC";
                ws.Range("A1:H1").Merge();
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 16;
                ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(2, 1).Value = $"BẢNG ĐIỂM CHI TIẾT - {SelectedStudent.StudentName.ToUpper()}";
                ws.Range("A2:H2").Merge();
                ws.Cell(2, 1).Style.Font.Bold = true;
                ws.Cell(2, 1).Style.Font.FontSize = 14;
                ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(3, 1).Value = $"Lớp: {className} - Giáo viên: {teacher} - Học kỳ: {term} - Năm học: {year}";
                ws.Range("A3:H3").Merge();
                ws.Cell(3, 1).Style.Font.Italic = true;
                ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // 📊 --- Bảng điểm chi tiết ---
                int startRow = 5;
                
                // Header
                ws.Cell(startRow, 1).Value = "Môn học";
                ws.Cell(startRow, 2).Value = "Điểm miệng";
                ws.Cell(startRow, 3).Value = "Điểm 15 phút";
                ws.Cell(startRow, 4).Value = "Điểm giữa kỳ";
                ws.Cell(startRow, 5).Value = "Điểm cuối kỳ";
                ws.Cell(startRow, 6).Value = "Điểm trung bình";
                ws.Cell(startRow, 7).Value = "Xếp loại";

                // --- Header style ---
                var headerRange = ws.Range(startRow, 1, startRow, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // --- Ghi dữ liệu điểm chi tiết ---
                int row = startRow + 1;
                float totalGPA = 0;
                int subjectCount = 0;

                foreach (var subject in StudentDetailScores)
                {
                    ws.Cell(row, 1).Value = subject.NameSubject;
                    
                    // Điểm miệng (chuỗi các điểm)
                    ws.Cell(row, 2).Value = subject.DiemMieng.Count > 0 
                        ? string.Join(", ", subject.DiemMieng) 
                        : "Chưa có điểm";
                    
                    // Điểm 15 phút (chuỗi các điểm)
                    ws.Cell(row, 3).Value = subject.Diem15p.Count > 0 
                        ? string.Join(", ", subject.Diem15p) 
                        : "Chưa có điểm";
                    
                    ws.Cell(row, 4).Value = subject.DiemGK > 0 ? subject.DiemGK : "Chưa có điểm";
                    ws.Cell(row, 5).Value = subject.DiemCK > 0 ? subject.DiemCK : "Chưa có điểm";
                    ws.Cell(row, 6).Value = Math.Round(subject.DiemTrungBinh, 2);
                    
                    // Xếp loại môn học
                    string xepLoai = GetAcademicRanking(subject.DiemTrungBinh);
                    ws.Cell(row, 7).Value = xepLoai;

                    // Tính tổng GPA
                    totalGPA += subject.DiemTrungBinh;
                    subjectCount++;

                    // Viền từng dòng
                    var dataRange = ws.Range(row, 1, row, 7);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    row++;
                }

                // --- Dòng tổng kết ---
                if (subjectCount > 0)
                {
                    float averageGPA = totalGPA / subjectCount;
                    string overallRanking = GetAcademicRanking(averageGPA);

                    ws.Cell(row, 1).Value = "TỔNG KẾT";
                    ws.Cell(row, 6).Value = Math.Round(averageGPA, 2);
                    ws.Cell(row, 7).Value = overallRanking;

                    // Style cho dòng tổng kết
                    var totalRange = ws.Range(row, 1, row, 7);
                    totalRange.Style.Font.Bold = true;
                    totalRange.Style.Fill.BackgroundColor = XLColor.LightYellow;
                    totalRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    totalRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

            
                // --- Căn chỉnh cột ---
                ws.Columns().AdjustToContents();
                ws.Column(1).Width = 20; // Môn học
                ws.Column(2).Width = 15; // Điểm miệng
                ws.Column(3).Width = 15; // Điểm 15p
                ws.Column(4).Width = 15; // Điểm GK
                ws.Column(5).Width = 15; // Điểm CK

                // Căn giữa các cột điểm
                for (int col = 2; col <= 7; col++)
                {
                    ws.Column(col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                workbook.SaveAs(path);
            }

            await MessageBoxUtil.ShowSuccess($"✅ Xuất điểm chi tiết của {SelectedStudent.StudentName} thành công!", null);
            Console.WriteLine($"✅ Xuất điểm chi tiết thành công cho học sinh: {SelectedStudent.StudentName}");
        }
        catch (Exception ex)
        {
            await MessageBoxUtil.ShowError($"❌ Xuất file Excel thất bại: {ex.Message}", null);
            Console.WriteLine($"❌ Lỗi khi xuất Excel điểm chi tiết: {ex.Message}");
        }
    }
    private string GetAcademicRanking(float score)
    {
        if (score >= 8.0f) return "Giỏi";
        if (score >= 6.5f) return "Khá";
        if (score >= 5.0f) return "Trung bình";
        return "Yếu";
    }
    #region Quản lý hạnh kiểm
    // [RelayCommand]
    // private async Task AddConduct()
    // {
    //     if (SelectedStudent == null)
    //     {
    //         await MessageBoxUtil.ShowError("Vui lòng chọn học sinh để cập nhật hạnh kiểm");
    //         return;
    //     }

    //     try
    //     {
    //         SelectedConductLevel = SelectedStudent.ConductLevel ?? "Trung bình";
            
    //         var window = new HomeClassAddDialog
    //         {
    //             DataContext = this,
    //         };

    //         await window.ShowDialog((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Lỗi khi mở dialog hạnh kiểm: {ex.Message}");
    //         await MessageBoxUtil.ShowError($"Lỗi khi mở dialog: {ex.Message}");
    //     }
    // }

    public event EventHandler? RequestAddConduct;
    [RelayCommand]
    private async Task AddConduct()
    {
        if (SelectedStudent == null)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn học sinh để cập nhật hạnh kiểm");
            return;
        }

        try
        {
            SelectedConductLevel = SelectedStudent.ConductLevel ?? "Trung bình";

            // ❌ KHÔNG mở dialog trong ViewModel
            // Thay bằng gửi tín hiệu
            RequestAddConduct?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi mở dialog hạnh kiểm: {ex.Message}");
            await MessageBoxUtil.ShowError($"Lỗi khi mở dialog: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SaveConduct()
    {
        try
        {
            if (SelectedStudent == null || string.IsNullOrEmpty(SelectedConductLevel))
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn học sinh và hạnh kiểm");
                return;
            }

            bool isSuccess = _service.Update(SelectedStudent.StudentId, SelectedConductLevel);

            if (isSuccess)
            {
                await MessageBoxUtil.ShowSuccess($"Cập nhật hạnh kiểm thành công: {SelectedConductLevel}");
                
                // Refresh dữ liệu
                LoadDataByTermCommand.Execute(null);
                
                // Đóng dialog
                RequestCancelConduct?.Invoke(this, EventArgs.Empty);
                // (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                //     .MainWindow?.OwnedWindows
                //     .OfType<HomeClassAddDialog>()
                //     .FirstOrDefault()?
                //     .Close(true);
            }
            else
            {
                await MessageBoxUtil.ShowError("Cập nhật hạnh kiểm thất bại");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lưu hạnh kiểm: {ex.Message}");
            await MessageBoxUtil.ShowError($"Lỗi khi lưu: {ex.Message}");
        }
    }

    public event EventHandler? RequestCancelConduct;
    [RelayCommand]
    private void CancelConduct()
    {
        RequestCancelConduct?.Invoke(this, EventArgs.Empty);
        // (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
        //     .MainWindow?.OwnedWindows
        //     .OfType<HomeClassAddDialog>()
        //     .FirstOrDefault()?
        //     .Close(false);
    }

    #endregion
}