using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using ClosedXML.Excel;
using ReactiveUI;
using Utils;

namespace ViewModels;

public partial class UserViewModel : ViewModelBase
{
    // Tiêu đề trang
    public string TitlePage { get; } = "Quản lý người dùng";
    // Mô tả trang
    public string DescriptionPage { get; } = "Quản lý thông tin người dùng";
    // Danh sách người dùng
    public ObservableCollection<UserModel> Users { get; }
    // Danh sách người dùng (dùng để hiển thị)
    public ObservableCollection<UserModel> FilteredUsers { get; }

    // Command thêm, cập nhật và khoá người dùng
    public ReactiveCommand<string, bool> UserIsExistsByUsernameCommand { get; }
    public ReactiveCommand<Unit, ObservableCollection<UserModel>> GetUsersCommand { get; }
    public ReactiveCommand<UserModel, bool> CreateUserCommand { get; }
    public ReactiveCommand<UserModel, bool> UpdateUserCommand { get; }
    public ReactiveCommand<UserModel, bool> LockUserCommand { get; }
    public ReactiveCommand<UserModel, bool> ChangePasswordUserCommand { get; }

    // Các biến lọc dữ liệu
    public string FilterFind { get; set; } = "";
    public string FilterStatus { get; set; } = "";
    // Hàm lọc dữ liệu
    public void ApplyFilter()
    {
        if (Users == null || Users.Count == 0)
            return;

        var filtered = Users.Where(user =>
        {
            bool matchKeyword =
                string.IsNullOrWhiteSpace(FilterFind) ||
                user.Id.ToString().Contains(FilterFind) ||
                user.Fullname.Contains(FilterFind, StringComparison.OrdinalIgnoreCase) ||
                user.Username.Contains(FilterFind, StringComparison.OrdinalIgnoreCase);

            bool matchStatus =
                string.IsNullOrWhiteSpace(FilterStatus) ||
                FilterStatus == "Chọn Trạng thái" ||
                user.Status.Equals(FilterStatus, StringComparison.OrdinalIgnoreCase);

            return matchKeyword && matchStatus;
        }).ToList();

        // Cập nhật FilteredUsers hiển thị
        FilteredUsers.Clear();
        foreach (var role in filtered)
            FilteredUsers.Add(role);
    }

