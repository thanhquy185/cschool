using System.ComponentModel;

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
        public bool IsPaid { get; set; }           // mapping từ is_paid
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
                    UpdateTotalAmount?.Invoke(this); // trigger tính tổng
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
}