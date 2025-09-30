using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Media;
using cschool.Utils;
using cschool.ViewModels;

namespace cschool.Views;

public partial class UserView : UserControl
{
    private UserViewModel _userViewModel { get; set; }

    public UserView()
    {
        InitializeComponent();
        _userViewModel = new UserViewModel();
        DataContext = _userViewModel;
    }

    private void InfoButton_Click(object sender, RoutedEventArgs e) => _ = ShowUserDialog(DialogModeEnum.Info);
    private void CreateButton_Click(object sender, RoutedEventArgs e) => _ = ShowUserDialog(DialogModeEnum.Create);
    private void UpdateButton_Click(object sender, RoutedEventArgs e) => _ = ShowUserDialog(DialogModeEnum.Update);
    private void LockButton_Click(object sender, RoutedEventArgs e) => _ = ShowUserDialog(DialogModeEnum.Lock);
    private void ChangePasswordButton_Click(object sender, RoutedEventArgs e) => _ = ShowUserDialog(DialogModeEnum.ChangePassword);
    private async Task ShowUserDialog(DialogModeEnum mode)
    {
        // Nếu là nhấn các nút...
        bool isInfo = mode == DialogModeEnum.Info;
        bool isCreate = mode == DialogModeEnum.Create;
        bool isUpdate = mode == DialogModeEnum.Update;
        bool isLock = mode == DialogModeEnum.Lock;
        bool isChangePassword = mode == DialogModeEnum.ChangePassword;

        // Lấy item đang chọn
        var selectedUser = UsersDataGrid.SelectedItem as UserModel;
        if (selectedUser == null && !isCreate)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn 1 đối tượng để có thể thực hiện thao tác!");
            return;
        }

        // Mặc định nhấn "Thêm" thì không chọn dòng nào (này xử lý hơi ngu)
        if (isCreate) selectedUser = null;

        // Tạo Window
        var dialog = new Window { Title = "Dialog", Classes = { "DialogWarper" } };

        // Tạo Header
        var header = new Grid
        {
            // ColumnDefinitions = new ColumnDefinitions("*, Auto, *"),
            ColumnDefinitions = mode switch
            {
                DialogModeEnum.Lock => new ColumnDefinitions("*"),
                DialogModeEnum.ChangePassword => new ColumnDefinitions("*"),
                _ => new ColumnDefinitions("*, Auto, *")
            },
            Classes = { "DialogHeader" }
        };
        // - Tiêu đề
        var title = new TextBlock
        {
            Text = mode switch
            {
                DialogModeEnum.Info => "Chi tiết người dùng",
                DialogModeEnum.Create => "Thêm người dùng",
                DialogModeEnum.Update => "Cập nhật người dùng",
                DialogModeEnum.Lock => $"{(selectedUser?.Status == "Hoạt động" ? "Khoá" : "Mở khoá")} người dùng",
                DialogModeEnum.ChangePassword => "Thay đổi mật khẩu người dùng",
                _ => "Người dùng"
            },
            Classes = { "DialogHeaderTitle" }
        };
        Grid.SetColumn(title, 1);
        header.Children.Add(title);
        // Nút đóng 'X'
        var closeButton = new Button { Content = "X", Classes = { "DialogCloseButton" } };
        Grid.SetColumn(closeButton, 2);
        header.Children.Add(closeButton);
        closeButton.Click += (_, __) => dialog.Close();