    // Hàm nhập excel
    public async Task ImportExcel(string filePath)
    {
        try
        {
            var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn nhập danh sách người dùng từ file Excel này?");
            if (!confirm)
                return;

            int successCount = 0;
            int failCount = 0;
            int duplicateCount = 0;

            using (var workbook = new XLWorkbook(filePath))
            {
                var ws = workbook.Worksheet(1);
                var rows = ws.RowsUsed().Skip(0);

                foreach (var row in rows)
                {
                    try
                    {
                        // đọc dữ liệu
                        string avatar = row.Cell(1).GetString() ?? "";
                        string username = row.Cell(2).GetString()?.Trim() ?? "";
                        string password = row.Cell(3).GetString()?.Trim() ?? "cschool";
                        int roleId = int.Parse(row.Cell(4).GetString() ?? "0");
                        string fullname = row.Cell(5).GetString()?.Trim() ?? "";
                        string phone = row.Cell(6).GetString()?.Trim() ?? "";
                        string email = row.Cell(7).GetString()?.Trim() ?? "";
                        string address = row.Cell(8).GetString()?.Trim() ?? "";
                        string status = row.Cell(9).GetString()?.Trim() ?? "Hoạt động";

                        // check trùng
                        bool exists = this.FilteredUsers.Any(s =>
                            string.Equals(s.Username, username, StringComparison.OrdinalIgnoreCase));

                        if (exists)
                        {
                            duplicateCount++;
                            continue;
                        }

                        var user = new UserModel
                        {
                            Avatar = string.IsNullOrWhiteSpace(avatar) ? null : avatar,
                            Username = username,
                            Password = password,
                            RoleId = roleId,
                            Fullname = fullname,
                            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                            Email = string.IsNullOrWhiteSpace(email) ? null : email,
                            Address = string.IsNullOrWhiteSpace(address) ? null : address,
                            Status = status,
                        };

                        bool isSuccess = await CreateUserCommand.Execute(user).ToTask();
                        if (isSuccess)
                        {
                            successCount++;
                            int newId = AppService.UserService.GetIdLastUser();
                            var userCreated = AppService.UserService.GetOneUserById(newId);
                            Users.Add(userCreated);
                            FilteredUsers.Add(userCreated);
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
    // Hàm xuất excel
    public async Task ExportExcel(string filePath)
    {
        try
        {
            if (Users == null || Users.Count == 0)
                throw new InvalidOperationException("Không có dữ liệu để xuất!");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Người dùng");

                // ===== HEADER =====
                string[] headers =
                {
                        "Mã người dùng", "Hình ảnh", "Tên tài khoản", "Mật khẩu", "Nhóm quyền",
                        "Họ và tên", "Số điện thoại", "Email", "Địa chỉ", "Trạng thái"
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
                foreach (var user in FilteredUsers)
                {
                    worksheet.Cell(row, 1).Value = user.Id;
                    worksheet.Cell(row, 2).Value = user.Avatar;
                    worksheet.Cell(row, 3).Value = user.Username;
                    worksheet.Cell(row, 4).Value = user.Password;
                    worksheet.Cell(row, 5).Value = user.RoleName;
                    worksheet.Cell(row, 6).Value = user.Fullname;
                    worksheet.Cell(row, 7).Value = user.Phone;
                    worksheet.Cell(row, 8).Value = user.Email;
                    worksheet.Cell(row, 9).Value = user.Address;
                    worksheet.Cell(row, 10).Value = user.Status;

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

    public UserViewModel()
    {
        Users = new ObservableCollection<UserModel>();
        FilteredUsers = new ObservableCollection<UserModel>();
        var users = AppService.UserService.GetUsers();
        foreach (var user in users)
        {
            Users.Add(user);
            FilteredUsers.Add(user);
        }

        UserIsExistsByUsernameCommand = ReactiveCommand.CreateFromTask<string, bool>(async (username) =>
        {
            return AppService.UserService.UserIsExistsByUsername(username);
        });
        GetUsersCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var users = AppService.UserService.GetUsers();
            Users.Clear();
            FilteredUsers.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
                FilteredUsers.Add(user);
            }

            return FilteredUsers;
        });
        CreateUserCommand = ReactiveCommand.CreateFromTask<UserModel, bool>(async (user) =>
        {
            string newAvatarFile = await UploadService.SaveImageAsync(user.AvatarFile, "user", AppService.UserService.GetIdLastUser() + 1);
            user.Avatar = newAvatarFile;

            var result = AppService.UserService.CreateUser(user);
            return result > 0;
        });
        UpdateUserCommand = ReactiveCommand.CreateFromTask<UserModel, bool>(async (user) =>
        {
            if (user.AvatarFile != null)
            {
                string newAvatarFile = await UploadService.SaveImageAsync(user.AvatarFile, "user", user.Id);
                user.Avatar = newAvatarFile;
            }
            else
            {
                user.Avatar = AppService.UserService.GetOneUserById(user.Id).Avatar;
            }

            var result = AppService.UserService.UpdateUser(user);
            return result > 0;
        });
        LockUserCommand = ReactiveCommand.CreateFromTask<UserModel, bool>(async (user) =>
        {
            var result = AppService.UserService.LockUser(user);
            return result > 0;
        });
        ChangePasswordUserCommand = ReactiveCommand.CreateFromTask<UserModel, bool>(async (user) =>
        {
            var result = AppService.UserService.ChangePasswordUser(user);
            return result > 0;
        });
    }
}