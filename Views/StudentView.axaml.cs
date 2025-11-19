using Avalonia.Controls;
using cschool.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using ClosedXML.Excel;
using cschool.Utils;
using cschool.Views.Student;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using System;
using Avalonia.Interactivity;

namespace cschool.Views;

public partial class StudentView : UserControl
{
    public StudentView()
    {
        InitializeComponent();
        DataContext = new StudentViewModel();

        InfoButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Info);
        CreateButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Create);
        UpdateButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Update);
        LockButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Lock);
        ImportExcelButton.Click += async (_, _) => await ImportExcel();
        ExportExcelButton.Click += async (_, _) => await ExportExcel();
    }

    private async Task ShowStudentDialog(DialogModeEnum mode)
    {
        var vm = DataContext as StudentViewModel;
        var selectedStudent = StudentsDataGrid.SelectedItem as StudentModel;

        if (selectedStudent == null && mode != DialogModeEnum.Create)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn người dùng để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                if (vm != null && selectedStudent != null)
                {
                    vm.GetStudentByIdCommand.Execute(selectedStudent.Id).ToTask();
                }
                dialog = new StudentInfoDialog(vm);
                break;

            case DialogModeEnum.Create:
                dialog = new StudentCreateDialog{studentViewModel = vm};
                break;

            case DialogModeEnum.Update:
                if (vm != null && selectedStudent != null)
                {
                    vm.GetStudentByIdCommand.Execute(selectedStudent.Id).ToTask();
                }
                dialog = new StudentUpdateDialog(vm);
                break;

            case DialogModeEnum.Lock:
                if (vm != null && selectedStudent != null)
                {
                    vm.GetStudentByIdCommand.Execute(selectedStudent.Id).ToTask();
                }
                dialog = new StudentLockDialog(vm);
                break;
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

            var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn nhập danh sách học sinh từ file Excel này?");
            if (!confirm)
                return;

            var vm = DataContext as StudentViewModel;
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
                        string birthdayRaw = row.Cell(3).GetString();
                        string birthday = birthdayRaw;
                        // nếu ô là định dạng date, chuyển về yyyy-MM-dd
                        if (row.Cell(3).TryGetValue(out DateTime dt))
                            birthday = dt.ToString("yyyy-MM-dd");
                        else
                        {
                            // nếu chuỗi có thể parse được, chuẩn hoá
                            if (DateTime.TryParse(birthdayRaw, out var parsed))
                                birthday = parsed.ToString("yyyy-MM-dd");
                        }

                        var gender = row.Cell(4).GetString();
                        var ethnicity = row.Cell(5).GetString();
                        var religion = row.Cell(6).GetString();
                        var address = row.Cell(7).GetString();
                        var phone = row.Cell(8).GetString();
                        var email = row.Cell(9).GetString();
                        var learnYear = row.Cell(10).GetString();
                        var learnStatus = row.Cell(11).GetString();

                        // ✅ Kiểm tra trùng trước khi thêm
                        bool exists = vm.AllStudents.Any(s =>
                            string.Equals(s.Fullname, fullname, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(s.Gender, gender, StringComparison.OrdinalIgnoreCase) &&
                            DateTime.TryParse(s.BirthDay, out var bDate) &&
                            DateTime.TryParse(birthday, out var bNew) &&
                            bDate.Date == bNew.Date);

                        if (exists)
                        {
                            duplicateCount++;
                            continue; // bỏ qua học sinh trùng
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

                        // Gọi command thêm học sinh qua vm (không dùng field studentViewModel)
                        bool isSuccess = await vm.CreateStudentCommand.Execute(student).ToTask();
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
            await vm.GetStudentsCommand.Execute().ToTask();

            // show kết quả trên owner (không truyền this)
            await MessageBoxUtil.ShowSuccess(
                $"Nhập Excel hoàn tất!\n" +
                $"Nhập thành công: {successCount} học sinh\n" +
                $"Trùng thông tin: {duplicateCount} học sinh\n" +
                $"Lỗi khi nhập: {failCount} học sinh",
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
            var vm = DataContext as StudentViewModel;
            if (vm == null || vm.Students == null || !vm.Students.Any())
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
                InitialFileName = "DanhSachHocSinh.xlsx"
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
                foreach (var s in vm.AllStudents)
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

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DataContext is StudentViewModel vm)
        {
            var textBox = sender as TextBox;
            vm.FilterKeyword = textBox?.Text ?? "";
            vm.ApplyFilter();
        }
    }

    private void OnStatusFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is StudentViewModel vm)
        {
            var combo = sender as ComboBox;
            var selectedItem = combo?.SelectedItem as ComboBoxItem;
            var selectedText = selectedItem?.Content?.ToString() ?? "";

            vm.FilterStatus = selectedText == "Chọn Trạng thái" ? "" : selectedText;
            vm.ApplyFilter();
        }
    }

    private void OnResetFilterClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is StudentViewModel vm)
        {
            vm.FilterKeyword = "";
            vm.FilterStatus = "";
            vm.ApplyFilter();

            SearchBox.Text = "";
            StatusFilterBox.SelectedIndex = 0;
        }
    }


}