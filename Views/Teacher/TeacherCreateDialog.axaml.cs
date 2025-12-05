using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Interactivity;
using System.Reactive.Threading.Tasks;
using cschool.ViewModels;
using cschool.Utils;
using System.Linq;
using cschool.Models;
using System.Collections.Generic;

namespace cschool.Views.Teacher
{
    public partial class TeacherCreateDialog : Window
    {
        private TeacherViewModel? _teacherViewModel;
        private string? _selectedAvatarPath;

        public TeacherCreateDialog(TeacherViewModel vm)
        {
            InitializeComponent();
            _teacherViewModel = vm;
            DataContext = vm;
            

            // this.Loaded += OnLoaded;
        }

        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ChooseImage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Lấy StorageProvider (API mới trong Avalonia 11+)
            var storageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storageProvider == null)
                return;

            // Mở hộp thoại chọn file ảnh
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Chọn ảnh đại diện",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Hình ảnh")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg" }
                    }
                }
            });

            // Nếu người dùng chọn ảnh
            if (files != null && files.Count > 0)
            {
                var file = files[0];
                var path = file.Path.LocalPath;

                if (File.Exists(path))
                {
                    try
                    {
                        // Hiển thị ảnh đã chọn
                        _selectedAvatarPath = path;  
                        AvatarImage.Source = new Bitmap(path);
                    }
                    catch
                    {
                        // Báo lỗi nếu không thể load ảnh
                        await MessageBoxUtil.ShowError("Không thể tải ảnh đã chọn!");
                    }
                }
            }
        }

        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            // Console.WriteLine("Username to check: " + _userViewModel.Users.Count);
            // Lấy dữ liệu từ các TextBox, ComboBox, DatePicker
            var fullName = Fullname.Text?.Trim();
            var gender = _teacherViewModel!.SelectedGender;
            var birthDay = BirthDay.SelectedDate?.DateTime ?? DateTime.Now;
            var phone = Phone.Text?.Trim();
            var email = Email.Text?.Trim();
            var address = Address.Text?.Trim();
            var avatar = AvatarImage.Source;
            var username = Username.Text?.Trim();
            var password = Password.Text?.Trim();
            
            // Lấy Department đã chọn
            var selectedDept = _teacherViewModel.SelectedDepartment;
            string? departmentName = selectedDept?.Name;
            int departmentId = selectedDept?.Id ?? 0;


            // Kiểm tra xác nhận
            var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn thêm giáo viên này?");
            if (!confirm)
                return;

            if (string.IsNullOrWhiteSpace(username))
            {
                await MessageBoxUtil.ShowError("Tên đăng nhập không được để trống!", owner: this);
                return;
            }
            if (!string.IsNullOrWhiteSpace(password) && password.Length < 6)
            {
                await MessageBoxUtil.ShowError("Mật khẩu phải có ít nhất 6 ký tự!", owner: this);
                return;
            }

            // Kiểm tra dữ liệu hợp lệ
            if (string.IsNullOrWhiteSpace(fullName))
            {
                await MessageBoxUtil.ShowError("Họ và tên không được để trống!", owner: this);
                return;
            }

            if (string.IsNullOrWhiteSpace(gender) || gender.StartsWith("---"))
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn giới tính!", owner: this);
                return;
            }

            // Kiểm tra Department
            if (departmentId == 0 || string.IsNullOrWhiteSpace(departmentName))
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn bộ môn!", owner: this);
                return;
            }

            if (!string.IsNullOrWhiteSpace(phone) && (phone.Length != 10 || Rules.rulePhone(phone)))
            {
                await MessageBoxUtil.ShowError("Số điện thoại không hợp lệ!", owner: this);
                return;
            }

            if (!string.IsNullOrWhiteSpace(email) && Rules.ruleEmail(email))
            {
                await MessageBoxUtil.ShowError("Email không đúng định dạng!", owner: this);
                return;
            }

            // Kiểm tra ngày sinh (không cho chọn tương lai)
            if (birthDay > DateTime.Now)
            {
                await MessageBoxUtil.ShowError("Ngày sinh không được lớn hơn ngày hiện tại!", owner: this);
                return;
            }

            // Kiểm tra trùng giáo viên trong danh sách hiện có
            // var exists = _teacherViewModel?.Teachers?.Any(t =>
            //     string.Equals(t.Name, fullName, StringComparison.OrdinalIgnoreCase) &&
            //     string.Equals(t.Gender, gender, StringComparison.OrdinalIgnoreCase) &&
            //     DateTime.TryParse(t.Birthday, out var bDate) && bDate.Date == birthDay.Date);

            // if (exists == true)
            // {
            //     await MessageBoxUtil.ShowWarning("Giáo viên này đã tồn tại trong danh sách!", owner: this);
            //     return;
            // }

            // Ngoài ra, có thể kiểm tra trùng theo SĐT hoặc Email (nếu có)
            var duplicateUsername = !string.IsNullOrWhiteSpace(username) &&
                                _teacherViewModel.Users.Any(s => s.Username == username);
            if (duplicateUsername)
            {
                await MessageBoxUtil.ShowWarning("Tên đăng nhập này đã tồn tại!", owner: this);
                return;
            }
            
            var duplicatePhone = !string.IsNullOrWhiteSpace(phone) &&
                                _teacherViewModel.Users.Any(s => s.Phone == phone);
            if (duplicatePhone)
            {
                await MessageBoxUtil.ShowWarning("Số điện thoại này đã được sử dụng!", owner: this);
                return;
            }

            var duplicateEmail = !string.IsNullOrWhiteSpace(email) &&
                                _teacherViewModel.Users.Any(s => s.Email == email);
            if (duplicateEmail)
            {
                await MessageBoxUtil.ShowWarning("Email này đã được sử dụng!", owner: this);
                return;
            }

            // Gửi dữ liệu tới backend hoặc lưu vào model
            var teacher = new TeacherModel
            {
                Name = fullName,
                AvatarFile = _selectedAvatarPath ?? "",
                Birthday = birthDay.ToString("yyyy-MM-dd"),
                Gender = gender,
                DepartmentId = departmentId,
                Phone = phone ?? "",
                Email = email ?? "",
                Address = address ?? "",
                Username = username ?? "",
                Password = password ?? ""
            };

            // - Xử lý
            bool isSuccess = await _teacherViewModel.CreateTeacherCommand.Execute(teacher).ToTask();

            // Thông báo xử lý, nếu thành công thì ẩn dialog
            if (isSuccess)
            {
                await MessageBoxUtil.ShowSuccess("Thêm giáo viên thành công!", owner: this);
                await _teacherViewModel.GetTeachersCommand.Execute().ToTask();
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Thêm giáo viên thất bại!", owner: this);
                this.Close();
            }
        }   
    }
}
