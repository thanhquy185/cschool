using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Models
{
    // Model học phí của học sinh
    public class TuitionModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AssignClassId { get; set; }
        public string StudentName { get; set; } = "";
        public int ClassId { get; set; }
        public string ClassName { get; set; } = "";
        public string ClassYear { get; set; } = "";
        public int Grade { get; set; }             // giữ int
        public decimal TotalAmount { get; set; }   // mapping từ total_amount
        public bool IsPaid { get; set; }       
        public int MonthId { get; set; }
        public string MonthName { get; set; } = "";
        public string YearMonthDisplay => $"{ClassYear} - {MonthName}";
        public string PaymentStatus => IsPaid ? "Đã thu" : "Chưa thu";


    }

    public class MonthFeeItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private decimal _amount;

        public int MonthId { get; set; }
        public string MonthName { get; set; } = "";
        public int Term { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    UpdateTotalAmount?.Invoke(this); 
                }
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        // Callback để cập nhật tổng tiền
        public Action<MonthFeeItem>? UpdateTotalAmount;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class TuiTionTotal
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        
        public string ClassYear { get; set; } = "";
        public decimal TotalAmount { get; set; }


    }

    public class FeeMonthItemViewModel : INotifyPropertyChanged
{
    public int AssignClassId { get; set; }
    public int FeeTemplateId { get; set; }
    public string FeeTemplateName { get; set; }
    public int MonthId { get; set; }
    public decimal Amount { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    public decimal Total => IsSelected ? Amount : 0;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

}