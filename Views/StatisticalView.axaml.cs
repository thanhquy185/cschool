using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using cschool.Services;
using cschool.ViewModels;
using cschool.Views.DialogAssignTeacher;

namespace cschool.Views;

public partial class StatisticalView : UserControl
{
    // AssignTeacherViewModel ViewModel => DataContext as AssignTeacherViewModel;
    public StatisticalView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
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