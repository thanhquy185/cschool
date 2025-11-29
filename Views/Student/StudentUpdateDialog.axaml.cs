using Avalonia.Controls;
using System;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Interactivity;
using System.Reactive.Threading.Tasks;
using cschool.ViewModels;
using cschool.Utils;
using System.Linq;

namespace cschool.Views.Student
{
    public partial class StudentUpdateDialog : Window
    {
        public StudentViewModel studentViewModel { get; set; }
        private string? _selectedAvatarPath;
        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }

        public StudentUpdateDialog(StudentViewModel vm)
        {
            InitializeComponent();
            studentViewModel = vm;
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
            // Lấy dữ liệu từ các TextBox, ComboBox, DatePicker
            var id = Convert.ToInt32(Id.Text?.Trim());
            var learnStatus = LearnStatus.SelectedItem as string;
            var fullName = Fullname.Text?.Trim();
            var gender = Gender.SelectedItem as string;
            var birthDay = BirthDay.SelectedDate?.DateTime ?? DateTime.Now;
            var ethnicity = Ethnicity.Text?.Trim();
            var religion = Religion.Text?.Trim();
            var learnYear = LearnYear.Text?.Trim();
            var phone = Phone.Text?.Trim();
            var email = Email.Text?.Trim();
            var address = Address.Text?.Trim();


            // Lấy ảnh hiện tại trong Image control
            var avatar = AvatarImage.Source;

            // Kiểm tra xác nhận

            var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn cập nhật học sinh này?");
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

            if (string.IsNullOrWhiteSpace(learnStatus))
            {
                await MessageBoxUtil.ShowError("Tình trạng học tập không được để trống!", owner: this);
                return;
            }

            if (string.IsNullOrWhiteSpace(learnYear))
            {
                await MessageBoxUtil.ShowError("Năm học không được để trống!", owner: this);
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
            
            // Kiểm tra trùng học sinh trong danh sách hiện có
            var exists = studentViewModel.AllStudents.Any(s =>
                string.Equals(s.Fullname, fullName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.Gender, gender, StringComparison.OrdinalIgnoreCase) &&
                DateTime.TryParse(s.BirthDay, out var bDate) && bDate.Date == birthDay.Date &&
                string.Equals(s.Ethnicity, ethnicity, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.Religion, religion, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.Phone, phone, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.Email, email, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.Address, address, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                await MessageBoxUtil.ShowWarning("Học sinh này đã tồn tại trong danh sách!", owner: this);
                return;
            }

            // Gửi dữ liệu tới backend hoặc lưu vào model
            var student = new StudentModel
            {
                Id = id,
                Fullname = fullName,
                BirthDay = birthDay.ToString("yyyy-MM-dd"),
                Gender = gender,
                Ethnicity = ethnicity,
                Religion = religion,
                Phone = phone,
                Email = email,
                Address = address,
                LearnYear = learnYear,
                LearnStatus = learnStatus,
                AvatarFile = _selectedAvatarPath,
            };

            // - Xử lý
            bool isSuccess = await studentViewModel.UpdateStudentCommand.Execute(student).ToTask();

            // Thông báo xử lý, nếu thành công thì ẩn dialog
            if (isSuccess)
            {
                await studentViewModel.GetStudentsCommand.Execute().ToTask();
                await MessageBoxUtil.ShowSuccess("Cập nhật học sinh thành công!", owner: this);
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Cập nhật học sinh thất bại!", owner: this);
                this.Close();
            }
        }
    }
}
