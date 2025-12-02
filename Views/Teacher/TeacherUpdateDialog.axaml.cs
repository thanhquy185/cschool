using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Interactivity;
using System.Reactive.Threading.Tasks;
using System.Collections.Generic;

using cschool.ViewModels;
using cschool.Utils;
using System.Linq;
using cschool.Models;

namespace cschool.Views.Teacher
{
    public partial class TeacherUpdateDialog : Window
    {
        private TeacherViewModel _teacherViewModel { get; set; }
        private string? _selectedAvatarPath;
        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }

        public TeacherUpdateDialog(TeacherViewModel vm)
        {
            InitializeComponent();
            _teacherViewModel = vm;
            DataContext = vm;

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
            Console.WriteLine("🔍 ConfirmButton_Click started");

            if (_teacherViewModel == null || _teacherViewModel.TeacherDetails == null)
            {
                Console.WriteLine("ViewModel or TeacherDetails is null");
                await MessageBoxUtil.ShowError("Lỗi: Dữ liệu không tồn tại!", owner: this);
                return;
            }

            // Lấy dữ liệu từ các TextBox, ComboBox, DatePicker

            var id = Convert.ToInt32(Id.Text?.Trim());
            var status = _teacherViewModel.SelectedStatus?.Key ?? 1;
            var fullName = Fullname.Text?.Trim();
            var gender = _teacherViewModel.SelectedGender;
            var birthDay = BirthDay.SelectedDate?.DateTime ?? DateTime.Now;
            var phone = Phone.Text?.Trim();
            var email = Email.Text?.Trim();
            var address = Address.Text?.Trim();
            
            var selectedDept = _teacherViewModel.SelectedDepartment;
            string? departmentName = selectedDept?.Name;
            int departmentId = selectedDept?.Id ?? 0;


            // Lấy ảnh hiện tại trong Image control
            var avatar = AvatarImage.Source;

            // Kiểm tra xác nhận
            var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn cập nhật giáo viên này?");
            if (!confirm)
                return;

            // Kiểm tra dữ liệu hợp lệ
            if (string.IsNullOrWhiteSpace(fullName))
            {
                await MessageBoxUtil.ShowError("Họ và tên không được để trống!", owner: this);
                return;
            }

            if (string.IsNullOrWhiteSpace(gender))
            {
                await MessageBoxUtil.ShowError("Giới tính không được để trống!", owner: this);
                return;
            }
    
            if (Rules.rulePhone(phone))
            {
                await MessageBoxUtil.ShowError("Số điện thoại không hợp lệ!", owner: this);
                return;
            }

            if (Rules.ruleEmail(email))
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
            

            // // Ngoài ra, có thể kiểm tra trùng theo SĐT hoặc Email (nếu có)
            // var duplicatePhone = !string.IsNullOrWhiteSpace(phone) &&
            //                     studentViewModel.Students.Any(s => s.Phone == phone);
            // if (duplicatePhone)
            // {
            //     await MessageBoxUtil.ShowWarning("Số điện thoại này đã được sử dụng!", owner: this);
            //     return;
            // }

            // var duplicateEmail = !string.IsNullOrWhiteSpace(email) &&
            //                     studentViewModel.Students.Any(s => s.Email == email);
            // if (duplicateEmail)
            // {
            //     await MessageBoxUtil.ShowWarning("Email này đã được sử dụng!", owner: this);
            //     return;
            // }


            // Gửi dữ liệu tới backend hoặc lưu vào model
            var teacher = new TeacherModel
            {
                Id = id,
                Name = fullName,
                Birthday = birthDay.ToString("yyyy-MM-dd"),
                Gender = gender,
                Phone = phone,
                Email = email,
                Address = address,
                DepartmentId = departmentId,
                DepartmentName = departmentName ?? "",
                Status = Convert.ToInt32(status),
                AvatarFile = _selectedAvatarPath ?? "",
            };

            // - Xử lý
            bool isSuccess = await _teacherViewModel.UpdateTeacherCommand.Execute(teacher).ToTask();

            // Thông báo xử lý, nếu thành công thì ẩn dialog
            if (isSuccess)
            {
                await MessageBoxUtil.ShowSuccess("Cập nhật giáo viên thành công!", owner: this);
                await _teacherViewModel.GetTeachersCommand.Execute().ToTask();
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Cập nhật giáo viên thất bại!", owner: this);
                this.Close();
            }
        }
    }
}
