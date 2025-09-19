using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using cschool.ViewModels;
using FluentAvalonia.UI.Controls;

namespace cschool.Views;

public partial class AccountView : UserControl
{
    public AccountView()
    {
        InitializeComponent();
        DataContext = new AccountViewModel();
    }

    // private async void TestDialogButton_Click(object sender, RoutedEventArgs e)
    // {
    //     // Header
    //     var header = new Grid
    //     {
    //         ColumnDefinitions = new ColumnDefinitions("*, Auto, *"),
    //         Classes = { "DialogHeader" }
    //     };
    //     // - Title
    //     var title = new TextBlock
    //     {
    //         Text = "Thông tin học sinh",
    //         Classes = { "DialogHeaderTitle" }
    //     };
    //     Grid.SetColumn(title, 1);
    //     header.Children.Add(title);
    //     // - Close Button
    //     var closeButton = new Button
    //     {
    //         Content = "X",
    //         Classes = { "DialogCloseButton" }
    //     };
    //     Grid.SetColumn(closeButton, 2);
    //     header.Children.Add(closeButton);

    //     // Form 
    //     var form = new StackPanel
    //     {
    //         Classes = { "DialogForm" },
    //     };
    //     // - Label, Input, Select...
    //     // Grid chính
    //     var formGroupWarper = new Grid { Classes = { "DialogFormGroupWarper" }, Width = 700 };
    //     // Dòng 0
    //     formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
    //     var row0Grid = new Grid { Classes = { "DialogFormGroupRow" } };
    //     row0Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    //     row0Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    //     // Cột 0: Tên học sinh
    //     var nameStack = new StackPanel { Classes = { "DialogFormGroup" } };
    //     var nameLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Tên học sinh", };
    //     var nameTextBox = new TextBox { Classes = { "DialogFormGroupInput" } };
    //     nameStack.Children.Add(nameLabel);
    //     nameStack.Children.Add(nameTextBox);
    //     Grid.SetColumn(nameStack, 0);
    //     row0Grid.Children.Add(nameStack);
    //     // Cột 1: Tuổi
    //     var ageStack = new StackPanel { Classes = { "DialogFormGroup" } };
    //     var ageLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Tuổi", };
    //     var ageTextBox = new TextBox { Classes = { "DialogFormGroupInput" } };
    //     ageStack.Children.Add(ageLabel);
    //     ageStack.Children.Add(ageTextBox);
    //     Grid.SetColumn(ageStack, 1);
    //     row0Grid.Children.Add(ageStack);
    //     // Gán vị trí các dòng
    //     Grid.SetRow(row0Grid, 0);
    //     // Thêm các dòng
    //     formGroupWarper.Children.Add(row0Grid);
    //     // Thêm formGroupWarper vào form
    //     form.Children.Add(formGroupWarper);
    //     // - Submit Button
    //     var submitButton = new Button
    //     {
    //         Content = "Xác nhận",
    //         Classes = { "DialogSubmitButton" }
    //     };
    //     form.Children.Add(submitButton);
    //     // - Form Warper
    //     var formWarper = new Border
    //     {
    //         Classes = { "DialogFormWarper" },
    //         Child = form
    //     };

    //     // Nội dung dialog (vì không dùng title và các nút mặc định của control)
    //     var dialogContent = new StackPanel
    //     {
    //         Children =
    //     {
    //         header,
    //         formWarper
    //     }
    //     };

    //     // Tạo dialog
    //     var dialog = new ContentDialog
    //     {
    //         Title = null,
    //         Content = dialogContent,
    //         PrimaryButtonText = null,
    //         Classes = { "Dialog", "Create" },
    //     };
    //     // dialog.SetValue(ContentDialog., new Size(1000, 800));

    //     // Đóng dialog khi bấm X
    //     closeButton.Click += (_, __) => dialog.Hide();

    //     // Hiện dialog
    //     var result = await dialog.ShowAsync();
    //     submitButton.Click += (_, __) =>
    //     {
    //         Console.WriteLine($"Tên: {nameTextBox.Text}, Tuổi: {ageTextBox.Text}");
    //         dialog.Hide(ContentDialogResult.Primary);
    //     };
    // }
    private async void TestDialogButton_Click(object sender, RoutedEventArgs e)
    {
        // Tạo Window
        var dialog = new Window
        {
            Title = "Dialog",
            Classes = { "DialogWarper" }
        };

        // Tạo Header
        var header = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto, *"),
            Classes = { "DialogHeader" }
        };
        // - Tiêu đề
        var title = new TextBlock
        {
            Text = "Thêm tài khoản",
            Classes = { "DialogHeaderTitle" }
        };
        Grid.SetColumn(title, 1);
        header.Children.Add(title);
        // Nút đóng 'X'
        var closeButton = new Button
        {
            Content = "X",
            Classes = { "DialogCloseButton" }
        };
        Grid.SetColumn(closeButton, 2);
        header.Children.Add(closeButton);
        closeButton.Click += (_, __) => dialog.Close();

