using Avalonia.Controls;
using cschool.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using cschool.Models;
using ClosedXML.Excel;
using cschool.Utils;
using cschool.Views.Teacher;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using System;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;

namespace cschool.Views;

public partial class TeacherView : UserControl
{
    public TeacherView()
    {
        InitializeComponent();
        DataContext = new TeacherViewModel();

        InfoButton.Click += async (_, _) => await ShowTeacherDialog(DialogModeEnum.Info);
        CreateButton.Click += async (_, _) => await ShowTeacherDialog(DialogModeEnum.Create);
        UpdateButton.Click += async (_, _) => await ShowTeacherDialog(DialogModeEnum.Update);
        LockButton.Click += async (_, _) => await ShowTeacherDialog(DialogModeEnum.Lock);
        ImportExcelButton.Click += async (_, _) => await ImportExcel();
        ExportExcelButton.Click += async (_, _) => await ExportExcel();
    }

    private async Task ShowTeacherDialog(DialogModeEnum mode)
    {
        var vm = DataContext as TeacherViewModel;
        
        var selectedTeacher = TeachersDataGrid.SelectedItem as TeacherModel;

        if (selectedTeacher == null && mode != DialogModeEnum.Create)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn người dùng để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                if (vm != null && selectedTeacher != null)
                {
                    var lastTerm = AppService.TeacherService.GetLatestTerm();
                    int termId = lastTerm?.Id ?? 0;
                    var fullTeacher = AppService.TeacherService.GetTeacherById(selectedTeacher.Id, termId);
                    Console.WriteLine("term: {0}", lastTerm);
                    fullTeacher.TermName = lastTerm?.DisplayTextTermYear ?? "";

                    vm.TeacherDetails = fullTeacher ?? selectedTeacher;
                    dialog = new TeacherInfoDialog(vm);
                }
                break;

            case DialogModeEnum.Create:
                vm.ClearTeacherDetails();
                dialog = new TeacherCreateDialog(vm);
                break;
            

            case DialogModeEnum.Update:
                if (vm != null && selectedTeacher != null)
                {
                    var lastTerm = AppService.TeacherService.GetLatestTerm();

                    if (lastTerm == null)
                    {
                        await MessageBoxUtil.ShowError(
                            "Chưa có học kỳ nào! Vui lòng tạo học kỳ trước."
                        );
                        return;
                    }

                    int termId = lastTerm?.Id ?? 0;
                    var teacherToShow = AppService.TeacherService.GetTeacherById(selectedTeacher.Id, termId);
                    teacherToShow.TermName = lastTerm?.DisplayTextTermYear ?? "";
                    vm.SetTeacherForEdit(teacherToShow);

                    vm.TeacherDetails = teacherToShow ?? selectedTeacher;
                }
                dialog = new TeacherUpdateDialog(vm);
                break;

            case DialogModeEnum.Lock:
                if (vm != null && selectedTeacher != null)
                {
                    dialog = new TeacherLockDialog(vm, selectedTeacher);
                }
                break;
        }
        if (dialog == null)
        {
            await MessageBoxUtil.ShowError("Không thể mở dialog. Vui lòng thử lại!");
            return;
        }


        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

