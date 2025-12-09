using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels;
using System;
using Models;

namespace Views.Tuition
{
    public partial class TuitionCreateDialog : Window
    {
        private TuitionViewModel vm;
        public TuitionCreateDialog(TuitionViewModel viewmodel)
        {
            DataContext = viewmodel;
            vm = viewmodel;
            InitializeComponent();
            // Khởi tạo ComboBox mặc định
            NewFeeType.SelectedIndex = 0;
        }

    private void AddFee_Click(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as TuitionViewModel;
        if (vm == null) return;

        // Lấy dữ liệu từ input
        string name = NewFeeName.Text?.Trim() ?? "";
        string type = (NewFeeType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "BASE";
        if (!decimal.TryParse(NewFeeAmount.Text, out decimal amount))
        {
            Console.WriteLine("Số tiền không hợp lệ!");
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Tên phí chưa nhập!");
            return;
        }

        // In ra kiểm tra dữ liệu
        Console.WriteLine($"Thêm phí mới: Name={name}, Type={type}, Amount={amount}");

        // Thêm vào ViewModel
        vm.AddFee(name, type, amount);

        // Clear input
        NewFeeName.Text = "";
        NewFeeAmount.Text = "";
        NewFeeType.SelectedIndex = 0;

        // Focus vào hàng mới
        if (FeeDataGrid.Columns.Count > 0 && vm.FeeTemplateList.Count > 0)
        {
            var firstColumn = FeeDataGrid.Columns[0];
            var lastItem = vm.FeeTemplateList[^1];
            FeeDataGrid.ScrollIntoView(lastItem, firstColumn);
            FeeDataGrid.SelectedItem = lastItem;
        }
    }


    private void EditFee_Click(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var fee = button?.DataContext as FeeTemplateModel;
        if (fee == null) return;

        // Chọn hàng
        FeeDataGrid.SelectedItem = fee;

        // Cho phép edit tạm thời
        FeeDataGrid.IsReadOnly = false;

        // Scroll tới hàng và focus ô đầu tiên
        if (FeeDataGrid.Columns.Count > 0)
        {
            FeeDataGrid.ScrollIntoView(fee, FeeDataGrid.Columns[0]);
            FeeDataGrid.CurrentColumn = FeeDataGrid.Columns[0];
            FeeDataGrid.BeginEdit();
        }
    }





private void DeleteFee_Click(object? sender, RoutedEventArgs e)
{
    var vm = DataContext as TuitionViewModel;
    var fee = (sender as Button)?.DataContext as FeeTemplateModel;
    if (vm != null && fee != null)
        vm.DeleteFee(fee);
}

private void Save_Click(object? sender, RoutedEventArgs e)
{
    var vm = DataContext as TuitionViewModel;
    vm?.SaveCommand.Execute().Subscribe();
}

private void Close_Click(object? sender, RoutedEventArgs e)
{
    this.Close();
}


    }
}
