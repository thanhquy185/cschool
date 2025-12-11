using Avalonia.Controls;
using ViewModels;
using Avalonia.Interactivity;

using Utils;
using System;
using System.Collections.Generic;


namespace Views.Tuition
{
    public partial class TuitionCollectionDialog : Window
    {
        public TuitionCollectionDialog(TuitionViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
        }

        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ======================= THU MỘT THÁNG =======================
        private void CollectOneMonth_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not TuitionViewModel vm)
                return;

            // DataContext của nút nằm trong DataGrid → chính là item đó
            if (sender is Button btn && btn.DataContext != null)
            {
                var monthItem = btn.DataContext;

                // gọi hàm trong ViewModel
                // vm.CollectOneMonthCommand?.Execute(monthItem);
            }
        }

        // ======================= THU TẤT CẢ CÁC THÁNG =======================
        private void ConfirmCollect_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not TuitionViewModel vm)
                return;

            // vm.ConfirmCollectCommand?.Execute(null);
        }


        


        private void MonthComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TuitionViewModel vm)
            {
                if (string.IsNullOrEmpty(vm.SelectedMonth) || vm.SelectedMonth == "Tất cả")
                {
                    // Nếu chưa chọn tháng hoặc chọn "Tất cả" -> hiển thị tổng phí từng tháng
                    vm.LoadFeeMonthsSummary();
                }
                else
                {
                    // Nếu chọn tháng cụ thể -> lọc chi tiết khoản phí
                    vm.FilterTuitionDetailByMonth();
                }
            }
        }

        private async void ExportExcel_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not TuitionViewModel vm)
                return;

#pragma warning disable CS0618
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
    }
}