        // Tạo Form
        var form = new StackPanel { Classes = { "DialogForm" }, };
        // - Tạo Form Group Warper
        var formGroupWarper = new Grid { Classes = { "DialogFormGroupWarper" } };
        if (!isLock && !isChangePassword)
        {
            // ========== Tạo 5 dòng ==========
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // ========== Dòng đầu tiên ==========
            var row0Grid = new Grid { Classes = { "DialogFormGroupRow" } };
            row0Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row0Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row0Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            // ===== Ảnh đại diện =====
            // - Tạo canvas
            var imageCanvas = new Canvas();
            // - Tạo các control
            var imageStack = new StackPanel { Classes = { "DialogFormGroup" }, ZIndex = 100 };
            var imageLabel = new TextBlock { Text = "Ảnh đại diện", Classes = { "DialogFormGroupLabel" } };
            var imagePreview = new Image { Classes = { "DialogFormGroupImagePreview" } };
            var imageButton = new Button { Classes = { "DialogFormGroupImageButton" }, Content = "Chọn ảnh", IsEnabled = !isInfo };
            // - Biến và sự kiện khi thay đổi file ảnh
            string? selectedImagePath = null;
            imageButton.Click += async (_, __) =>
            {
                var dlg = new OpenFileDialog
                {
                    AllowMultiple = false,
                    Filters = { new FileDialogFilter { Name = "Ảnh", Extensions = { "png", "jpg", "jpeg" } } }
                };

                var result = await dlg.ShowAsync(dialog); // "dialog" là window cha
                if (result != null && result.Length > 0)
                {
                    selectedImagePath = result[0];
                    imagePreview.Source = new Avalonia.Media.Imaging.Bitmap(selectedImagePath);
                }
            };
            if (!string.IsNullOrWhiteSpace(selectedUser?.Avatar) && !isCreate)
            {
                // selectedImagePath = selectedUser?.Avatar;
                imagePreview.Source = new Avalonia.Media.Imaging.Bitmap($"{AppService.AppPath}/Assets/Images/Users/{selectedUser?.Avatar}");
            }
            // - Thêm vào stack
            imageStack.Children.Add(imageLabel);
            imageStack.Children.Add(imagePreview);
            imageStack.Children.Add(imageButton);
            // - Thiết lập canvas
            Canvas.SetTop(imageStack, 0);
            Canvas.SetLeft(imageStack, 0);
            // - Thêm vào canvas
            imageCanvas.Children.Add(imageStack);
            // - imageCanvas bạn add vào grid hoặc thẳng vào dialog
            Grid.SetRow(imageCanvas, 0);
            Grid.SetColumnSpan(imageCanvas, 3);
            row0Grid.Children.Add(imageCanvas);
            // ===== Mã người dùng =====
            // - Tạo các control
            var idStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var idLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Mã người dùng" };
            var idTextBox = new TextBox
            {
                Classes = { "DialogFormGroupInput", "TextCenter" },
                Text = selectedUser != null ? selectedUser.Id.ToString() : "Được xác định sau khi xác nhận thêm !",
                IsEnabled = false
            };
            // - Thêm vào stack
            idStack.Children.Add(idLabel);
            idStack.Children.Add(idTextBox);
            // - Thêm vào cột
            Grid.SetColumn(idStack, 1);
            row0Grid.Children.Add(idStack);
            // ===== Trạng thái =====
            // - Tạo các control
            var statusStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var statusLabel = new TextBlock
            {
                Classes = { "DialogFormGroupLabel" },
                Inlines =
                {
                    isCreate ? new Run { Text = "* ", Foreground = Brushes.Red } : new Run {},
                    new Run { Text = "Trạng thái" }
                }
            };
            var statusComboBox = new ComboBox
            {
                Classes = { "DialogFormGroupComboBox" },
                Items = { "Chọn Trạng thái", "Hoạt động", "Tạm dừng" },
                SelectedItem = !string.IsNullOrWhiteSpace(selectedUser?.Status) ? selectedUser?.Status : "Chọn Trạng thái",
                IsEnabled = !isInfo && !isUpdate
            };
            // - Thêm vào stack
            statusStack.Children.Add(statusLabel);
            statusStack.Children.Add(statusComboBox);
            // - Thêm vào cột
            Grid.SetColumn(statusStack, 2);
            row0Grid.Children.Add(statusStack);
            // --- Thêm tất cả vào dòng đầu tiên ---
            Grid.SetRow(row0Grid, 0);
            formGroupWarper.Children.Add(row0Grid);

            // ========== Dòng thứ 2 ==========
            var row1Grid = new Grid { Classes = { "DialogFormGroupRow" } };
            row1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            // ===== Tên người dùng =====
            // - Tạo các control
            var usernameStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var usernameLabel = new TextBlock
            {
                Classes = { "DialogFormGroupLabel" },
                Inlines =
                {
                    isCreate ? new Run { Text = "* ", Foreground = Brushes.Red } : new Run {},
                    new Run { Text = "Tên người dùng" }
                }
            };
            var usernameTextBox = new TextBox
            {
                Classes = { "DialogFormGroupInput" },
                Watermark = "Nhập Tên người dùng",
                Text = !string.IsNullOrWhiteSpace(selectedUser?.Username) ? selectedUser?.Username : null,
                IsEnabled = !isInfo && !isUpdate
            };
            // - Thêm vào stack
            usernameStack.Children.Add(usernameLabel);
            usernameStack.Children.Add(usernameTextBox);
            // - Thêm vào cột
            Grid.SetColumn(usernameStack, 1);
            row1Grid.Children.Add(usernameStack);
            // ===== Mật khẩu =====
            // - Tạo các control
            var passwordStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var passwordLabel = new TextBlock
            {
                Classes = { "DialogFormGroupLabel" },
                Inlines =
                {
                    isCreate ? new Run { Text = "* ", Foreground = Brushes.Red } : new Run {},
                    new Run { Text = "Mật khẩu" }
                }
            };
            var passwordTextBox = new TextBox
            {
                Classes = { "DialogFormGroupInput" },
                Watermark = "Nhập Mật khẩu",
                Text = !string.IsNullOrWhiteSpace(selectedUser?.Password) ? "Mật khẩu đã được mã hoá!" : null,
                IsEnabled = !isInfo && !isUpdate
            };
            // - Thêm vào stack
            passwordStack.Children.Add(passwordLabel);
            passwordStack.Children.Add(passwordTextBox);
            // - Thêm vào cột
            Grid.SetColumn(passwordStack, 2);
            row1Grid.Children.Add(passwordStack);
            // --- Thêm tất cả vào dòng thứ 2 ---
            Grid.SetRow(row1Grid, 1);
            formGroupWarper.Children.Add(row1Grid);

            // ========== Dòng thứ 3 ==========
            var row2Grid = new Grid { Classes = { "DialogFormGroupRow" } };
            row2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            // ===== Nhóm quyền =====
            // - Tạo các control
            var roleStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var roleLabel = new TextBlock
            {
                Classes = { "DialogFormGroupLabel" },
                Inlines =
                {
                    isCreate || isUpdate ? new Run { Text = "* ", Foreground = Brushes.Red } : new Run {},
                    new Run { Text = "Nhóm quyền" }
                }
            };
            var roleComboBox = new ComboBox
            {
                Classes = { "DialogFormGroupComboBox" },
                Items = { "Chọn Nhóm quyền", "#1 - Quản lý học sinh", "#2 - Quản lý giáo viên" },
                SelectedIndex = 0,
                IsEnabled = !isInfo
            };
            // - Thêm vào stack
            roleStack.Children.Add(roleLabel);
            roleStack.Children.Add(roleComboBox);
            // - Thêm vào cột
            Grid.SetColumn(roleStack, 1);
            row2Grid.Children.Add(roleStack);
            // ===== Họ và tên =====
            // - Tạo các control
            var fullnameStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var fullnameLabel = new TextBlock
            {
                Classes = { "DialogFormGroupLabel" },
                Inlines =
                {
                    isCreate || isUpdate ? new Run { Text = "* ", Foreground = Brushes.Red } : new Run {},
                    new Run { Text = "Họ và tên" }
                }
            };
            var fullnameTextBox = new TextBox
            {
                Classes = { "DialogFormGroupInput" },
                Watermark = "Nhập Họ và tên",
                Text = !string.IsNullOrWhiteSpace(selectedUser?.Fullname) ? selectedUser?.Fullname : null,
                IsEnabled = !isInfo
            };
            // - Thêm vào stack
            fullnameStack.Children.Add(fullnameLabel);
            fullnameStack.Children.Add(fullnameTextBox);
            // - Thêm vào cột
            Grid.SetColumn(fullnameStack, 2);
            row2Grid.Children.Add(fullnameStack);
            // --- Thêm tất cả vào dòng thứ 3 ---
            Grid.SetRow(row2Grid, 2);
            formGroupWarper.Children.Add(row2Grid);

            // ========== Dòng thứ 4 ==========
            var row3Grid = new Grid { Classes = { "DialogFormGroupRow" } };
            row3Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row3Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row3Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            // ===== Số điện thoại =====
            // - Tạo các control
            var phoneStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var phoneLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Số điện thoại" };
            var phoneTextBox = new MaskedTextBox
            {
                Classes = { "DialogFormGroupInput" },
                Watermark = "Nhập Số điện thoại",
                Text = !string.IsNullOrWhiteSpace(selectedUser?.Phone) ? selectedUser?.Phone : null,
                IsEnabled = !isInfo
            };
            // - Thêm vào stack
            phoneStack.Children.Add(phoneLabel);
            phoneStack.Children.Add(phoneTextBox);
            // - Thêm vào cột
            Grid.SetColumn(phoneStack, 1);
            row3Grid.Children.Add(phoneStack);
            // ===== Email =====
            // - Tạo các control
            var emailStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var emailLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Email" };
            var emailTextBox = new TextBox
            {
                Classes = { "DialogFormGroupInput" },
                Watermark = "Nhập Email",
                Text = !string.IsNullOrWhiteSpace(selectedUser?.Email) ? selectedUser?.Email : null,
                IsEnabled = !isInfo
            };
            // - Thêm vào stack
            emailStack.Children.Add(emailLabel);
            emailStack.Children.Add(emailTextBox);
            // - Thêm vào cột
            Grid.SetColumn(emailStack, 2);
            row3Grid.Children.Add(emailStack);
            // --- Thêm tất cả vào dòng thứ 4 ---
            Grid.SetRow(row3Grid, 3);
            formGroupWarper.Children.Add(row3Grid);

            // ========== Dòng thứ 5 ==========
            var row4Grid = new Grid { Classes = { "DialogFormGroupRow" } };
            row4Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row4Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            // row4Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Star) });
            // ===== Địa chỉ =====
            // - Tạo các control
            var addressStack = new StackPanel { Classes = { "DialogFormGroup", "Multiple2" } };
            var addressLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Địa chỉ" };
            var addressTextBox = new TextBox
            {
                Classes = { "DialogFormGroupInput" },
                Watermark = "Nhập Địa chỉ",
                Text = !string.IsNullOrWhiteSpace(selectedUser?.Address) ? selectedUser?.Address : null,
                IsEnabled = !isInfo,
            };
            // - Thêm vào stack
            addressStack.Children.Add(addressLabel);
            addressStack.Children.Add(addressTextBox);
            // - Thêm vào cột
            Grid.SetColumn(addressStack, 1);
            row4Grid.Children.Add(addressStack);
            // --- Thêm tất cả vào dòng thứ 5 ---
            Grid.SetRow(row4Grid, 4);
            formGroupWarper.Children.Add(row4Grid);
            // ----- Thêm tất cả dòng -----
            form.Children.Add(formGroupWarper);

            // ========== Thêm nút submit ==========
            if (!isInfo)
            {
                var submitButton = new Button { Content = "Xác nhận", Classes = { "DialogSubmitButton" } };
                submitButton.Click += async (_, __) =>
                {
                    if (await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn xác nhận? Hành động này không thể hoàn tác!"))
                    {
                        // Kiểm tra dữ liệu
                        // - Kiểm tra trạng thái
                        if (isCreate && Rules.ruleRequiredForComboBox(statusComboBox, "Chọn Trạng thái"))
                        {
                            await MessageBoxUtil.ShowError("Trạng thái không được để trống!", owner: dialog);
                            return;

                        }
                        // - Kiểm tra tên người dùng
                        if (isCreate && Rules.ruleRequiredForTextBox(usernameTextBox.Text ?? ""))
                        {
                            await MessageBoxUtil.ShowError("Tên người dùng không được để trống!", owner: dialog);
                            return;
                        }
                        if (isCreate && await _userViewModel.UserIsExistsByUsernameCommand.Execute(usernameTextBox.Text).ToTask())
                        {
                            await MessageBoxUtil.ShowError("Tên người dùng đã tồn tại!", owner: dialog);
                            return;
                        }
                        // - Kiểm tra mật khẩu
                        if (isCreate && Rules.ruleRequiredForTextBox(passwordTextBox.Text ?? ""))
                        {
                            await MessageBoxUtil.ShowError("Mật khẩu không được để trống!", owner: dialog);
                            return;
                        }
                        // - Kiểm tra nhóm quyền
                        if (Rules.ruleRequiredForComboBox(roleComboBox, "Chọn Nhóm quyền"))
                        {
                            await MessageBoxUtil.ShowError("Nhóm quyền không được để trống!", owner: dialog);
                            return;
                        }
                        // - Kiểm tra họ và tên
                        if (Rules.ruleRequiredForTextBox(fullnameTextBox.Text ?? ""))
                        {
                            await MessageBoxUtil.ShowError("Họ và tên không được để trống!", owner: dialog);
                            return;
                        }
                        // Kiểm tra số điện thoại
                        if (Rules.rulePhone(phoneTextBox.Text ?? ""))
                        {
                            await MessageBoxUtil.ShowError("Số điện thoại phải gồm 10 hoặc 11 chữ số!", owner: dialog);
                            return;
                        }
                        // Kiểm tra email
                        if (Rules.ruleEmail(emailTextBox.Text ?? ""))
                        {
                            await MessageBoxUtil.ShowError("Email không đúng định dạng!", owner: dialog);
                            return;
                        }

                        // Nếu dữ liệu hợp lệ
                        // - Khởi tạo đối tượng
                        var user = new UserModel();
                        if (isUpdate) user.Id = int.Parse(idTextBox.Text);
                        if (isCreate || isUpdate) user.AvatarFile = selectedImagePath;
                        if (isCreate || isUpdate) user.RoleId = 0;
                        if (isCreate) user.Username = usernameTextBox.Text;
                        if (isCreate) user.Password = passwordTextBox.Text;
                        if (isCreate || isUpdate) user.Fullname = fullnameTextBox.Text;
                        if (isCreate || isUpdate) user.Phone = phoneTextBox.Text;
                        if (isCreate || isUpdate) user.Email = emailTextBox.Text;
                        if (isCreate || isUpdate) user.Address = addressTextBox.Text;
                        if (isCreate) user.Status = statusComboBox.SelectedItem.ToString();
                        // - Xử lý
                        bool isSuccess = mode switch
                        {
                            DialogModeEnum.Create => await _userViewModel.CreateUserCommand.Execute(user).ToTask(),
                            DialogModeEnum.Update => await _userViewModel.UpdateUserCommand.Execute(user).ToTask(),
                        };

                        // Thông báo xử lý, nếu thành công thì ẩn dialog
                        if (isSuccess)
                        {
                            if (isCreate) await MessageBoxUtil.ShowSuccess("Thêm người dùng thành công!", owner: dialog);
                            if (isUpdate) await MessageBoxUtil.ShowSuccess("Cập nhật người dùng thành công!", owner: dialog);
                            dialog.Close();

                            UsersDataGrid.ItemsSource = await _userViewModel.GetUsersCommand.Execute().ToTask();
                        }
                        else
                        {
                            if (isCreate) await MessageBoxUtil.ShowSuccess("Thêm người dùng thất bại!", owner: dialog);
                            if (isUpdate) await MessageBoxUtil.ShowSuccess("Cập nhật người dùng thất bại!", owner: dialog);
                        }
                    }
                };
                form.Children.Add(submitButton);
            }
        }
        else if (isLock)
        {
            // =========== Tạo 1 dòng ==========
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Lock Form
            var lockForm = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                },
                Classes = { "LockForm" }
            };
            // Ảnh
            var image = new Image { Source = new Avalonia.Media.Imaging.Bitmap(AppService.QuestionIconPath) };
            Grid.SetRow(image, 0);
            lockForm.Children.Add(image);
            // Nội dung
            var textBlock = new TextBlock
            {
                Inlines =
                {
                    new Run { Text = "Bạn có chắc chắn rằng muốn " },
                    new Run { Text = $"{(selectedUser?.Status == "Hoạt động" ? "khoá" : "mở khoá")} đối tượng "},
                    new Run { Text = "người dùng", Foreground = new SolidColorBrush(Color.Parse("#9f4d4d")), FontWeight = FontWeight.SemiBold},
                    new Run { Text = " có mã là " },
                    new Run { Text = $"#{selectedUser?.Id}", Foreground = new SolidColorBrush(Color.Parse("#9f4d4d")), FontWeight = FontWeight.SemiBold},
                },
            };
            Grid.SetRow(textBlock, 1);
            lockForm.Children.Add(textBlock);
            // Thêm nút submit
            var submitButton = new Button { Content = "Xác nhận", Classes = { "DialogSubmitButton" }, };
            submitButton.Click += async (_, __) =>
                {
                    if (await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn xác nhận? Hành động này không thể hoàn tác!"))
                    {

                        // Nếu dữ liệu hợp lệ
                        // - Khởi tạo đối tượng
                        var user = new UserModel();
                        user.Id = selectedUser.Id;
                        user.Status = selectedUser.Status == "Hoạt động" ? "Tạm dừng" : "Hoạt động";
                        // - Xử lý
                        bool isSuccess = await _userViewModel.LockUserCommand.Execute(user).ToTask();

                        // Thông báo xử lý, nếu thành công thì ẩn dialog
                        if (isSuccess)
                        {
                            await MessageBoxUtil.ShowSuccess($"{(selectedUser.Status == "Hoạt động" ? "Khoá" : "Mở khoá")} người dùng thành công!", owner: dialog);
                            dialog.Close();

                            UsersDataGrid.ItemsSource = await _userViewModel.GetUsersCommand.Execute().ToTask();
                        }
                        else await MessageBoxUtil.ShowSuccess($"{(selectedUser.Status == "Hoạt động" ? "Khoá" : "Mở khoá")} người dùng thất bại!", owner: dialog);
                    }
                };

            // Thêm tất cả vào form
            formGroupWarper.Children.Add(lockForm);
            form.Children.Add(formGroupWarper);
            form.Children.Add(submitButton);
        }
        else if (isChangePassword)
        {

        }
        // Tạo Form Warper 
        var formWarper = new Border { Classes = { "DialogFormWarper" }, Child = form };

        // Container chính
        var container = new Border
        {
            Child = new StackPanel { Children = { header, formWarper } },
            Classes =
            {
                "Dialog",
                mode switch {
                    DialogModeEnum.Info => "Info",
                    DialogModeEnum.Create => "Create",
                    DialogModeEnum.Update => "Update",
                    DialogModeEnum.Lock => "Lock",
                    DialogModeEnum.ChangePassword => "ChangePassword",
                },
                "User"
            },
        };

        dialog.Content = container;

        // Hiện Window kiểu modal
        var topLevel = TopLevel.GetTopLevel(this);
        var owner = topLevel as Window;
        if (owner != null) await dialog.ShowDialog(owner);
        else dialog.Show();
    }
}