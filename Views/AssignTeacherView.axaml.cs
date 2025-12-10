using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Models;
using Services;
using Utils;
using ViewModels;
using Views.DialogAssignTeacher;

namespace Views;

public partial class AssignTeacherView : UserControl
{
    // AssignTeacherViewModel ViewModel => DataContext as AssignTeacherViewModel;
    // private static AssignTeacherViewModel _assignTeacherViewModel { get; set; } = new AssignTeacherViewModel();
     private AssignTeacherViewModel _assignTeacherViewModel { get; set; }

    public AssignTeacherView()
    {
        InitializeComponent();
        this._assignTeacherViewModel = new AssignTeacherViewModel();
        DataContext = _assignTeacherViewModel;

        // Subscribe events ở đây (chạy chỉ một lần)
        if (DataContext is AssignTeacherViewModel vm)
        {
            vm.RequestOpenDetailDialog += Vm_RequestOpenDetailDialog;
            vm.RequestOpenEditDialog += Vm_RequestOpenEditDialog;
            vm.RequestCloseAddDialog += Vm_RequestCloseAddAssignTeacher;
            vm.RequestCloseEditDialog += Vm_RequestCloseEditAssignTeacher;
        }

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (SessionService.currentUserLogin != null && AppService.RoleDetailService != null)
            {
                if (InfoButton != null)
                    InfoButton.IsEnabled = this._assignTeacherViewModel.InfoButtonEnabled;

                if (CreateButton != null)
                    CreateButton.IsEnabled = this._assignTeacherViewModel.CreateButtonEnabled;

                if (UpdateButton != null)
                    UpdateButton.IsEnabled = this._assignTeacherViewModel.UpdateButtonEnabled;

                if (LockButton != null)
                    LockButton.IsEnabled = this._assignTeacherViewModel.LockButtonEnabled;
            }
        });

        this.AttachedToVisualTree += OnAttachedToVisualTree_Handler;
        this.DetachedFromVisualTree += OnDetachedFromVisualTree_Handler;


    }

    private void OnAttachedToVisualTree_Handler(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is AssignTeacherViewModel vm)
        {
            // Unsub trước để safe, tránh duplicate handler
            vm.RequestOpenDetailDialog -= Vm_RequestOpenDetailDialog;
            vm.RequestOpenDetailDialog += Vm_RequestOpenDetailDialog;

            vm.RequestOpenEditDialog -= Vm_RequestOpenEditDialog;
            vm.RequestOpenEditDialog += Vm_RequestOpenEditDialog;

            vm.RequestCloseAddDialog -= Vm_RequestCloseAddAssignTeacher;
            vm.RequestCloseAddDialog += Vm_RequestCloseAddAssignTeacher;

            vm.RequestCloseEditDialog -= Vm_RequestCloseEditAssignTeacher;
            vm.RequestCloseEditDialog += Vm_RequestCloseEditAssignTeacher;

        }
    }

    private void OnDetachedFromVisualTree_Handler(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is AssignTeacherViewModel vm)
        {
            vm.RequestOpenDetailDialog -= Vm_RequestOpenDetailDialog;
            vm.RequestOpenEditDialog -= Vm_RequestOpenEditDialog;
            vm.RequestCloseAddDialog -= Vm_RequestCloseAddAssignTeacher;
            vm.RequestCloseEditDialog -= Vm_RequestCloseEditAssignTeacher;

        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private async void OnAddButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AssignTeacherViewModel vm)
        {
            vm.ClearForm();
            var dialog = new AssignTeacherAddDialog
            {
                DataContext = vm   // ✅ GÁN VM CHO DIALOG
            };

            var window = (Window)this.VisualRoot!;
            var result = await dialog.ShowDialog<bool>(window);

            if (result)
            {
                // vm.LoadDataCommand.Execute(null);
            }
        }
    }
    private async void OnDetailButtonClick(object? sender, RoutedEventArgs e)
    {

        if (DataContext is AssignTeacherViewModel vm)

        {
            if (vm.SelectedAssignTeacher != null)
            {
                // DataContext = vm;
                await vm.OpenDetailDialogCommand.ExecuteAsync(vm.SelectedAssignTeacher);
            }
            else
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn 1 dòng để xem chi tiết", null);
                return;
            }

        }

    }

    private async void Vm_RequestOpenDetailDialog(object? sender, AssignTeacher a)
    {
        if (DataContext is AssignTeacherViewModel vm)
        {
            var dialog = new AssignTeacherDetailDialog
            {
                DataContext = vm   // vẫn dùng VM chính
            };

            // var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            // await dialog.ShowDialog(owner);
            var owner = this.VisualRoot as Window;
            if (owner == null)
            {
                Console.WriteLine("Error: Owner window not found!");
                await MessageBoxUtil.ShowError("Không tìm thấy cửa sổ chính!", null);
                return;
            }

            await dialog.ShowDialog(owner);
            }
    }


    private async void OnEditButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AssignTeacherViewModel vm)
        {
            if (vm.SelectedAssignTeacher != null)
            {
                await vm.OpenEditDialogCommand.ExecuteAsync(vm.SelectedAssignTeacher);
            }
            else
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn 1 dòng để sửa", null);
                return;
            }

        }
    }

    private async void Vm_RequestOpenEditDialog(object? sender, AssignTeacher a)
    {
        try
        {
            if (DataContext is AssignTeacherViewModel vm)
            {
                var dialog = new AssignTeacherEditDialog
                {
                    DataContext = vm
                };

                // var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

                // await dialog.ShowDialog(owner);
                var owner = this.VisualRoot as Window;
                if (owner == null)
                {
                    Console.WriteLine("Error: Owner window not found!");
                    await MessageBoxUtil.ShowError("Không tìm thấy cửa sổ chính!", null);
                    return;
                }

                await dialog.ShowDialog(owner);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error opening dialog: " + ex.Message);
            await MessageBoxUtil.ShowError("Lỗi mở dialog: " + ex.Message, null);
        }
    }

    private void SubjectCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = DataContext as AssignTeacherViewModel;
        vm?.SearchNameSubjectCommand.Execute(null);
    }

    // Đóng Dialog
    private async void Vm_RequestCloseAddAssignTeacher(object? sender, EventArgs e)
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;

        var dialog = mainWindow?.OwnedWindows
            .OfType<AssignTeacherAddDialog>()
            .FirstOrDefault();

        dialog?.Close();

        if (DataContext is AssignTeacherViewModel vm)
            vm.SelectedAssignTeacher = null;
    }

    private async void Vm_RequestCloseEditAssignTeacher(object? sender, EventArgs e)
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;

        var dialog = mainWindow?.OwnedWindows
            .OfType<AssignTeacherEditDialog>()
            .FirstOrDefault();

        dialog?.Close();

        if (DataContext is AssignTeacherViewModel vm)
            vm.SelectedAssignTeacher = null;
    }
}
