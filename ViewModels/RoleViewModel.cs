using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using ClosedXML.Excel;
using ReactiveUI;
using Utils;

namespace ViewModels;

public partial class RoleViewModel : ViewModelBase
{
    public string TitlePage { get; } = "Quản lý nhóm quyền";
    public string DescriptionPage { get; } = "Quản lý thông tin nhóm quyền";

    // Danh sách nhóm quyền
    public ObservableCollection<RoleModel> Roles { get; }
    // Danh sách nhóm quyền (dùng để hiển thị)
    public ObservableCollection<RoleModel> FilteredRoles { get; }

    // Các biến lọc dữ liệu
    public string FilterFind { get; set; } = "";
    public string FilterStatus { get; set; } = "";
    // Hàm lọc dữ liệu
    public void ApplyFilter()
    {
        if (Roles == null || Roles.Count == 0)
            return;

        var filtered = Roles.Where(role =>
        {
            bool matchKeyword =
                string.IsNullOrWhiteSpace(FilterFind) ||
                role.Id.ToString().Contains(FilterFind) ||
                role.Name.Contains(FilterFind, StringComparison.OrdinalIgnoreCase);

            bool matchStatus =
                string.IsNullOrWhiteSpace(FilterStatus) ||
                FilterStatus == "Chọn Trạng thái" ||
                role.Status.Equals(FilterStatus, StringComparison.OrdinalIgnoreCase);

            return matchKeyword && matchStatus;
        }).ToList();

        // Cập nhật FilteredRoles hiển thị
        FilteredRoles.Clear();
        foreach (var role in filtered)
            FilteredRoles.Add(role);
    }

    // Hàm nhập excel
    public async Task ImportExcel(string filePath)
    {
        try
        {
            var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn nhập danh sách nhóm quyền từ file Excel này?");
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
                        string name = row.Cell(1).GetString()?.Trim() ?? "";
                        string status = row.Cell(2).GetString()?.Trim() ?? "Hoạt động";

                        // check trùng
                        bool exists = this.FilteredRoles.Any(s =>
                            string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));

                        if (exists)
                        {
                            duplicateCount++;
                            continue;
                        }

                        var role = new RoleModel
                        {
                            Name = name,
                            Status = status,
                        };

                        bool isSuccess = await CreateRoleCommand.Execute(role).ToTask();
                        if (isSuccess)
                        {
                            successCount++;
                            int newId = AppService.RoleService.GetIdLastRole();
                            var roleCreated = AppService.RoleService.GetRoleById(newId);
                            Roles.Add(roleCreated);
                            FilteredRoles.Add(roleCreated);
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
            if (Roles == null || Roles.Count == 0)
                throw new InvalidOperationException("Không có dữ liệu để xuất!");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Nhóm quyền");

                // ===== HEADER =====
                string[] headers =
                {
                    "Mã nhóm quyền", "Tên nhóm quyền", "Trạng thái"
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
                foreach (var role in FilteredRoles)
                {
                    worksheet.Cell(row, 1).Value = role.Id;
                    worksheet.Cell(row, 2).Value = role.Name;
                    worksheet.Cell(row, 3).Value = role.Status;

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

    // Commands
    public ReactiveCommand<Unit, ObservableCollection<RoleModel>> GetRolesCommand { get; }
    public ReactiveCommand<RoleModel, bool> CreateRoleCommand { get; }
    public ReactiveCommand<RoleModel, bool> UpdateRoleCommand { get; }
    public ReactiveCommand<RoleModel, bool> LockRoleCommand { get; }

    public ReactiveCommand<RoleModel, ObservableCollection<RoleDetailModel>> GetRoleDetailsCommand { get; }

    public RoleViewModel()
    {
        Roles = new ObservableCollection<RoleModel>();
        FilteredRoles = new ObservableCollection<RoleModel>();

        // Load roles ban đầu
        var roles = AppService.RoleService.GetRoles();
        foreach (var role in roles)
        {
            Roles.Add(role);
            FilteredRoles.Add(role);
        }

        // Commands cơ bản cho Role
        GetRolesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var roles = AppService.RoleService.GetRoles();
            Roles.Clear();
            FilteredRoles.Clear();
            foreach (var role in roles)
            {
                Roles.Add(role);
                FilteredRoles.Add(role);
            }

            return FilteredRoles;
        });

        CreateRoleCommand = ReactiveCommand.CreateFromTask<RoleModel, bool>(async (role) =>
        {
            var result = AppService.RoleService.CreateRole(role);
            if (result > 0 && role.RoleDetails != null && role.RoleDetails.Count > 0)
            {
                int newRoleId = AppService.RoleService.GetIdLastRole();
                foreach (RoleDetailModel roleDetail in role.RoleDetails)
                {
                    AppService.RoleDetailService.Create(new RoleDetailModel(newRoleId, roleDetail.FunctionId, roleDetail.Action));
                }
            }

            return result > 0;
        });

        UpdateRoleCommand = ReactiveCommand.CreateFromTask<RoleModel, bool>(async (role) =>
        {
            var result = AppService.RoleService.UpdateRole(role);
            if (result > 0 && role.RoleDetails != null && role.RoleDetails.Count > 0)
            {
                AppService.RoleDetailService.DeleteAllByRoleId(role.Id);
                foreach (RoleDetailModel roleDetail in role.RoleDetails)
                {
                    AppService.RoleDetailService.Create(new RoleDetailModel(role.Id, roleDetail.FunctionId, roleDetail.Action));
                }
            }

            return result > 0;
        });

        LockRoleCommand = ReactiveCommand.CreateFromTask<RoleModel, bool>(async (role) =>
        {
            var result = AppService.RoleService.LockRole(role);
            if (result > 0) return true;
            return false;
        });
    }
}
