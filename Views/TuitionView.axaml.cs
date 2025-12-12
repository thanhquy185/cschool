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
          
            // vm.GenerateFeeMonths();
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Update:
                 System.Console.WriteLine("Opening TuitionUpdateDialog for class Id: " + vm.SelectedClass?.Id);
                dialog = new TuitionUpdateDialog(vm);
                break;
            case DialogModeEnum.Create:
                await vm.LoadData();
                dialog = new TuitionCreateDialog(vm);
                break;
            case DialogModeEnum.Info:
                
                var tabControl = this.FindControl<TabControl>("TuitionTabControl");
                if (tabControl?.SelectedItem is TabItem selectedTab)
                {
                    if (selectedTab.Header?.ToString() == "Mức học phí")
                    {
                        // Tab "Mức học phí" → hiển thị chi tiết fee
                        dialog = new TuitionFeeDetailDialog(vm);
                    }
                    else
                    {
                        // Tab "Quản lý học phí" → hiển thị chi tiết học sinh
                        dialog = new TuitionInfoDialog(vm);
                    }
                }
                else
                {
                    dialog = new TuitionInfoDialog(vm);
                }
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
                
                InfoButton.Opacity = isManageTab ? 0 : 1;
                InfoButton.IsHitTestVisible = !isManageTab;

                UpdateButton.Opacity = isManageTab ? 0 : 1;
                UpdateButton.IsHitTestVisible = !isManageTab;

                // Reset search/filter khi chuyển tab
                var vm = DataContext as TuitionViewModel;
                if (vm != null)
                {
                    var searchBox = this.FindControl<TextBox>("SearchBox");
                    if (searchBox != null)
                        searchBox.Text = string.Empty;

                    if (isManageTab)
                    {
                        // Tab "Quản lý học phí" - reset student filters
                        FilterClass.IsVisible = true;
                        vm.StudentSearchText = string.Empty;
                        vm.SelectedFilterClassYear = "Tất cả";
                        vm.SelectedFilterClassName = "Tất cả";
                        vm.FilterTuitionSummary();
                    }
                    else
                    {
                        FilterClass.IsVisible = false;
                        vm.SelectedFilterClassName = "Tất cả";
                        vm.FeeClassSearchText = string.Empty;
                        vm.FilterFeeClassList();
                    }
                }

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

            var dlg = new TuitionCollectionDialog(vm);
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

    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DataContext is TuitionViewModel vm && sender is TextBox tb)
        {
            var tabControl = this.FindControl<TabControl>("TuitionTabControl");
            if (tabControl?.SelectedItem is TabItem selectedTab)
            {
                bool isManageTab = selectedTab.Header?.ToString() == "Quản lý học phí";

                if (isManageTab)
                {
                    // "Quản lý học phí" tab - tìm kiếm theo tên học sinh
                    vm.StudentSearchText = tb.Text?.Trim();
                }
                else
                {
                    // "Mức học phí" tab - tìm kiếm theo tên lớp/khối
                    vm.FeeClassSearchText = tb.Text?.Trim();
                }
            }
        }
    }

    private void FilterYear_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is TuitionViewModel vm)
        {
            vm.FilterTuitionSummary();
        }
    }

    private void FilterClass_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is TuitionViewModel vm)
        {
            vm.FilterTuitionSummary();
        }
    }

    private void ResetFilter_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is TuitionViewModel vm)
        {
            var searchBox = this.FindControl<TextBox>("SearchBox");
            if (searchBox != null)
                searchBox.Text = string.Empty;

            var tabControl = this.FindControl<TabControl>("TuitionTabControl");
            if (tabControl?.SelectedItem is TabItem selectedTab)
            {
                bool isManageTab = selectedTab.Header?.ToString() == "Quản lý học phí";

                if (isManageTab)
                {
                    // "Quản lý học phí" - reset student filters
                    vm.StudentSearchText = string.Empty;
                    vm.SelectedFilterClassYear = "Tất cả";
                    vm.SelectedFilterClassName = "Tất cả";
                    vm.FilterTuitionSummary();
                }
                else
                {
                    // "Mức học phí" - reset fee class search
                    vm.FeeClassSearchText = string.Empty;
                    vm.FilterFeeClassList();
                }
            }
        }
    }
}
