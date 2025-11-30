using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using cschool.Models;
using cschool.Services;
using System;
using ClosedXML.Excel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using cschool.Utils;
using System.Linq;
using Avalonia.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using cschool.ViewModels;
using cschool.Views.DialogHomeClass;
using cschool.Views;

namespace cschool.ViewModels;
public partial class HomeClassViewModel : ViewModelBase
{
    private readonly HomeClassService _service;

    [ObservableProperty] public string nameTeacher = "";
    [ObservableProperty] public string nameClass = "";
    [ObservableProperty] public string nameTerm = "";
    [ObservableProperty] public string year = "";
    [ObservableProperty] public String? _searchName ;
    public ObservableCollection<HomeClass> Students { get; } = new();
    public ObservableCollection<Information> Information { get; } = new();
    [ObservableProperty]
    private ObservableCollection<DetailScore> studentDetailScores = new();

    [ObservableProperty]
    private HomeClass? selectedStudent;


    [ObservableProperty]
    private string selectedStudentName = "";

    #region load dữ liệu
    [RelayCommand]
    private void LoadData()
    {
        try
        {

            var students = _service.GetStudents(12);
            var information = _service.GetInformation(12);
            Students.Clear();
            Information.Clear();
            foreach (var a in students)
            {
                Students.Add(a);
            }
            foreach (var i in information)
            {
                Information.Add(i);
            }
            var info = information.FirstOrDefault();
            if (info != null)
            {
                NameTeacher = info.NameTeacher;
                NameClass = info.NameClass;
                NameTerm = info.NameTerm;
                Year = info.Year.ToString();
            }
            Console.WriteLine("Load dữ liệu thành công");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading students: {ex.Message}");
        }
    }
    #endregion

    [RelayCommand]
    public void Search()
    {
        var results = _service.Search(12, SearchName ?? "");
        Dispatcher.UIThread.Post(() =>
        {
            Students.Clear();
            foreach (var a in results)
                Students.Add(a);
        });
    }


    partial void OnSearchNameChanged(string value)
    {
        Search();
    }

