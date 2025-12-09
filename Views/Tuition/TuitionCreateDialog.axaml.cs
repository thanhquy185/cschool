using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels;
using Models;
using System.Threading.Tasks;
using System;

namespace Views.Tuition
{
    public partial class TuitionCreateDialog : Window
    {
        private readonly TuitionViewModel vm;

        public TuitionCreateDialog(TuitionViewModel viewmodel)
{
    vm = viewmodel;
    DataContext = vm;

    InitializeComponent();

    // Load dữ liệu ngay khi cửa sổ mở xong
    this.Opened += (_, __) =>
    {
        vm.LoadData();
        Console.WriteLine("LoadData chạy! Số phí: " + vm.FeeTemplateList.Count);
    };

    NewFeeType.SelectedIndex = 0;
}


        private void AddFee_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewFeeName.Text))
            {
                Console.WriteLine("Tên phí chưa nhập!");
                return;
            }

            if (!decimal.TryParse(NewFeeAmount.Text, out decimal amount))
            {
                Console.WriteLine("Số tiền không hợp lệ!");
                return;
            }

            string type = (NewFeeType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "BASE";

            vm.AddFee(NewFeeName.Text.Trim(), type, amount);

            NewFeeName.Text = "";
            NewFeeAmount.Text = "";
            NewFeeType.SelectedIndex = 0;

            if (vm.FeeTemplateList.Count > 0)
            {
                var last = vm.FeeTemplateList[^1];
                FeeDataGrid.ScrollIntoView(last, FeeDataGrid.Columns[0]);
                FeeDataGrid.SelectedItem = last;
            }
        }

        private async void EditFee_Click(object? sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not FeeTemplateModel fee)
                return;

            FeeDataGrid.IsReadOnly = false;
            FeeDataGrid.SelectedItem = fee;

            await Task.Delay(10);

            FeeDataGrid.ScrollIntoView(fee, FeeDataGrid.Columns[0]);
            FeeDataGrid.CurrentColumn = FeeDataGrid.Columns[0];
            FeeDataGrid.BeginEdit();

            await Task.Delay(500);
            FeeDataGrid.IsReadOnly = true;
        }

        private void DeleteFee_Click(object? sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is FeeTemplateModel fee)
                vm.DeleteFee(fee);
        }

        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            vm.SaveCommand.Execute().Subscribe();
            Close();
        }
    }
}
