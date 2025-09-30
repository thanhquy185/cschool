using System;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace cschool.ViewModels;

public partial class UserViewModel : ViewModelBase
{
    // Tiêu đề trang
    public string TitlePage { get; } = "Thông tin người dùng";
    // Mô tả trang
    public string DescriptionPage { get; } = "Thông tin cơ bản người dùng";
    // Danh sách người dùng
    public ObservableCollection<UserModel> Users { get; }
    // Command thêm, cập nhật và khoá người dùng
    public ReactiveCommand<string, bool> UserIsExistsByUsernameCommand { get; }
    public ReactiveCommand<Unit, ObservableCollection<UserModel>> GetUsersCommand { get; }
    public ReactiveCommand<UserModel, bool> CreateUserCommand { get; }
    public ReactiveCommand<UserModel, bool> UpdateUserCommand { get; }
    public ReactiveCommand<UserModel, bool> LockUserCommand { get; }

    public UserViewModel()
    {
        Users = new ObservableCollection<UserModel>();
        var users = AppService.UserService.GetUsers();
        foreach (var user in users) Users.Add(user);

        UserIsExistsByUsernameCommand = ReactiveCommand.CreateFromTask<string, bool>(async (username) =>
        {
            return AppService.UserService.UserIsExistsByUsername(username);
        });
        GetUsersCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var users = AppService.UserService.GetUsers();
            Users.Clear();
            foreach (var user in users) Users.Add(user);

            return Users;
        });
        CreateUserCommand = ReactiveCommand.CreateFromTask<UserModel, bool>(async (user) =>
        {
            string newAvatarFile = await UploadService.SaveImageAsync(user.AvatarFile, "user", AppService.UserService.GetIdLastUser() + 1);
            user.Avatar = newAvatarFile;

            var result = AppService.UserService.CreateUser(user);
            if (result > 0) return true;
            return false;
        });
        UpdateUserCommand = ReactiveCommand.CreateFromTask<UserModel, bool>(async (user) =>
        {
            string newAvatarFile = await UploadService.SaveImageAsync(user.AvatarFile, "user", user.Id);
            user.Avatar = newAvatarFile;

            var result = AppService.UserService.UpdateUser(user);
            if (result > 0) return true;
            return false;
        });
        LockUserCommand = ReactiveCommand.CreateFromTask<UserModel, bool>(async (user) =>
        {
            var result = AppService.UserService.LockUser(user);
            if (result > 0) return true;
            return false;
        });
    }
}