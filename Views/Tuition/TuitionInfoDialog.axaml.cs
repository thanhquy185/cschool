using Avalonia.Controls;
using ViewModels;
using Avalonia.Interactivity;

namespace Views.Tuition
{
    public partial class TuitionInfoDialog : Window
    {
        public TuitionInfoDialog(TuitionViewModel viewmodel)
        {   
            InitializeComponent();
            DataContext = viewmodel; // truyền viewmodel vào để dialog bind dữ liệu
        }

                private void Close_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MonthComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    if (DataContext is TuitionViewModel vm)
    {
        if (string.IsNullOrEmpty(vm.SelectedMonth))
        {
            // Nếu chưa chọn tháng -> hiển thị tổng hợp, ẩn chi tiết
            TuitionSummaryGrid.IsVisible = true;
            TuitionDetailGrid.IsVisible = false;
        }
        else
        {
           
            vm.FilterTuitionDetailByMonth();
            // Hiển thị DataGrid chi tiết, ẩn tổng hợp
            TuitionDetailGrid.IsVisible = true;
            TuitionSummaryGrid.IsVisible = false;
        }
    }
}

        
    }
}