        // Tạo Form
        var form = new StackPanel
        {
            Classes = { "DialogForm" },
        };
        // - Tạo Form Group Warper
        var formGroupWarper = new Grid
        {
            Classes = { "DialogFormGroupWarper" },
        };
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
        var imageButton = new Button { Classes = { "DialogFormGroupImageButton" }, Content = "Chọn ảnh" };
        // - Biến và sự kiện khi thay đổi file ảnh
        string? selectedImagePath = null;
        imageButton.Click += async (_, __) =>
        {
            var dlg = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters =
                {
            new FileDialogFilter { Name = "Ảnh", Extensions = { "png", "jpg", "jpeg" } }
                }
            };

            var result = await dlg.ShowAsync(dialog); // "dialog" là window cha
            if (result != null && result.Length > 0)
            {
                selectedImagePath = result[0];
                imagePreview.Source = new Avalonia.Media.Imaging.Bitmap(selectedImagePath);
            }
        };
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
        // ===== Mã tài khoản =====
        // - Tạo các control
        var idStack = new StackPanel { Classes = { "DialogFormGroup" } };
        var idLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Mã tài khoản" };
        var idTextBox = new TextBox { Classes = { "DialogFormGroupInput", "TextCenter" }, Watermark = "Mã tài khoản", IsEnabled = false };
        // - Thêm vào stack
        idStack.Children.Add(idLabel);
        idStack.Children.Add(idTextBox);
        // - Thêm vào cột
        Grid.SetColumn(idStack, 1);
        row0Grid.Children.Add(idStack);
        // ===== Trạng thái =====
        // - Tạo các control
        var statusStack = new StackPanel { Classes = { "DialogFormGroup" } };
        var statusLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Trạng thái" };
        var statusComboBox = new ComboBox { Classes = { "DialogFormGroupComboBox" }, Items = { "Chọn Trạng thái", "Hoạt động", "Tạm dừng" }, SelectedIndex = 0 };
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
        // ===== Tên tài khoản =====
        // - Tạo các control
        var usernameStack = new StackPanel { Classes = { "DialogFormGroup" } };
        var usernameLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Tên tài khoản" };
        var usernameTextBox = new TextBox { Classes = { "DialogFormGroupInput" }, Watermark = "Nhập Tên tài khoản" };
        // - Thêm vào stack
        usernameStack.Children.Add(usernameLabel);
        usernameStack.Children.Add(usernameTextBox);
        // - Thêm vào cột
        Grid.SetColumn(usernameStack, 1);
        row1Grid.Children.Add(usernameStack);
        // ===== Mật khẩu =====
        // - Tạo các control
        var passwordStack = new StackPanel { Classes = { "DialogFormGroup" } };
        var passwordLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Mật khẩu" };
        var passwordTextBox = new TextBox { Classes = { "DialogFormGroupInput" }, Watermark = "Nhập Mật khẩu" };
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
        var roleLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Nhóm quyền" };
        var roleComboBox = new ComboBox { Classes = { "DialogFormGroupComboBox" }, Items = { "Chọn Nhóm quyền", "#1 - Quản lý học sinh", "#2 - Quản lý giáo viên" }, SelectedIndex = 0 };
        // - Thêm vào stack
        roleStack.Children.Add(roleLabel);
        roleStack.Children.Add(roleComboBox);
        // - Thêm vào cột
        Grid.SetColumn(roleStack, 1);
        row2Grid.Children.Add(roleStack);
        // ===== Họ và tên =====
        // - Tạo các control
        var fullnameStack = new StackPanel { Classes = { "DialogFormGroup" } };
        var fullnameLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Họ và tên" };
        var fullnameTextBox = new TextBox { Classes = { "DialogFormGroupInput" }, Watermark = "Nhập Họ và tên" };
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
        var phoneTextBox = new MaskedTextBox { Classes = { "DialogFormGroupInput" }, Mask = "(+09) 000 000 000" };
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
        var emailTextBox = new TextBox { Classes = { "DialogFormGroupInput" }, Watermark = "Nhập Email" };
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
        var addressTextBox = new TextBox { Classes = { "DialogFormGroupInput" }, Watermark = "Nhập Địa chỉ" };
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
        // - Tạo Submit Button
        var submitButton = new Button
        {
            Content = "Xác nhận",
            Classes = { "DialogSubmitButton" }
        };
        submitButton.Click += (_, __) =>
        {
            // Console.WriteLine($"Tên tài khoản: {idTextBox.Text}, Mật khẩu: {statusComboBox.SelectedItem.ToString()}");
            dialog.Close();
        };
        form.Children.Add(submitButton);

        // Tạo Form Warper 
        var formWarper = new Border
        {
            Classes = { "DialogFormWarper" },
            Child = form
        };

        // Container chính
        var container = new Border
        {
            Child = new StackPanel
            {
                Children =
            {
                header,
                formWarper
            }
            },
            Classes = { "Dialog", "Create", "Account" },
        };

        dialog.Content = container;

        // Hiện Window kiểu modal
        var topLevel = TopLevel.GetTopLevel(this);
        var owner = topLevel as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }
}