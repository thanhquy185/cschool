using System;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Media;
using Utils;
using ViewModels;
using Models;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Styling;
using System.Data;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using System.Collections.Generic;


namespace Views;

public partial class RoleView : UserControl
{
    private RoleViewModel _roleViewModel { get; set; }

    public RoleView()
    {
        InitializeComponent();
        _roleViewModel = new RoleViewModel();
        DataContext = _roleViewModel;
    }

    private IDataTemplate CreateCheckboxTemplate(string valueProp, string allowProp, DialogModeEnum mode)
    {
        return new FuncDataTemplate<RoleDetailRowFormat>((item, _) =>
        {
            var cb = new CheckBox();
            cb.IsEnabled = mode != DialogModeEnum.Info;
            cb.HorizontalAlignment = HorizontalAlignment.Center;
            cb.Bind(CheckBox.IsCheckedProperty, new Binding(valueProp));
            cb.Bind(CheckBox.IsVisibleProperty, new Binding(allowProp));

            return cb;
        });
    }

    private void InfoButton_Click(object sender, RoutedEventArgs e) => _ = ShowRoleDialog(DialogModeEnum.Info);

    private void CreateButton_Click(object sender, RoutedEventArgs e) => _ = ShowRoleDialog(DialogModeEnum.Create);
    private void UpdateButton_Click(object sender, RoutedEventArgs e) => _ = ShowRoleDialog(DialogModeEnum.Update);
    private void LockButton_Click(object sender, RoutedEventArgs e) => _ = ShowRoleDialog(DialogModeEnum.Lock);
    private async Task ShowRoleDialog(DialogModeEnum mode)
    {
        // Nếu là nhấn các nút...
        bool isInfo = mode == DialogModeEnum.Info;
        bool isCreate = mode == DialogModeEnum.Create;
        bool isUpdate = mode == DialogModeEnum.Update;
        bool isLock = mode == DialogModeEnum.Lock;

        // Lấy item đang chọn
        var selectedRole = RolesDataGrid.SelectedItem as RoleModel;
        if (selectedRole == null && !isCreate)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn 1 đối tượng để có thể thực hiện thao tác!");
            return;
        }

        // Mặc định nhấn "Thêm" thì không chọn dòng nào (này xử lý hơi ngu)
        if (isCreate) selectedRole = null;

        // Tạo Window
        var dialog = new Window { Title = "Dialog", Classes = { "DialogWarper" } };

        // Tạo Header
        var header = new Grid
        {
            ColumnDefinitions = mode switch
            {
                DialogModeEnum.Lock => new ColumnDefinitions("*"),
                _ => new ColumnDefinitions("*, Auto, *")
            },
            Classes = { "DialogHeader" }
        };

