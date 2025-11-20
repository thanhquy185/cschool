using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.Kernel;
using SkiaSharp;
using cschool.Services;
using cschool.Models;
using LiveChartsCore.SkiaSharpView.Painting;
using Avalonia.Threading;

namespace cschool.ViewModels;

public partial class StatisticalViewModel : ViewModelBase
{
    private readonly StatisticalService _statService;

    // Series collections bound to the PieCharts
    [ObservableProperty]
    private ObservableCollection<ISeries> academicSeries = new();
    
    [ObservableProperty]
    // biến lưu tất cả các kì học
    private ObservableCollection<Term> terms = new(); 

    [ObservableProperty]
    private ObservableCollection<ISeries> conductSeries = new();

    [ObservableProperty]
    private ObservableCollection<ISeries> gpaSeries = new();

    // Labels (optional) to show totals on UI if you want
    [ObservableProperty]
    private string academicSummary = "";

    [ObservableProperty]
    private Term? selectedTerm;

    [ObservableProperty]
    private string conductSummary = "";

    [ObservableProperty]
    private string gpaSummary = "";

    [ObservableProperty]
    private string totalStudents = "";
    [ObservableProperty]
    private string totalTeacher = "";
    [ObservableProperty]
    private string totalClass = "";
    [ObservableProperty]
    private string totalSubject = "";

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

    [RelayCommand]
    public void ResetSearch()
    {
        LoadDataCommand.Execute(null);
    }

    private Task LoadDataAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                var stats = _statService.GetStatistics() ?? new List<Statistical>();
                _rawStats = stats;

                int total1 = _statService.GetCountStudents();
                int total2 = _statService.GetCountTeacher();
                int total3 = _statService.GetCountClass();
                int total4 = _statService.GetCountSubject();

                totalStudents = " " + total1;
                totalTeacher = " " + total2;
                totalClass = " " + total3;
                totalSubject = " " + total4;

                // Load danh sách kỳ học
                var termList = _statService.GetTerms();
                Terms.Clear(); // Xóa dữ liệu cũ trước khi thêm mới
                foreach (var t in termList)
                {
                    Terms.Add(t);
                }

                // Tự động chọn kỳ học đầu tiên nếu có
                if (Terms.Count > 0 && SelectedTerm == null)
                {
                    SelectedTerm = Terms[0];
                }

                UpdateChartData(stats);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        });
    }

    [RelayCommand]
    public void SearchByTerm()
    {
        if (SelectedTerm == null)
        {
            // Nếu không chọn term nào, hiển thị tất cả dữ liệu
            UpdateChartData(_rawStats);
            return;
        }

        var results = _statService.SearchStatistics(SelectedTerm.Id);
        Dispatcher.UIThread.Post(() =>
        {
            // Cập nhật dữ liệu thống kê theo term
            var filteredStats = results?.ToList() ?? new List<Statistical>();
            UpdateChartData(filteredStats);
        });
    }
    
    // Tự động lọc khi SelectedTerm thay đổi
       partial void OnSelectedTermChanged(Term? value)
    {
        
        SearchByTerm();
    }

    // Hàm mới để cập nhật dữ liệu biểu đồ
    private void UpdateChartData(List<Statistical> stats)
    {
        try
        {
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
            var gpaSeriesLocal = BuildSeriesFromCounts(gpaCounts, totalGpa, canonical, isGpa: true);

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
                SelectedGroupHeader = SelectedTerm != null ? 
                    $"Đang hiển thị: {SelectedTerm.Name}" : 
                    "Đang hiển thị: Tất cả kỳ học";
            });
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    // Hàm xử lý lỗi
    private void HandleError(Exception ex)
    {
        var empty = new ObservableCollection<ISeries>
        {
            new PieSeries<double> { Name = "No data", Values = new double[] { 1 } }
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
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer
            };

            pie.DataLabelsFormatter = (point) =>
            {
                try
                {
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

        // Lọc theo term nếu có chọn
        IEnumerable<Statistical> filteredStats = _rawStats;
        if (SelectedTerm != null)
        {
            filteredStats = _rawStats.Where(s => s.Term_id == SelectedTerm.Id);
        }

        // filter raw stats by GPA classification according to same rules
        IEnumerable<Statistical> list = filteredStats.Where(s =>
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