    [RelayCommand]
    public void ResetSearch()
    {
        SearchName = string.Empty;
        LoadDataCommand.Execute(null); // 🔁 Hiển thị lại toàn bộ danh sách
    }
[RelayCommand]
private async Task ShowStudentDetail()
{
    if (SelectedStudent == null)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn 1 đối tượng để xem");
            return;
        } 
    try
    {
        SelectedStudentName = SelectedStudent.StudentName;
        LoadStudentDetailScores(SelectedStudent.StudentId);
        Console.WriteLine($"=== DEBUG BEFORE DIALOG ===");
        Console.WriteLine($"SelectedStudentName: {SelectedStudentName}");
        Console.WriteLine($"StudentDetailScores Count: {StudentDetailScores.Count}");
        foreach (var score in StudentDetailScores)
        {
            Console.WriteLine($"  - {score.NameSubject}: Miệng={score.DiemMieng}, 15p={score.Diem15p}, GK={score.DiemGK}, CK={score.DiemCK}");
        }
        
        // Tạo và hiển thị dialog riêng
        var dialog = new HomeClassDetailDialog
        {
            DataContext = this // Sử dụng cùng ViewModel
        };
        
        await dialog.ShowDialog((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
        
        // Clear data sau khi đóng dialog
        // StudentDetailScores.Clear();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi khi tải chi tiết điểm: {ex.Message}");
        await MessageBoxUtil.ShowError($"Lỗi khi tải chi tiết điểm: {ex.Message}", null);
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

    if (diemMieng == null || diem15p == null || diemGK == null || diemCK == null)
    {
        Console.WriteLine("Không có dữ liệu");
        return;
    }
    
    Console.WriteLine("Load dữ liệu chi tiết điểm thành công");

    // Gom điểm theo môn học
    var allSubjects = diemMieng.Select(d => d.NameSubject)
                             .Union(diem15p.Select(d => d.NameSubject))
                             .Union(diemGK.Select(d => d.NameSubject))
                             .Union(diemCK.Select(d => d.NameSubject))
                             .Distinct();


    foreach (var subject in allSubjects)
    {
        var detailScore = new DetailScore
        {
            NameSubject = subject,
            DiemMieng = diemMieng.FirstOrDefault(d => d.NameSubject == subject)?.DiemMieng ?? new List<float>(),
            Diem15p = diem15p.FirstOrDefault(d => d.NameSubject == subject)?.Diem15p ?? new List<float>(),
            DiemGK = diemGK.FirstOrDefault(d => d.NameSubject == subject)?.DiemGK ?? 0,
            DiemCK = diemCK.FirstOrDefault(d => d.NameSubject == subject)?.DiemCK ?? 0
        };

        detailScore.DiemTrungBinh = CalculateAverageScore(
            detailScore.DiemMieng, 
            detailScore.Diem15p, 
            detailScore.DiemGK, 
            detailScore.DiemCK);

        StudentDetailScores.Add(detailScore);
        
        // Debug log
        Console.WriteLine($"Môn: {subject}");
        Console.WriteLine($"  - Điểm miệng: {string.Join(", ", detailScore.DiemMieng)}");
        Console.WriteLine($"  - Điểm 15p: {string.Join(", ", detailScore.Diem15p)}");
        Console.WriteLine($"  - Điểm GK: {detailScore.DiemGK}");
        Console.WriteLine($"  - Điểm CK: {detailScore.DiemCK}");
        Console.WriteLine($"  - Điểm TB: {detailScore.DiemTrungBinh}");
    }
}
private float CalculateAverageScore(List<float> diemMieng, List<float> diem15p, float diemGK, float diemCK)
{
    // Tính tổng điểm miệng (nếu có nhiều điểm)
    float tongMieng = diemMieng.Count > 0 ? diemMieng.Sum() : 0;
    
    // Tính tổng điểm 15p (nếu có nhiều điểm)
    float tong15p = diem15p.Count > 0 ? diem15p.Sum() : 0;
    int soBaiMieng = diemMieng.Count;
    int soBai15P = diem15p.Count; 

    return (tongMieng * 1 + tong15p * 1 + diemGK * 2 + diemCK * 3) / (5+soBai15P+soBaiMieng);
}

[RelayCommand] 
private async Task ExportToExcelAsync()
{
    try
    {
        if (Students.Count == 0)
        {
            await MessageBoxUtil.ShowError("Không có dữ liệu để xuất");
            Console.WriteLine("⚠️ Không có dữ liệu để xuất.");
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
            InitialFileName = "DanhSachHocSinh.xlsx"
        };

        string? path = await sfd.ShowAsync((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
        if (string.IsNullOrWhiteSpace(path)) return;

        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.Worksheets.Add("Danh sách học sinh");

            // 🧩 --- Lấy thông tin lớp ---
            var info = Information.FirstOrDefault();
            string teacher = info?.NameTeacher ?? "Chưa rõ";
            string className = info?.NameClass ?? "Chưa rõ";
            string term = info?.NameTerm ?? "Chưa rõ";
            string year = info?.Year.ToString() ?? "Chưa rõ";

            // 🧾 --- Thiết kế phần tiêu đề ---
            ws.Cell(1, 1).Value = "TRƯỜNG THCS ABC";
            ws.Range("A1:E1").Merge();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(2, 1).Value = $"DANH SÁCH HỌC SINH - LỚP {className}";
            ws.Range("A2:E2").Merge();
            ws.Cell(2, 1).Style.Font.Bold = true;
            ws.Cell(2, 1).Style.Font.FontSize = 14;
            ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // 🌸 --- Thông tin lớp học ---
            ws.Cell(4, 1).Value = "Giáo viên chủ nhiệm:";
            ws.Cell(4, 2).Value = teacher;

            ws.Cell(5, 1).Value = "Kỳ học:";
            ws.Cell(5, 2).Value = term;

            ws.Cell(6, 1).Value = "Năm học:";
            ws.Cell(6, 2).Value = year;

            // --- Kẻ khung cho phần thông tin ---
            var infoRange = ws.Range("A4:B6");
            infoRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            infoRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // 📋 --- Bảng danh sách học sinh ---
            int startRow = 8;
            ws.Cell(startRow, 1).Value = "STT";
            ws.Cell(startRow, 2).Value = "Họ và tên";
            ws.Cell(startRow, 3).Value = "Điểm các môn học";
            ws.Cell(startRow, 4).Value = "GPA tổng";
            ws.Cell(startRow, 5).Value = "Hạnh kiểm";
            ws.Cell(startRow, 6).Value = "Xếp loại";

            // --- Header style ---
            var headerRange = ws.Range(startRow, 1, startRow, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // --- Ghi dữ liệu ---
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

                // Viền từng dòng
                var dataRange = ws.Range(row, 1, row, 6);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                row++;
            }

            // --- Căn chỉnh cột ---
            ws.Columns().AdjustToContents();
            ws.Column(3).Width = 40; // cột "Điểm các môn học" rộng hơn

            // --- Chữ ký cuối trang ---
            ws.Cell(row + 2, 5).Value = "Giáo viên chủ nhiệm";
            ws.Cell(row + 2, 5).Style.Font.Bold = true;
            ws.Cell(row + 2, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            workbook.SaveAs(path);
        }

        await MessageBoxUtil.ShowSuccess("✅ Xuất file Excel thành công!", null);
        Console.WriteLine("✅ Xuất file Excel thành công");
    }
    catch (Exception ex)
    {
        await MessageBoxUtil.ShowError("❌ Xuất file Excel thất bại.", null);
        Console.WriteLine($"❌ Lỗi khi xuất Excel: {ex.Message}");
    }
}

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

// Phương thức xác định xếp loại học lực
private string GetAcademicRanking(float score)
{
    if (score >= 8.0f) return "Giỏi";
    if (score >= 6.5f) return "Khá";
    if (score >= 5.0f) return "Trung bình";
    return "Yếu";
}

    public HomeClassViewModel(HomeClassService service)
    {
        _service = service;
        LoadDataCommand.Execute(null);
    }
}
