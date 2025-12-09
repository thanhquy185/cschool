using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Services;
using ViewModels;
using Views.Statistical;

namespace Views;

public partial class StatisticalView : UserControl
{
    private StatisticalViewModel _statisticalViewModel { get; set; }

    public StatisticalView()
    {
        InitializeComponent();
        this._statisticalViewModel = new StatisticalViewModel();
        DataContext = this._statisticalViewModel;

        this.DataContextChanged += OnDataContextChanged;

        this.AttachedToVisualTree += StatisticalView_AttachedToVisualTree;

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is StatisticalViewModel vm)
        {
            vm.RequestOpenDetailDialog += Vm_RequestOpenDetailDialog;
            vm.RequestCloseDetailDialog += Vm_RequestCloseDetailDialog;
        }
    }

    private void StatisticalView_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        if (InfoButton != null)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                InfoButton.IsEnabled = this._statisticalViewModel.InfoButtonEnabled;
                Console.WriteLine("++++++++++++++++++" + this._statisticalViewModel.InfoButtonEnabled);

            });
        }
    }

    private async void Vm_RequestOpenDetailDialog(object? sender, EventArgs e)
    {
        if (DataContext is StatisticalViewModel vm)
        {
            var dialog = new StatisticalDetailDialog
            {
                DataContext = vm
            };

            await dialog.ShowDialog((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
        }
    }

    // Đóng Dialog
    private void Vm_RequestCloseDetailDialog(object? sender, EventArgs e)
    {
        // Tìm dialog đang mở, rồi đóng
        var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        if (desktop is null) return;

        var dialog = desktop.Windows
            .OfType<StatisticalDetailDialog>()
            .FirstOrDefault(w => w.DataContext == sender);

        dialog?.Close();
    }

    private void GpaChart_DataPointerDown(object? sender, object? e)
    {
        try
        {
            // e is likely an instance of LiveChartsCore.Kernel.Events.ChartPointerEventArgs or similar.
            // We'll use dynamic/reflection to extract the clicked ChartPoint and its Series.Name.
            dynamic evt = e ?? throw new ArgumentNullException(nameof(e));

            // Some versions expose property "ChartPoint" or "Point". Try common names:
            dynamic chartPoint = null;
            if (HasProperty(evt, "ChartPoint")) chartPoint = evt.ChartPoint;
            else if (HasProperty(evt, "Point")) chartPoint = evt.Point;
            else chartPoint = evt; // maybe evt already is the chart point

            if (chartPoint == null) return;

            // chartPoint.Series is the series object; get its Name
            string category = null;
            try
            {
                var series = chartPoint.Series;
                category = series?.Name as string;
            }
            catch
            {
                // fallback: try property "SeriesName" or "Label"
                try
                {
                    category = chartPoint.SeriesName as string;
                }
                catch
                {
                    // give up
                    category = null;
                }
            }

            if (string.IsNullOrWhiteSpace(category)) return;

            if (this.DataContext is StatisticalViewModel vm)
            {
                // Call ViewModel to load students for the clicked GPA category
                vm.LoadStudentsForGpaCategory(category);
            }
        }
        catch
        {
            // swallow; non-fatal
        }
    }

    private static bool HasProperty(object obj, string propName)
    {
        if (obj == null) return false;
        var t = obj.GetType();
        return t.GetProperty(propName) != null;
    }
}