        // - Tiêu đề
        var title = new TextBlock
        {
            Text = mode switch
            {
                DialogModeEnum.Info => "Chi tiết nhóm quyền",
                DialogModeEnum.Create => "Thêm nhóm quyền",
                DialogModeEnum.Update => "Cập nhật nhóm quyền",
                DialogModeEnum.Lock => $"{(selectedRole?.Status == "Hoạt động" ? "Khoá" : "Mở khoá")} nhóm quyền",
                _ => "Nhóm quyền"
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
        if (!isLock)
        {
            // ========== Truy vấn dữ liệu chức năng ==========
            var functions = AppService.FunctionService.GetFunctions();

            // ========== Tạo 3 dòng ==========
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            formGroupWarper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            // ========== Dòng đầu tiên ==========
            var row0Grid = new Grid { Classes = { "DialogFormGroupRow" } };
            row0Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row0Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            // ===== Mã nhóm quyền =====
            // - Tạo các control
            var idStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var idLabel = new TextBlock { Classes = { "DialogFormGroupLabel" }, Text = "Mã nhóm quyền" };
            var idTextBox = new TextBox
            {
                Classes = { "DialogFormGroupInput", "TextCenter" },
                Text = selectedRole != null ? selectedRole.Id.ToString() : "Được xác định sau khi xác nhận thêm !",
                IsEnabled = false
            };
            // - Thêm vào stack
            idStack.Children.Add(idLabel);
            idStack.Children.Add(idTextBox);
            // - Thêm vào cột
            Grid.SetColumn(idStack, 0);
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
                SelectedItem = !string.IsNullOrWhiteSpace(selectedRole?.Status) ? selectedRole?.Status : "Chọn Trạng thái",
                IsEnabled = !isInfo && !isUpdate
            };
            // - Thêm vào stack
            statusStack.Children.Add(statusLabel);
            statusStack.Children.Add(statusComboBox);
            // - Thêm vào cột
            Grid.SetColumn(statusStack, 1);
            row0Grid.Children.Add(statusStack);
            // --- Thêm tất cả vào dòng đầu tiên ---
            Grid.SetRow(row0Grid, 0);
            formGroupWarper.Children.Add(row0Grid);

            // ========== Dòng thứ 2 ==========
            var row1Grid = new Grid { Classes = { "DialogFormGroupRow" } };
            row1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            // ===== Tên nhóm quyền =====
            // - Tạo các control
            var nameStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var nameLabel = new TextBlock
            {
                Classes = { "DialogFormGroupLabel" },
                Inlines =
                {
                    isCreate ? new Run { Text = "* ", Foreground = Brushes.Red } : new Run {},
                    new Run { Text = "Tên nhóm quyền" }
                }
            };
            var nameTextBox = new TextBox
            {
                Classes = { "DialogFormGroupInput" },
                Watermark = "Nhập Tên nhóm quyền",
                Text = !string.IsNullOrWhiteSpace(selectedRole?.Name) ? selectedRole?.Name : null,
                IsEnabled = !isInfo && !isUpdate
            };
            // - Thêm vào stack
            nameStack.Children.Add(nameLabel);
            nameStack.Children.Add(nameTextBox);
            // - Thêm vào cột
            Grid.SetColumn(nameStack, 0);
            row1Grid.Children.Add(nameStack);
            // --- Thêm tất cả vào dòng thứ 2 ---
            Grid.SetRow(row1Grid, 1);
            formGroupWarper.Children.Add(row1Grid);


            // ========== Dòng thứ 3 ==========
            var row2Grid = new Grid { Classes = { "DialogFormGroupRow" } };
            row2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            // - Tạo các control
            var roleDetailsStack = new StackPanel { Classes = { "DialogFormGroup" } };
            var roleDetailsLabel = new TextBlock
            {
                Classes = { "DialogFormGroupLabel" },
                Inlines =
                {
                    // isCreate ? new Run { Text = "* ", Foreground = Brushes.Red } : new Run {},
                    new Run { Text = "Chi tiết nhóm quyền" }
                }
            };
            var roleDetailsGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                Classes = { "FunctionTable" },
                Margin = new Thickness(0, 10, 0, 0),
                Height = 300
            };
            // -

            // -- Thêm các cột
            // --- Tên chức năng
            roleDetailsGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Tên chức năng",
                Width = new DataGridLength(3, DataGridLengthUnitType.Star),
                Binding = new Avalonia.Data.Binding("FunctionName")
            });
            // --- Xem
            roleDetailsGrid.Columns.Add(new DataGridTemplateColumn
            {
                Header = "Xem",
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                CellTemplate = CreateCheckboxTemplate("CanView", "AllowView", mode),
            });
            // --- Thêm
            roleDetailsGrid.Columns.Add(new DataGridTemplateColumn
            {
                Header = "Thêm",
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                CellTemplate = CreateCheckboxTemplate("CanCreate", "AllowCreate", mode),
            });
            // --- Cập nhật
            roleDetailsGrid.Columns.Add(new DataGridTemplateColumn
            {
                Header = "Cập nhật",
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                CellTemplate = CreateCheckboxTemplate("CanUpdate", "AllowUpdate", mode),
            });
            // --- Xoá / Khoá
            roleDetailsGrid.Columns.Add(new DataGridTemplateColumn
            {
                Header = "Xoá / Khoá",
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                CellTemplate = CreateCheckboxTemplate("CanDelete", "AllowDelete", mode),
            });
            // -- Thêm các dòng chức năng
            var roleDetails = new ObservableCollection<RoleDetailRowFormat>();
            roleDetailsGrid.ItemsSource = roleDetails;
            foreach (FunctionModel function in functions)
            {
                string[] actions = (function.Actions ?? "").Split('|');

                if (!function.IsTeacherFunction)
                {
                    roleDetails.Add(new RoleDetailRowFormat
                    {
                        FunctionId = function.Id,
                        FunctionName = function.Name,

                        CanView = !isCreate ? AppService.RoleDetailService.HasPermission(selectedRole.Id, function.Id, "Xem") : false,
                        CanCreate = !isCreate ? AppService.RoleDetailService.HasPermission(selectedRole.Id, function.Id, "Thêm") : false,
                        CanUpdate = !isCreate ? AppService.RoleDetailService.HasPermission(selectedRole.Id, function.Id, "Cập nhật") : false,
                        CanDelete = !isCreate ? AppService.RoleDetailService.HasPermission(selectedRole.Id, function.Id, "Xoá / Khoá") : false,

                        AllowView = actions.Contains("Xem"),
                        AllowCreate = actions.Contains("Thêm"),
                        AllowUpdate = actions.Contains("Cập nhật"),
                        AllowDelete = actions.Contains("Xoá / Khoá"),
                    });
                }
            }
            // - Thêm vào stack
            roleDetailsStack.Children.Add(roleDetailsLabel);
            roleDetailsStack.Children.Add(roleDetailsGrid);
            // - Thêm vào cột
            Grid.SetColumn(roleDetailsStack, 0);
            row2Grid.Children.Add(roleDetailsStack);
            // -- Thêm tất cả vào dòng thứ 3 --
            Grid.SetRow(row2Grid, 2);
            formGroupWarper.Children.Add(row2Grid);

            // ========== Thêm tất cả dòng ==========
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
                        // - Kiểm tra tên nhóm quyền
                        if (isCreate && Rules.ruleRequiredForTextBox(nameTextBox.Text ?? ""))
                        {
                            await MessageBoxUtil.ShowError("Tên nhóm quyền không được để trống!", owner: dialog);
                            return;
                        }

                        foreach (var roleDetail in roleDetails)
                        {
                            Console.WriteLine(roleDetail.FunctionId + " " + roleDetail.CanCreate);
                        }

                        // // Nếu dữ liệu hợp lệ
                        // // - Khởi tạo đối tượng
                        // var role = new RoleModel();
                        // if (isUpdate) role.Id = int.Parse(idTextBox.Text);
                        // if (isCreate || isUpdate) role.Name = nameTextBox.Text;
                        // if (isCreate) role.Status = statusComboBox.SelectedItem.ToString();
                        // // - Xử lý
                        // bool isSuccess = mode switch
                        // {
                        //     DialogModeEnum.Create => await _roleViewModel.CreateRoleCommand.Execute(role).ToTask(),
                        //     DialogModeEnum.Update => await _roleViewModel.UpdateRoleCommand.Execute(role).ToTask(),
                        // };

                        // // Thông báo xử lý, nếu thành công thì ẩn dialog
                        // if (isSuccess)
                        // {
                        //     if (isCreate) await MessageBoxUtil.ShowSuccess("Thêm người dùng thành công!", owner: dialog);
                        //     if (isUpdate) await MessageBoxUtil.ShowSuccess("Cập nhật người dùng thành công!", owner: dialog);
                        //     dialog.Close();

                        //     RolesDataGrid.ItemsSource = await _roleViewModel.GetRolesCommand.Execute().ToTask();
                        // }
                        // else
                        // {
                        //     if (isCreate) await MessageBoxUtil.ShowSuccess("Thêm người dùng thất bại!", owner: dialog);
                        //     if (isUpdate) await MessageBoxUtil.ShowSuccess("Cập nhật người dùng thất bại!", owner: dialog);
                        // }
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
                    new Run { Text = $"{(selectedRole?.Status == "Hoạt động" ? "khoá" : "mở khoá")} đối tượng "},
                    new Run { Text = "người dùng", Foreground = new SolidColorBrush(Color.Parse("#9f4d4d")), FontWeight = FontWeight.SemiBold},
                    new Run { Text = " có mã là " },
                    new Run { Text = $"#{selectedRole?.Id}", Foreground = new SolidColorBrush(Color.Parse("#9f4d4d")), FontWeight = FontWeight.SemiBold},
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
                        var role = new RoleModel();
                        role.Id = selectedRole.Id;
                        role.Status = selectedRole.Status == "Hoạt động" ? "Tạm dừng" : "Hoạt động";
                        // - Xử lý
                        bool isSuccess = await _roleViewModel.LockRoleCommand.Execute(role).ToTask();

                        // Thông báo xử lý, nếu thành công thì ẩn dialog
                        if (isSuccess)
                        {
                            await MessageBoxUtil.ShowSuccess($"{(selectedRole.Status == "Hoạt động" ? "Khoá" : "Mở khoá")} người dùng thành công!", owner: dialog);
                            dialog.Close();

                            RolesDataGrid.ItemsSource = await _roleViewModel.GetRolesCommand.Execute().ToTask();
                        }
                        else await MessageBoxUtil.ShowSuccess($"{(selectedRole.Status == "Hoạt động" ? "Khoá" : "Mở khoá")} người dùng thất bại!", owner: dialog);
                    }
                };

            // Thêm tất cả vào form
            formGroupWarper.Children.Add(lockForm);
            form.Children.Add(formGroupWarper);
            form.Children.Add(submitButton);
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
                },
                "Role"
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
