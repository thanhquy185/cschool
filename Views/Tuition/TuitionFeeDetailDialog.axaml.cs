using Avalonia.Controls;
using ViewModels;
using Avalonia.Interactivity;
using System.IO;
using System;
using System.Linq;
using Utils;
using System.Collections.Generic;

namespace Views.Tuition
{
    public partial class TuitionFeeDetailDialog : Window
    {
        public TuitionFeeDetailDialog(TuitionViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
            
            // Khởi tạo danh sách học kỳ với "Tất cả"
            viewmodel.TermDisplayList.Clear();
            viewmodel.TermDisplayList.Add("Tất cả");
            viewmodel.TermDisplayList.Add("Học kỳ 1");
            viewmodel.TermDisplayList.Add("Học kỳ 2");
            
            // Set mặc định chọn "Tất cả"
            viewmodel.SelectedTermDisplay = "Tất cả";
            viewmodel.SelectedMonth = "Tất cả";
            
            // Hiển thị tổng tiền theo tháng ngay khi mở
            viewmodel.LoadFeeMonthsSummary();
        }

        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MonthComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TuitionViewModel vm)
            {
                if (vm.SelectedMonth == "Tất cả" || string.IsNullOrEmpty(vm.SelectedMonth))
                {
                    // Hiển thị tổng phí từng tháng
                    vm.LoadFeeMonthsSummary();
                }
                else
                {
                    // Lọc chi tiết phí của tháng được chọn
                    vm.FilterTuitionDetailByMonth();
                }
            }
        }

        private async void ExportExcel_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
#pragma warning disable CS0618
                var dialog = new SaveFileDialog
                {
                    Title = "Chọn nơi lưu file Excel",
                    Filters = new List<FileDialogFilter>
                    {
                        new FileDialogFilter { Name = "Excel Files", Extensions = { "xlsx" } }
                    },
                    InitialFileName = $"ChiTietHocPhi_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
#pragma warning restore CS0618

                var filePath = await dialog.ShowAsync(this);

                if (string.IsNullOrWhiteSpace(filePath))
                    return;

                if (DataContext is TuitionViewModel vm)
                {
                    // TODO: Gọi service export Excel thực tế
                    await MessageBoxUtil.ShowSuccess($"Xuất file Excel thành công!\n\nFile: {filePath}", owner: this);
                }
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowError($"Lỗi xuất Excel: {ex.Message}", owner: this);
            }
        }
    }
}


