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

namespace cschool.Views.Student
{
    public partial class StudentCreateDialog : Window
    {
        public StudentViewModel studentViewModel { get; set; }
        private string? _selectedAvatarPath;
        public StudentCreateDialog()
        {
            InitializeComponent();
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
                        await MessageBox("Không thể tải ảnh đã chọn!");
                    }
                }
            }
        }

        private async Task MessageBox(string message)
        {
            var dialog = new Window
            {
                Width = 300,
                Height = 120,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new TextBlock
                {
                    Text = message,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                }
            };
            await dialog.ShowDialog(this);
        }

        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu từ các TextBox, ComboBox, DatePicker
            var learnStatus = (LearnStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var fullName = Fullname.Text?.Trim();
            var gender = (Gender.SelectedItem as ComboBoxItem)?.Content?.ToString();
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

            var confirm = await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn thêm học sinh này?");
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

            if (string.IsNullOrWhiteSpace(ethnicity))
            {
                await MessageBoxUtil.ShowError("Dân tộc không được để trống!", owner: this);
                return;
            }

            if (string.IsNullOrWhiteSpace(religion))
            {
                await MessageBoxUtil.ShowError("Tôn giáo không được để trống!", owner: this);
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

            if (string.IsNullOrWhiteSpace(phone))
            {
                await MessageBoxUtil.ShowError("Số điện thoại không được để trống!", owner: this);
                return;
            }
            else if (!Rules.rulePhone(phone))
            {
                await MessageBoxUtil.ShowError("Số điện thoại không hợp lệ!", owner: this);
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                await MessageBoxUtil.ShowError("Email không được để trống!", owner: this);
                return;
            }
            else if (!Rules.ruleEmail(email))
            {
                await MessageBoxUtil.ShowError("Email không đúng định dạng!", owner: this);
                return;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                await MessageBoxUtil.ShowError("Địa chỉ không được để trống!", owner: this);
                return;
            }

            // Kiểm tra ngày sinh (không cho chọn tương lai)
            if (birthDay > DateTime.Now)
            {
                await MessageBoxUtil.ShowError("Ngày sinh không được lớn hơn ngày hiện tại!", owner: this);
                return;
            }


            // Gửi dữ liệu tới backend hoặc lưu vào model
            var student = new StudentModel
            {
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
                // AvatarFile = _selectedAvatarPath,
                // Avatar = _selectedAvatarPath is null ? "" : Path.GetFileName(_selectedAvatarPath)
            };

            // - Xử lý
            bool isSuccess = await studentViewModel.CreateStudentCommand.Execute(student).ToTask();

            // Thông báo xử lý, nếu thành công thì ẩn dialog
            if (isSuccess)
            {
                await MessageBoxUtil.ShowSuccess("Thêm người dùng thành công!", owner: this);

                await studentViewModel.GetStudentsCommand.Execute().ToTask();

                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowSuccess("Thêm người dùng thất bại!", owner: this);
                this.Close();
            }
        }
    }
}
