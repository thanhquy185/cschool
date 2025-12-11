using Avalonia.Controls;
using ViewModels;
using Avalonia.Interactivity;
using System.Linq;
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Layout;
using Models;
using Utils;
using Services;

namespace Views.Tuition
{
    public partial class TuitionInfoDialog : Window
    {
        public TuitionInfoDialog(TuitionViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;

            // Khởi tạo danh sách học kỳ
            viewmodel.TermDisplayList.Clear();
            viewmodel.TermDisplayList.Add("Tất cả");
            viewmodel.TermDisplayList.Add("Học kỳ 1");
            viewmodel.TermDisplayList.Add("Học kỳ 2");
            viewmodel.SelectedTermDisplay = "Tất cả";

            // Reset SelectedMonth để hiển thị tổng hợp
            viewmodel.SelectedMonth = "Tất cả";
            viewmodel.LoadFeeMonthsSummary();
        }
        
         private async void ExportExcel_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not TuitionViewModel vm)
                return;

            var dialog = new SaveFileDialog
            {
                Title = "Chọn nơi lưu file Excel",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Excel Files", Extensions = { "xlsx" } }
                },
                InitialFileName = "ChiTietHocPhi.xlsx"
            };

            var owner = TopLevel.GetTopLevel(this) as Window;
            var filePath = await dialog.ShowAsync(owner ?? this);

            if (string.IsNullOrWhiteSpace(filePath))
                return;

            try
            {
                await vm.ExportExcel(filePath);
                await MessageBoxUtil.ShowSuccess("Xuất file Excel thành công!\n", owner: owner);
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowError(ex.Message, owner: owner);
            }

        }
        private void Close_Click(object? sender, RoutedEventArgs e) => this.Close();

        private void MonthComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TuitionViewModel vm)
            {
                if (string.IsNullOrEmpty(vm.SelectedMonth) || vm.SelectedMonth == "Tất cả")
                    vm.LoadFeeMonthsSummary();
                else
                    vm.FilterTuitionDetailByMonth();
            }
        }
        

private async void Collection_Click(object? sender, RoutedEventArgs e)
{
    if (DataContext is not TuitionViewModel vm)
        return;

    string selectedItemText = MonthComboBox.SelectedItem?.ToString() ?? "";

    if (selectedItemText.Contains("Tháng"))
    {
        // Xử lý từng tháng như hiện tại
        var match = System.Text.RegularExpressions.Regex.Match(selectedItemText, @"\d+");
        if (!match.Success) return;

        int monthNumber = int.Parse(match.Value);

        var assignClassItem = vm.TuitionList
            .FirstOrDefault(t => t.StudentId == vm.SelectedStudent.StudentId
                              && t.MonthId == monthNumber
                              && t.ClassYear == vm.SelectedStudent.ClassYear);

        if (assignClassItem != null && !assignClassItem.IsPaid)
        {
            var dialog = new CollectDialog(
                studentName: vm.SelectedStudent.StudentName,
                monthName: $"Tháng {monthNumber}",
                totalAmount: assignClassItem.TotalAmount,
                collectorName: SessionService.currentUserLogin.Fullname,
                studentId: vm.SelectedStudent.StudentId,
                assignClassId: assignClassItem.AssignClassId,
                monthId: monthNumber
            );

            var result = await dialog.ShowDialog<bool>(this);
            if (result)
                vm.LoadFeeMonthsSummary();
        }
        else
        {
            Console.WriteLine($"Không tìm thấy AssignClassId hoặc đã thu tiền cho StudentId: {vm.SelectedStudent.StudentId}, Month: {monthNumber}");
        }
    }
    else if (selectedItemText.Contains("Tất cả"))
    {
        // Xử lý thu tất cả các tháng chưa đóng
        var unpaidItems = vm.TuitionList
            .Where(t => t.StudentId == vm.SelectedStudent.StudentId && !t.IsPaid)
            .ToList();

        foreach (var item in unpaidItems)
        {
            var dialog = new CollectDialog(
                studentName: vm.SelectedStudent.StudentName,
                monthName: $"Tháng {item.MonthId}",
                totalAmount: item.TotalAmount,
                collectorName: SessionService.currentUserLogin.Fullname,
                studentId: vm.SelectedStudent.StudentId,
                assignClassId: item.AssignClassId,
                monthId: item.MonthId
            );

            var result = await dialog.ShowDialog<bool>(this);
            if (!result) break; // Nếu người dùng hủy, dừng thu các tháng còn lại
        }

        // Cập nhật lại dữ liệu sau khi thu xong tất cả
        vm.LoadFeeMonthsSummary();
    }
}

    }

