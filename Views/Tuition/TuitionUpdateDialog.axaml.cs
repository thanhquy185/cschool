using Avalonia.Controls;
using ViewModels;
using System;
using System.Linq;
using Avalonia.Interactivity;
using System.Reactive.Threading.Tasks; 
using Utils;
using Models;

namespace Views.Tuition
{
    public partial class TuitionUpdateDialog : Window
    {
        private TuitionViewModel vm;

        public TuitionUpdateDialog(TuitionViewModel viewModel)
        {
            DataContext = viewModel;
            vm = viewModel;
            InitializeComponent();
        }

        // ==================== Khi tick/untick phí ====================
        private void FeeCheckBox_Changed(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
{
    // Học kỳ 1
    decimal totalSelectedFeesHK1 = vm.BaseFees1.Concat(vm.ExtraFees1)
                                        .Where(f => f.IsSelected)
                                        .Sum(f => f.Amount);

    foreach (var month in vm.MonthsHK1)
    {
        System.Console.WriteLine(month.MonthName + " " + month.IsSelected);
        if (month.IsSelected)  // Chỉ update các tháng đang tick
            month.Amount = totalSelectedFeesHK1;
    }

    // Học kỳ 2
    decimal totalSelectedFeesHK2 = vm.BaseFees2.Concat(vm.ExtraFees2)
                                        .Where(f => f.IsSelected)
                                        .Sum(f => f.Amount);

    foreach (var month in vm.MonthsHK2)
    {
        if (month.IsSelected)
            month.Amount = totalSelectedFeesHK2;
    }
}

        // ==================== Khi tick/untick tháng ====================
        private void MonthCheckBox_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is MonthFeeItem month)
            {
                // Chỉ update tháng đang tick
                var fees = vm.MonthsHK1.Contains(month)
                           ? vm.BaseFees1.Concat(vm.ExtraFees1)
                           : vm.BaseFees2.Concat(vm.ExtraFees2);

                month.Amount = month.IsSelected ? fees.Where(f => f.IsSelected).Sum(f => f.Amount) : 0;
            }
        }
        

       


private async void Save(object? sender, RoutedEventArgs e)
{
    try
    {
        // Thực hiện lệnh lưu
        await vm.SaveMonthFeeCommand.Execute().ToTask();

        // Hiện thông báo thành công dùng Util
        await MessageBoxUtil.ShowSuccess("Lưu học phí thành công", owner: this);

        // Đóng dialog
        this.Close();
    }
    catch (Exception ex)
    {
        // Hiện thông báo lỗi dùng Util
        await MessageBoxUtil.ShowError($"Có lỗi xảy ra: {ex.Message}", owner: this);
    }
}


    }
}
