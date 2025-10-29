using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView; // PieSeries<T>
using LiveChartsCore.SkiaSharpView.Avalonia; // PieChart control (for xaml)
using LiveChartsCore.Kernel; // for ChartPoint type used by formatter (may be optional)
using SkiaSharp;
using cschool.Services;
using cschool.Models;
using LiveChartsCore.SkiaSharpView.Painting;

namespace cschool.ViewModels
{
    public partial class StatisticalViewModel : ViewModelBase
    {
        private readonly StatisticalService _statService;

        // Series collections bound to the PieCharts
        [ObservableProperty]
        private ObservableCollection<ISeries> academicSeries = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> conductSeries = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> gpaSeries = new();

        // Labels (optional) to show totals on UI if you want
        [ObservableProperty]
        private string academicSummary = "";

        [ObservableProperty]
        private string conductSummary = "";

        [ObservableProperty]
        private string gpaSummary = "";

        // Selected students to display on right panel (when clicking GPA slice)
        [ObservableProperty]
        private ObservableCollection<Statistical> selectedStudents = new();

        [ObservableProperty]
        private string selectedGroupHeader = "";

        public IAsyncRelayCommand LoadDataCommand { get; }

        // Keep raw stats so we can filter quickly on click
        private List<Statistical> _rawStats = new();

        public StatisticalViewModel(StatisticalService statService)
        {
            _statService = statService ?? throw new ArgumentNullException(nameof(statService));
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);

            // auto-load once constructed
            _ = LoadDataAsync();
        }

        private Task LoadDataAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    var stats = _statService.GetStatistics() ?? new List<Statistical>();
                    _rawStats = stats;

                    // Academic counts (Good/Fair/Satisfactory)
                    var academicGroups = stats
                        .GroupBy(s => (s.Academic ?? "").Trim())
                        .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

                    // Conduct counts
                    var conductGroups = stats
                        .GroupBy(s => (s.ConductLevel ?? "").Trim())
                        .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

                    // GPA classification according to rules:
                    // Good: gpa >= 8
                    // Fair: gpa >= 6.5 && < 8
                    // Satisfactory: gpa >= 5 && < 6.5
                    int gpaGood = stats.Count(s => s.Gpa >= 8f);
                    int gpaFair = stats.Count(s => s.Gpa >= 6.5f && s.Gpa < 8f);
                    int gpaSat = stats.Count(s => s.Gpa >= 5f && s.Gpa < 6.5f);

                    // Totals
                    int totalAcademic = academicGroups.Values.Sum();
                    int totalConduct = conductGroups.Values.Sum();
                    int totalGpa = gpaGood + gpaFair + gpaSat;

                    // Categories canonical order
                    string[] canonical = new[] { "Good", "Fair", "Satisfactory" };

                    var academicCounts = canonical.ToDictionary(k => k, k => academicGroups.ContainsKey(k) ? academicGroups[k] : 0);
                    var conductCounts = canonical.ToDictionary(k => k, k => conductGroups.ContainsKey(k) ? conductGroups[k] : 0);
                    var gpaCounts = new Dictionary<string, int>
                    {
                        ["Good"] = gpaGood,
                        ["Fair"] = gpaFair,
                        ["Satisfactory"] = gpaSat
                    };

                    // Build series using helper
                    var academicSeriesLocal = BuildSeriesFromCounts(academicCounts, totalAcademic, canonical);
                    var conductSeriesLocal = BuildSeriesFromCounts(conductCounts, totalConduct, canonical);
                    var gpaSeriesLocal = BuildSeriesFromCounts(gpaCounts, totalGpa, canonical, isGpa:true);

                    // Put summaries
                    var acSum = $"Total: {totalAcademic}";
                    var coSum = $"Total: {totalConduct}";
                    var gpSum = $"Total classified: {totalGpa}";

                    // Update UI thread
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        AcademicSeries = academicSeriesLocal;
                        ConductSeries = conductSeriesLocal;
                        GpaSeries = gpaSeriesLocal;

                        AcademicSummary = acSum;
                        ConductSummary = coSum;
                        GpaSummary = gpSum;

                        SelectedStudents = new ObservableCollection<Statistical>();
                        SelectedGroupHeader = "Chưa chọn";
                    });
                }
                catch (Exception ex)
                {
                    var empty = new ObservableCollection<ISeries>
                    {
                        new PieSeries<double> { Name="No data", Values = new double[]{1} }
                    };

                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        AcademicSeries = empty;
                        ConductSeries = empty;
                        GpaSeries = empty;
                        AcademicSummary = $"Error: {ex.Message}";
                        ConductSummary = "";
                        GpaSummary = "";
                    });
                }
            });
        }

        /// <summary>
        /// Build PieSeries collection from counts.
        /// For GPA series we set DataLabelsFormatter to show "count (percent%)".
        /// </summary>
        private ObservableCollection<ISeries> BuildSeriesFromCounts(
            IDictionary<string, int> counts,
            int total,
            string[] orderPreferred = null,
            bool isGpa = false)
        {
            var series = new ObservableCollection<ISeries>();

            string[] categories = orderPreferred ?? counts.Keys.ToArray();

            foreach (var cat in categories)
            {
                counts.TryGetValue(cat, out int cnt);
                double value = cnt;
                double pct = total > 0 ? Math.Round((double)cnt / total * 100.0, 1) : 0.0;

                var pie = new PieSeries<double>
                {
                    Name = string.IsNullOrWhiteSpace(cat) ? "(Unknown)" : cat,
                    Values = new double[] { value },

                    // enable labels (paint) and set position
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black) ,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer
                };

               
                    // DataLabelsFormatter expects a delegate that receives a ChartPoint.
                    // To avoid strong coupling to exact ChartPoint type names across versions,
                    // we provide a lambda that uses dynamic for formatting.
                    pie.DataLabelsFormatter = (point) =>
                    {
                        try
                        {
                            // point.PrimaryValue should be the numeric value (count)
                            // point.Context may contain share but not guaranteed: so compute percent ourselves.
                            double count = 0;
                            string labelName = cat;
                            // Try to get PrimaryValue (works in v2)
                            var primary = ((dynamic)point).PrimaryValue;
                            count = Convert.ToDouble(primary);
                            // Use pct computed above:
                            return $"{pct}%";
                        }
                        catch
                        {
                            return $"{pct}%";
                        }
                    };
                

                series.Add(pie);
            }

            return series;
        }

        /// <summary>
        /// Public method to be called from the view code-behind when a GPA slice is clicked.
        /// category is expected to be "Good" | "Fair" | "Satisfactory"
        /// </summary>
        public void LoadStudentsForGpaCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return;

            // filter raw stats by GPA classification according to same rules
            IEnumerable<Statistical> list = _rawStats.Where(s =>
            {
                if (category.Equals("Good", StringComparison.OrdinalIgnoreCase)) return s.Gpa >= 8f;
                if (category.Equals("Fair", StringComparison.OrdinalIgnoreCase)) return s.Gpa >= 6.5f && s.Gpa < 8f;
                if (category.Equals("Satisfactory", StringComparison.OrdinalIgnoreCase)) return s.Gpa >= 5f && s.Gpa < 6.5f;
                return false;
            });

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                SelectedStudents = new ObservableCollection<Statistical>(list);
                SelectedGroupHeader = $"GPA: {category} — {SelectedStudents.Count} học sinh";
            });
        }
    }
}