    private async Task ImportExcel()
    {
        try
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Chọn file Excel để nhập",
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Excel Files", Extensions = { "xlsx" } }
                }
            };

            var owner = TopLevel.GetTopLevel(this) as Window;
            var result = await openDialog.ShowAsync(owner);
            if (result == null || result.Length == 0)
                return;

            var filePath = result[0];
            if (!File.Exists(filePath))
                return;

            var confirm = await MessageBoxUtil.ShowConfirm(
                "Bạn có chắc chắn muốn nhập danh sách giáo viên từ file Excel này?",
                "Xác nhận",
                owner
            );
            if (!confirm)
                return;

            var vm = DataContext as TeacherViewModel;
            if (vm == null)
            {
                await MessageBoxUtil.ShowError("Không tìm thấy ViewModel.", owner: owner);
                return;
            }

            int successCount = 0;
            int failCount = 0;
            int duplicateCount = 0;

            using (var workbook = new XLWorkbook(filePath))
            {
                var ws = workbook.Worksheet(1);
                var rows = ws.RowsUsed().Skip(1); // bỏ header

                foreach (var row in rows)
                {
                    try
                    {
                        // đọc các ô an toàn
                        string fullname = row.Cell(1).GetString()?.Trim() ?? "";
                        string avatar = row.Cell(2).GetString();
                        
                        // Xử lý birthday - MySQL cần format yyyy-MM-dd
                        string birthday = "";
                        if (row.Cell(3).TryGetValue(out DateTime dt))
                        {
                            // Nếu cell là DateTime, convert trực tiếp
                            birthday = dt.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            // Nếu là chuỗi, thử parse với các format phổ biến
                            string birthdayRaw = row.Cell(3).GetString()?.Trim() ?? "";
                            DateTime parsed;
                            
                            // Thử parse với format dd/MM/yyyy
                            if (DateTime.TryParseExact(birthdayRaw, "dd/MM/yyyy", 
                                System.Globalization.CultureInfo.InvariantCulture, 
                                System.Globalization.DateTimeStyles.None, out parsed))
                            {
                                birthday = parsed.ToString("yyyy-MM-dd");
                            }
                            else if (DateTime.TryParse(birthdayRaw, out parsed))
                            {
                                birthday = parsed.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                // Nếu không parse được, bỏ qua dòng này
                                failCount++;
                                continue;
                            }
                        }

                        var gender = row.Cell(4).GetString();
                        var departmentName = row.Cell(5).GetString();
                        var className = row.Cell(6).GetString();
                        var phone = row.Cell(7).GetString();
                        var email = row.Cell(8).GetString();
                        var address = row.Cell(9).GetString();
                        var status = row.Cell(10).GetString();

                        // ✅ Tìm DepartmentId từ DepartmentName
                        int departmentId = 0;
                        if (!string.IsNullOrWhiteSpace(departmentName))
                        {
                            var dept = AppService.TeacherService.GetDepartments()
                                .FirstOrDefault(d => d.Name.Equals(departmentName, StringComparison.OrdinalIgnoreCase));
                            departmentId = dept?.Id ?? 0;
                        }

                        // ✅ Tìm ClassId từ ClassName
                        int classId = 0;
                        if (!string.IsNullOrWhiteSpace(className))
                        {
                            var cls = AppService.AssignTeacherService.GetClasses()
                                .FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
                            classId = cls?.Id ?? 0;
                        }

                        // ✅ Kiểm tra trùng trước khi thêm
                        // bool exists = vm.Teachers.Any(s =>
                        //     string.Equals(s.Fullname, fullname, StringComparison.OrdinalIgnoreCase) &&
                        //     string.Equals(s.Gender, gender, StringComparison.OrdinalIgnoreCase) &&
                        //     DateTime.TryParse(s.Birthday, out var bDate) &&
                        //     DateTime.TryParse(birthday, out var bNew) &&
                        //     bDate.Date == bNew.Date);

                        // if (exists)
                        // {
                        //     duplicateCount++;
                        //     continue; // bỏ qua học sinh trùng
                        // }

                        var teacher = new TeacherModel
                        {
                            Name = fullname,
                            Avatar = string.IsNullOrWhiteSpace(avatar) ? null : avatar,
                            Birthday = birthday,
                            Gender = gender,
                            DepartmentId = departmentId,  
                            DepartmentName = departmentName,
                            ClassId = classId,
                            ClassName = className,
                            Address = address,
                            Phone = phone,
                            Email = email,
                            Status = status == "Hoạt động" ? 1 : 0
                        };

                        // Gọi command thêm giáo viên qua vm (không dùng field teacherViewModel)
                        bool isSuccess = await vm.CreateTeacherCommand.Execute(teacher).ToTask();
                        if (isSuccess) successCount++;
                        else failCount++;
                    }
                    catch
                    {
                        failCount++;
                    }
                }
            }

            // reload danh sách từ DB qua vm
            await vm.GetTeachersCommand.Execute().ToTask();

            // show kết quả trên owner (không truyền this)
            await MessageBoxUtil.ShowSuccess(
                $"Nhập Excel hoàn tất!\n" +
                $"Nhập thành công: {successCount} giáo viên\n" +
                $"Trùng thông tin: {duplicateCount} giáo viên\n" +
                $"Lỗi khi nhập: {failCount} giáo viên",
                owner: owner
            );
        }
        catch (Exception ex)
        {
            var owner = TopLevel.GetTopLevel(this) as Window;
            await MessageBoxUtil.ShowError("Lỗi khi nhập Excel: " + ex.Message, owner: owner);
        }
    }
    private async Task ExportExcel()
    {
        try
        {
            var vm = DataContext as TeacherViewModel;
            if (vm == null || vm.Teachers == null || !vm.Teachers.Any())
            {
                await MessageBoxUtil.ShowError("Không có dữ liệu để xuất!");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Chọn nơi lưu file Excel",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Excel Files", Extensions = { "xlsx" } }
                },
                InitialFileName = "DanhSachGiaoVien.xlsx"
            };

            var owner = TopLevel.GetTopLevel(this) as Window;
            var filePath = await dialog.ShowAsync(owner);

            if (string.IsNullOrWhiteSpace(filePath))
                return;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Students");

                // ===== HEADER =====
                string[] headers =
                {
                    "ID", "Họ và tên", "Avatar", "Ngày sinh", "Giới tính",
                    "Bộ môn", "Lớp chủ nhiệm", "Số điện thoại", "Email", "Địa chỉ", "Trạng thái"
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
                foreach (var s in vm.Teachers)
                {
                    worksheet.Cell(row, 1).Value = s.Id;
                    worksheet.Cell(row, 2).Value = s.Name;
                    worksheet.Cell(row, 3).Value = s.Avatar ?? "";
                    worksheet.Cell(row, 4).Value = s.Birthday;
                    worksheet.Cell(row, 5).Value = s.Gender;
                    worksheet.Cell(row, 6).Value = s.DepartmentName;
                    worksheet.Cell(row, 7).Value = s.ClassName;
                    worksheet.Cell(row, 8).Value = s.Phone;
                    worksheet.Cell(row, 9).Value = s.Email;
                    worksheet.Cell(row, 10).Value = s.Address;
                    worksheet.Cell(row, 11).Value = s.Status == 1 ? "Hoạt động" : "Đã khóa";

                    // Viền mỏng quanh từng ô dữ liệu
                    for (int col = 1; col <= headers.Length; col++)
                    {
                        worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    row++;
                }

                // Tự động giãn cột
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            }

            await MessageBoxUtil.ShowSuccess("Xuất file Excel thành công!\n", owner: owner);
        }
        catch (Exception ex)
        {
            await MessageBoxUtil.ShowError("Lỗi khi xuất Excel: " + ex.Message);
        }
    }

    private void OnResetFilters(object? sender, RoutedEventArgs e)
    {
        // Reset SearchBox
        if (SearchBox != null)
            SearchBox.Text = "";

        // Reset ComboBox về item đầu tiên
        if (DepartmentFilterBox != null)
            DepartmentFilterBox.SelectedIndex = 0;

        // Áp dụng filter (sẽ hiển thị tất cả)
        // ApplyFilters();
    }


      private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void OnDepartmentFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        ApplyFilters();
    }

    // ✅ Method gộp chung để áp dụng cả 2 bộ lọc
    private void ApplyFilters()
    {
        if (DataContext is not TeacherViewModel vm) return;

        // Lấy giá trị từ SearchBox
        var searchText = SearchBox?.Text?.Trim().ToLower() ?? "";

        // Lấy bộ môn được chọn
        var selectedDept = DepartmentFilterBox?.SelectedItem as DepartmentModel;
        var departmentName = selectedDept?.Name ?? "";

        // Lấy tất cả giáo viên
        var allTeachers = AppService.TeacherService.GetTeachers();

        // Áp dụng filter
        var filteredTeachers = allTeachers.Where(t =>
        {
            // Filter theo tên (nếu có nhập text)
            bool matchesSearch = string.IsNullOrEmpty(searchText) ||
                                t.Id.ToString().Contains(searchText) ||
                                t.Name.ToLower().Contains(searchText);

            // Filter theo bộ môn (nếu không phải "---- Chọn Bộ môn ----")
            bool matchesDepartment = string.IsNullOrEmpty(departmentName) ||
                                    departmentName == "---- Chọn Bộ môn ----" ||
                                    t.DepartmentName.Equals(departmentName, StringComparison.OrdinalIgnoreCase);

            // Chỉ hiển thị nếu thỏa mãn CẢ 2 điều kiện
            return matchesSearch && matchesDepartment;
        }).ToList();

        // Update UI
        vm.Teachers.Clear();
        foreach (var teacher in filteredTeachers)
        {
            vm.Teachers.Add(teacher);
        }
    }

}