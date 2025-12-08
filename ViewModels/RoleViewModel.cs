using System.Collections.ObjectModel;
using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using Services;

namespace ViewModels;

public partial class RoleViewModel : ViewModelBase
{
    public string TitlePage { get; } = "Quản lý nhóm quyền";
    public string DescriptionPage { get; } = "Quản lý thông tin nhóm quyền";

    // Danh sách nhóm quyền
    public ObservableCollection<RoleModel> Roles { get; }

    // // Role hiện tại được chọn
    // [ObservableProperty]
    // private RoleModel? _selectedRole;

    // Danh sách chức năng có thể gán
    public ObservableCollection<FunctionModel> Functions { get; }

    // Danh sách quyền của role được chọn
    public ObservableCollection<RoleDetailModel> RoleDetails { get; }

    // Commands
    public ReactiveCommand<Unit, ObservableCollection<RoleModel>> GetRolesCommand { get; }
    public ReactiveCommand<RoleModel, bool> CreateRoleCommand { get; }
    public ReactiveCommand<RoleModel, bool> UpdateRoleCommand { get; }
    public ReactiveCommand<RoleModel, bool> LockRoleCommand { get; }

    public ReactiveCommand<RoleModel, ObservableCollection<RoleDetailModel>> GetRoleDetailsCommand { get; }

    public RoleViewModel()
    {
        Roles = new ObservableCollection<RoleModel>();
        Functions = new ObservableCollection<FunctionModel>();
        RoleDetails = new ObservableCollection<RoleDetailModel>();

        // Load roles ban đầu
        var roles = AppService.RoleService.GetRoles();
        foreach (var role in roles) Roles.Add(role);

        // Commands cơ bản cho Role
        GetRolesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var roles = AppService.RoleService.GetRoles();
            Roles.Clear();
            foreach (var role in roles) Roles.Add(role);
            return Roles;
        });

        CreateRoleCommand = ReactiveCommand.CreateFromTask<RoleModel, bool>(async (role) =>
        {
            var result = AppService.RoleService.CreateRole(role);
            return result > 0;
        });

        UpdateRoleCommand = ReactiveCommand.CreateFromTask<RoleModel, bool>(async (role) =>
        {
            var result = AppService.RoleService.UpdateRole(role);
            return result > 0;
        });

        LockRoleCommand = ReactiveCommand.CreateFromTask<RoleModel, bool>(async (role) =>
        {
            var result = AppService.RoleService.LockRole(role);
            if (result > 0) return true;
            return false;
        });

        // Command load quyền của role
        GetRoleDetailsCommand = ReactiveCommand.CreateFromTask<RoleModel, ObservableCollection<RoleDetailModel>>(async (role) =>
        {
            RoleDetails.Clear();
            var details = AppService.RoleDetailService.GetRoleDetailsByRoleId(role.Id);
            foreach (var rd in details) RoleDetails.Add(rd);
            return RoleDetails;
        });

        // Load tất cả chức năng hệ thống
        var funcs = AppService.FunctionService.GetFunctions();
        foreach (var f in funcs) Functions.Add(f);
    }
}
