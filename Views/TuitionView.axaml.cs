using Avalonia.Controls;
using ViewModels;
using Models;
using System.Threading.Tasks;
using Utils;
using Views.Tuition;
using System;
using Services;

namespace Views;

public partial class TuitionView : UserControl
{
    private TuitionViewModel _tuitionViewModel { get; set; }

    public TuitionView()
    {
        InitializeComponent();
        this._tuitionViewModel = new TuitionViewModel();
        DataContext = this._tuitionViewModel;

        InfoButton.Click += async (_, _) => await ShowTuitionDialog(DialogModeEnum.Info);
        CreateButton.Click += async (_, _) => await ShowTuitionDialog(DialogModeEnum.Create);
        UpdateButton.Click += async (_, _) => await ShowTuitionDialog(DialogModeEnum.Update);

        var tabControl = this.FindControl<TabControl>("TuitionTabControl");
        if (tabControl != null)
            tabControl.SelectionChanged += TuitionTabControl_SelectionChanged;

        // Phân quyền các nút chức năng
        if (SessionService.currentUserLogin != null && AppService.RoleDetailService != null)
        {
            InfoButton.IsEnabled = AppService.RoleDetailService.HasPermission(
                SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Tuition, "Xem");
            CreateButton.IsEnabled = AppService.RoleDetailService.HasPermission(
               SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Tuition, "Thêm");
            UpdateButton.IsEnabled = AppService.RoleDetailService.HasPermission(
               SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Tuition, "Cập nhật");
        }
    }

    private async Task ShowTuitionDialog(DialogModeEnum mode)
    {
        var vm = DataContext as TuitionViewModel;
        ClassModel? selectedClass = null;

        if (mode != DialogModeEnum.Create)
        {
            selectedClass = Tuition_Class.SelectedItem as ClassModel;
            if (selectedClass == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn lớp!");
                return;
            }
        }
        else
        {
            // Create mode: chọn lớp mặc định nếu muốn
            if (Tuition_Class.SelectedItem is ClassModel cls)
                selectedClass = cls;
        }

        vm.SelectedClass = selectedClass;

        if (selectedClass != null)
        {
            await vm.LoadFeeMonths(selectedClass.Id);
            await vm.LoadSelectedFeesForClass(selectedClass);
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Update:
                dialog = new TuitionUpdateDialog(vm);
                break;
            case DialogModeEnum.Create:
                await vm.LoadData();
                dialog = new TuitionCreateDialog(vm);
                break;
            case DialogModeEnum.Info:
                dialog = new TuitionInfoDialog(vm);
                break;
        }

        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null && dialog != null)
            await dialog.ShowDialog(owner);
    }

    private void TuitionTabControl_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is TabControl tabControl)
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                bool isManageTab = selectedTab.Header?.ToString() == "Quản lý học phí";

                CreateButton.Opacity = isManageTab ? 0 : 1;
                CreateButton.IsHitTestVisible = !isManageTab;

                Console.WriteLine($"Tab changed: {selectedTab.Header}");
            }
        }
    }

    private async void StudentDetail_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (sender is not Button btn) return;

            if (btn.DataContext is not TuitionModel studentRow)
            {
                await MessageBoxUtil.ShowError(
                    "Không lấy được dữ liệu học sinh.",
                    owner: TopLevel.GetTopLevel(this) as Window
                );
                return;
            }

            var vm = DataContext as TuitionViewModel;
            if (vm == null) return;

            vm.SelectedStudent = studentRow;
            vm.LoadStudentTuitionDetail(studentRow.StudentId);

            var dlg = new TuitionInfoDialog(vm);
            var owner = TopLevel.GetTopLevel(this) as Window;
            if (owner != null)
                await dlg.ShowDialog(owner);
            else
                dlg.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine("StudentDetail_Click error: " + ex.Message);
        }
    }

    private async void CollectionFee_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (sender is not Button btn) return;

            if (btn.DataContext is not TuitionModel studentRow)
            {
                await MessageBoxUtil.ShowError("Không lấy được dữ liệu học sinh.",
                                                owner: TopLevel.GetTopLevel(this) as Window);
                return;
            }

            var vm = DataContext as TuitionViewModel;
            if (vm == null) return;

            vm.SelectedStudent = studentRow;
            vm.LoadStudentTuitionDetail(studentRow.StudentId);

            var dlg = new TuitionInfoDialog(vm);
            var owner = TopLevel.GetTopLevel(this) as Window;
            if (owner != null)
                await dlg.ShowDialog(owner);
            else
                dlg.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine("CollectionFee_Click error: " + ex.Message);
        }
    }
}
