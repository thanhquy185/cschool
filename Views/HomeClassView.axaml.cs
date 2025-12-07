using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using ViewModels;
using Views.DialogHomeClass;

namespace Views;

public partial class HomeClassView : UserControl
{
    public HomeClassView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is HomeClassViewModel vm)
            {
                vm.RequestShowStudentDetail += Vm_RequestShowStudentDetail;
                vm.RequestAddConduct += Vm_RequestAddConduct;
                vm.RequestCancelConduct += Vm_RequestCancelConduct;
            }
        };
    }

    private async void Vm_RequestShowStudentDetail(object? sender, Models.HomeClass selected)
    {
        if (DataContext is not HomeClassViewModel vm || selected == null) return;

        vm.SelectedStudent = selected;

        var param = selected.StudentId;

        if (vm.LoadStudentDetailScoresCommand is IAsyncRelayCommand<int> asyncCmd)
        {
            if (asyncCmd.CanExecute(param))
                await asyncCmd.ExecuteAsync(param); // await nếu cần
        }
        else if (vm.LoadStudentDetailScoresCommand.CanExecute(param))
        {
            vm.LoadStudentDetailScoresCommand.Execute(param);
        }

        var dialog = new HomeClassDetailDialog
        {
            DataContext = vm
        };

        var owner = this.VisualRoot as Window
                    ?? (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        await dialog.ShowDialog(owner);
    }

    // 🔥 Handler mới cho dialog hạnh kiểm
    private async void Vm_RequestAddConduct(object? sender, EventArgs e)
    {
        if (DataContext is HomeClassViewModel vm)
        {
            var dialog = new HomeClassAddDialog
            {
                DataContext = vm
            };

            await dialog.ShowDialog((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
        }
    }

    // Đóng Dialog
    private async void Vm_RequestCancelConduct(object? sender, EventArgs e)
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;

        var dialog = mainWindow?.OwnedWindows
            .OfType<HomeClassAddDialog>()
            .FirstOrDefault();

        dialog?.Close();
    }
}
