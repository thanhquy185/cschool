using Avalonia.Controls;
using ViewModels;
using System;
using System.Linq;
using Avalonia.Interactivity;
using System.Reactive.Threading.Tasks; 
using Utils;
using Models;
using DocumentFormat.OpenXml.Drawing.Diagrams;

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
            // If sender is a CheckBox inside the DataGrid, ensure the model reflects its state immediately
            if (sender is CheckBox cb && cb.DataContext is FeeTemplateModel ftModel)
            {
                ftModel.IsSelected = cb.IsChecked ?? false;
            }

            // Học kỳ 1: compute based on model state (now synced)
            decimal totalSelectedFeesHK1 = vm.BaseFees1.Concat(vm.ExtraFees1)
                                                .Where(f => f.IsSelected)
                                                .Sum(f => f.Amount);

            foreach (var month in vm.MonthsHK1)
            {
                System.Console.WriteLine(month.MonthName + " " + month.IsSelected);
                if (month.IsSelected)
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

            // Diagnostic: print fee selection states and computed totals
            System.Console.WriteLine("[DEBUG] FeeCheckBox_Changed: HK1 fees:");
            foreach (var f in vm.BaseFees1.Concat(vm.ExtraFees1))
            {
                System.Console.WriteLine($"  Fee: {f.Name}, Id={f.Id}, IsSelected={f.IsSelected}, Amount={f.Amount}");
            }
            System.Console.WriteLine($"  Computed total HK1 = {totalSelectedFeesHK1}");
            System.Console.WriteLine("[DEBUG] FeeCheckBox_Changed: HK2 fees:");
            foreach (var f in vm.BaseFees2.Concat(vm.ExtraFees2))
            {
                System.Console.WriteLine($"  Fee: {f.Name}, Id={f.Id}, IsSelected={f.IsSelected}, Amount={f.Amount}");
            }
            System.Console.WriteLine($"  Computed total HK2 = {totalSelectedFeesHK2}");
        }

        // ==================== Khi tick/untick tháng ====================
    
       
        private void MonthHK1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            
            if (sender is not DataGrid dg) return;
            if (dg.SelectedItem is not MonthFeeItem selectedItem) return;
            System.Console.WriteLine("Selected HK1 month: " + selectedItem.MonthName);
            foreach (var month in vm.MonthsHK1)
            {
                System.Console.WriteLine(month.MonthName + " " + month.IsSelected);
                if (month.MonthId == selectedItem.MonthId)
                {
                    month.IsSelected = true;
                }else
                {
                    month.IsSelected = false;
                }
               
            }
            
            
            vm.LoadSelectedFeeForMonth(selectedItem);
        }

        private void MonthHK2_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is not DataGrid dg) return;
            if (dg.SelectedItem is not MonthFeeItem selectedItem) return;
                        foreach (var month in vm.MonthsHK2)
            {
                System.Console.WriteLine(month.MonthName + " " + month.IsSelected);
                if (month.MonthId == selectedItem.MonthId)
                {
                    month.IsSelected = true;
                }
                else
                {
                    month.IsSelected = false;
                }
               
            }

            // Load fee data for the selected month (don't reset month or fee selections)
            vm.LoadSelectedFeeForMonth(selectedItem);
        }
private async void Save(object? sender, RoutedEventArgs e)
{
    try
    {
        // Ensure any in-progress checkbox edits commit by moving focus away
        try
        {
            this.Focus();
            // small delay to let bindings update
            await System.Threading.Tasks.Task.Delay(60);
        }
        catch { }

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
