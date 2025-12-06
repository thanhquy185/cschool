using Avalonia.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace cschool.Models
{
    // Model học phí của học sinh
    public class TuitionModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = "";
        public int ClassId { get; set; }
        public string ClassName { get; set; } = "";
        public int TotalFee { get; set; }
        public int PaidAmount { get; set; }
        public int RemainingAmount { get; set; }
        public string Status { get; set; } = "";
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


    // Model tổng hợp của tháng với property bind cho Avalonia
    //     public class FeeClassMonthModel : INotifyPropertyChanged
    // {
    //     private decimal _totalAmount;
    //     public string MonthName { get; set; } = "";
    //     public ObservableCollection<MonthFeeItem> BaseFees { get; set; } = new();

    //     public decimal TotalAmount
    //     {
    //         get => _totalAmount;
    //         set
    //         {
    //             if (_totalAmount != value)
    //             {
    //                 _totalAmount = value;
    //                 OnPropertyChanged(nameof(TotalAmount));
    //             }
    //         }
    //     }

    //     public void RecalculateTotal()
    //     {
    //         TotalAmount = BaseFees.Where(f => f.IsSelected).Sum(f => f.Amount);
    //     }

    //     public event PropertyChangedEventHandler? PropertyChanged;
    //     protected void OnPropertyChanged(string name)
    //         => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    // }
}
