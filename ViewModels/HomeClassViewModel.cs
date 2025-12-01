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
            Console.WriteLine($" Error loading students: {ex.Message}");
        }
    }

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



private void LoadStudentDetailScores(int studentId)
{
    StudentDetailScores.Clear();

    // Lấy tất cả các loại điểm từ 4 hàm của bạn
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
         Console.WriteLine($"  - Điểm miệng: {diemMieng.Count} bản ghi");
    Console.WriteLine($"  - Điểm 15p: {diem15p.Count} bản ghi");
    Console.WriteLine($"  - Điểm GK: {diemGK.Count} bản ghi");
    Console.WriteLine($"  - Điểm CK: {diemCK.Count} bản ghi");
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
            DiemMieng = diemMieng.FirstOrDefault(d => d.NameSubject == subject)?.DiemMieng ?? 0,
            Diem15p = diem15p.FirstOrDefault(d => d.NameSubject == subject)?.Diem15p ?? 0,
            DiemGK = diemGK.FirstOrDefault(d => d.NameSubject == subject)?.DiemGK ?? 0,
            DiemCK = diemCK.FirstOrDefault(d => d.NameSubject == subject)?.DiemCK ?? 0
        };

        StudentDetailScores.Add(detailScore);
    }
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

            //  --- Bảng danh sách học sinh ---
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

    public HomeClassViewModel(HomeClassService service)
    {
        _service = service;
        LoadDataCommand.Execute(null);
    }
}
