using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace cschool.Views.DialogHomeClass;

public partial class HomeClassAddDialog : Window
{
    public HomeClassAddDialog()
    {
        InitializeComponent();
        
        // Đặt sự kiện cho phím tắt
        this.KeyDown += OnWindowKeyDown;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Sự kiện khi click vào option hạnh kiểm
    private void OnConductOptionPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Tag is string conductLevel)
        {
            var viewModel = DataContext as ViewModels.HomeClassViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedConductLevel = conductLevel;
            }
        }
    }

    // Sự kiện phím tắt
    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
        else if (e.Key == Key.Enter && e.KeyModifiers == 0)
        {
            var viewModel = DataContext as ViewModels.HomeClassViewModel;
            if (viewModel != null && viewModel.SaveConductCommand.CanExecute(null))
            {
                viewModel.SaveConductCommand.Execute(null);
            }
        }
    }

    // Sự kiện khi window đóng
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        this.KeyDown -= OnWindowKeyDown;
    }
}