public class CollectDialog : Window
{
    public TextBox AmountBox;
    public ComboBox MethodBox;
    public TextBox NoteBox;
    public TextBox CollectorBox;

    private string _studentName;
    private string _monthName;
    private decimal _totalAmount;
    private string _collectorName;

    private int _studentId;
    private int _assignClassId;
    private int _monthId;

    public CollectDialog(string studentName, string monthName, decimal totalAmount, string collectorName,
                         int studentId, int assignClassId, int monthId)
    {
        _studentName = studentName;
        _monthName = monthName;
        _totalAmount = totalAmount;
        _collectorName = collectorName;

        _studentId = studentId;
        _assignClassId = assignClassId;
        _monthId = monthId;

        Width = 450;
        Height = 500;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Title = "Thu học phí";

        Content = CreateUI();
    }

    private Control CreateUI()
    {
        var confirmButton = new Button
        {
            Content = "Xác nhận",
            Width = 120,
            Background = Brushes.Green,
            Foreground = Brushes.White
        };
        confirmButton.Click += (s, e) => OnConfirmClicked();

        return new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = "Thu học phí",
                    FontSize = 24,
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                },

                new Border
                {
                    Background = Brushes.LightGray,
                    Padding = new Thickness(10),
                    CornerRadius = new CornerRadius(6),
                    Child = new StackPanel
                    {
                        Spacing = 4,
                        Children =
                        {
                            new TextBlock { Text = $"Học sinh: {_studentName}" },
                            new TextBlock { Text = $"Tháng: {_monthName}" },
                            new TextBlock { Text = $"Số tiền phải thu: {_totalAmount:n0} đ" }
                        }
                    }
                },

                new StackPanel
                {
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock { Text = "Người thu:" },
                        (CollectorBox = new TextBox { Text = _collectorName, IsReadOnly = true })
                    }
                },

                new StackPanel
                {
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock { Text = "Số tiền thu:" },
                        (AmountBox = new TextBox { Text = _totalAmount.ToString("N0") })
                    }
                },

                new StackPanel
                {
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock { Text = "Hình thức thanh toán:" },
                        (MethodBox = new ComboBox
                        {
                            ItemsSource = new[] { "Tiền mặt", "Chuyển khoản", "Ví điện tử" },
                            SelectedIndex = 0
                        })
                    }
                },

                new StackPanel
                {
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock { Text = "Ghi chú:" },
                        (NoteBox = new TextBox { Height = 80, AcceptsReturn = true })
                    }
                },

                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    Spacing = 12,
                    Children =
                    {
                        new Button
                        {
                            Content = "Hủy",
                            Width = 100,
                            Background = Brushes.Gray,
                            Foreground = Brushes.White,
                            Command = ReactiveUI.ReactiveCommand.Create(() => Close(false))
                        },
                        confirmButton
                    }
                }
            }
        };
    }

    private async void OnConfirmClicked()
    {
        string method = MethodBox.SelectedItem?.ToString() ?? "";
        string note = NoteBox.Text;
        decimal amount = decimal.TryParse(AmountBox.Text.Replace(",", ""), out var val) ? val : _totalAmount;

        // In ra console để kiểm tra
        Console.WriteLine("=== Dữ liệu thu học phí ===");
        Console.WriteLine($"StudentId: {_studentId}");
        Console.WriteLine($"AssignClassId: {_assignClassId}");
        Console.WriteLine($"MonthId: {_monthId}");
        Console.WriteLine($"Amount: {amount}");
        Console.WriteLine($"PaymentMethod: {method}");
        Console.WriteLine($"Note: {note}");
        Console.WriteLine($"Collector: {_collectorName}");
        Console.WriteLine("===========================");

       try
        {
            // Gọi service đồng bộ
            bool result = AppService.TuitionService.CollectFee(
                studentId: _studentId,
                assignClassId: _assignClassId,
                monthId: _monthId,
                amount: amount,
                paymentMethod: method,
                note: note,
                collectorId: SessionService.currentUserLogin.Id
            );

            if (result)
                await MessageBoxUtil.ShowSuccess("Thu học phí thành công!", owner: this);
            else
                await MessageBoxUtil.ShowError("Không tìm thấy bản ghi để thu học phí!", owner: this);
        }
        catch (Exception ex)
        {
            await MessageBoxUtil.ShowError($"Lỗi: {ex.Message}", owner: this);
        }


        Close(true);
    }
}

}
