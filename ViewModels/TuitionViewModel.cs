using System;
using System.Collections.ObjectModel;
using cschool.Models;
using ReactiveUI;
using System.Reactive;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Bibliography;
using System.Globalization;
using Avalonia.Controls;




namespace cschool.ViewModels
{
    public class TuitionViewModel : ViewModelBase
    {
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    OnPropertyChanged();
                    OnTabChanged(value);
                }
            }
        }

        private void OnTabChanged(int index)
        {
            if (index == 0)
                Console.WriteLine("Đang mở mức học phí");
            else if (index == 1)
                Console.WriteLine("Đang mở quản lý học phí");
        }

        public ObservableCollection<ClassModel> FeeClassList { get; set; } = new ObservableCollection<ClassModel>();
        public ObservableCollection<FeeTemplateModel> FeeTemplateList { get; set; } = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<TuitionModel> TuitionList { get; set; } = new ObservableCollection<TuitionModel>();
         public ObservableCollection<FeeTemplateModel> DeletedFees { get; set; } = new();
        public ObservableCollection<FeeTemplateModel> BaseFees  {get;set;} = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<FeeTemplateModel> ExtraFees  {get;set;} = new ObservableCollection<FeeTemplateModel>();

        public ObservableCollection<FeeTemplateModel> BaseFees1  {get;set;} = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<FeeTemplateModel> ExtraFees1  {get;set;} = new ObservableCollection<FeeTemplateModel>();

        public ObservableCollection<FeeTemplateModel> BaseFees2  {get;set;} = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<FeeTemplateModel> ExtraFees2  {get;set;} = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<MonthFeeItem> MonthsHK1 { get; set; }
        public ObservableCollection<MonthFeeItem> MonthsHK2 { get; set; }
        public ObservableCollection<FeeTemplateModel> selectedFeeTemplate { get; set; }

        public decimal DefaultFeeHK1 { get; set; }
        public decimal DefaultFeeHK2 { get; set; }
        private TermModel T1Model;
        private TermModel T2Model;


        // ReactiveCommand cho Save
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveMonthFeeCommand { get; }
        private string _selectedTerm;
        public string SelectedTerm { get; set; }
        public ReactiveCommand<Unit, Unit> ApplyDefaultHK1Command { get; }
        public ReactiveCommand<Unit, Unit> ApplyDefaultHK2Command { get; }



        private ClassModel _selectedClass;
        public ClassModel SelectedClass
        {
            get => _selectedClass;
            set
            {
                _selectedClass = value;
                OnPropertyChanged();

                if (value != null)
                {
                    _ = LoadFeeMonths(value.Id);
                }
            }
        }



        public TuitionViewModel()
        {
            LoadData();

            // Tạo dữ liệu mẫu học sinh
            TuitionList.Add(new TuitionModel
            {
                StudentName = "Nguyễn Văn An",
                ClassName = "1A",
                TotalFee = 1000000 + 300000 + 200000,
                PaidAmount = 1500000,
                RemainingAmount = 600000,
                Status = "Đóng một phần"
            });

            TuitionList.Add(new TuitionModel
            {
                StudentName = "Trần Thị Bích",
                ClassName = "1B",
                TotalFee = 1200000 + 300000 + 250000,
                PaidAmount = 1750000,
                RemainingAmount = 0,
                Status = "Đã đóng"
            });

            TuitionList.Add(new TuitionModel
            {
                StudentName = "Lê Văn Cường",
                ClassName = "2A",
                TotalFee = 1100000 + 300000 + 220000,
                PaidAmount = 0,
                RemainingAmount = 1600000,
                Status = "Chưa đóng"
            });

            SaveCommand = ReactiveCommand.Create(SaveFeeTemplates);
           SaveMonthFeeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    
                    var feeClassMonths = SaveMonthFeeTemplates();
 
                    // gọi service lưu (nếu service thread-safe, không thao tác UI thì ok)
                    await Task.Run(() => AppService.TuitionService.SaveFeeClassMonths(feeClassMonths));
                });


        }

        public void LoadData()
        {
            FeeClassList.Clear();
            FeeTemplateList.Clear();
            BaseFees.Clear();
            BaseFees1.Clear();
            BaseFees2.Clear();
            ExtraFees1.Clear();
            ExtraFees2.Clear();

            var ft = AppService.TuitionService.GetFeeTemplates();
            
          foreach (var ftTemplate in ft)
{
     FeeTemplateList.Add(ftTemplate);
    // BASE
    if (ftTemplate.Type == "BASE")
    {
        BaseFees.Add(ftTemplate);

        BaseFees1.Add(new FeeTemplateModel
        {
            Id = ftTemplate.Id,
            Name = ftTemplate.Name,
            Type = ftTemplate.Type,
            Amount = ftTemplate.Amount,
            CreatedAt = ftTemplate.CreatedAt,
            UpdatedAt = ftTemplate.UpdatedAt,
            IsReadOnly = ftTemplate.IsReadOnly
        });

        BaseFees2.Add(new FeeTemplateModel
        {
            Id = ftTemplate.Id,
            Name = ftTemplate.Name,
            Type = ftTemplate.Type,
            Amount = ftTemplate.Amount,
            CreatedAt = ftTemplate.CreatedAt,
            UpdatedAt = ftTemplate.UpdatedAt,
            IsReadOnly = ftTemplate.IsReadOnly
        });
    }
    else
    {
        ExtraFees.Add(ftTemplate);

        ExtraFees1.Add(new FeeTemplateModel
        {
            Id = ftTemplate.Id,
            Name = ftTemplate.Name,
            Type = ftTemplate.Type,
            Amount = ftTemplate.Amount,
            CreatedAt = ftTemplate.CreatedAt,
            UpdatedAt = ftTemplate.UpdatedAt,
            IsReadOnly = ftTemplate.IsReadOnly
        });

        ExtraFees2.Add(new FeeTemplateModel
        {
            Id = ftTemplate.Id,
            Name = ftTemplate.Name,
            Type = ftTemplate.Type,
            Amount = ftTemplate.Amount,
            CreatedAt = ftTemplate.CreatedAt,
            UpdatedAt = ftTemplate.UpdatedAt,
            IsReadOnly = ftTemplate.IsReadOnly
        });
    }
}


            var classes= AppService.ClassService.GetClasses();
            foreach (var c in classes)
            {
                FeeClassList.Add(c);
            
            }


        

        }

        public void AddFee(string name = "", string type = "BASE", decimal amount = 0)
        {
            var newFee = new FeeTemplateModel
            {
                Name = name,
                Type = type,
                Amount = amount,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsReadOnly = false // cho phép chỉnh sửa ngay
            };

            FeeTemplateList.Add(newFee);
        }

        public void EditFee(FeeTemplateModel fee)
        {
            if (fee != null)
                fee.IsReadOnly = false;
        }

        public void DeleteFee(FeeTemplateModel fee)
        {
            FeeTemplateList.Remove(fee);

            if (fee.Id != 0)
                DeletedFees.Add(fee); // đánh dấu để cập nhật DB
        }

        private void SaveFeeTemplates()
        {
              AppService.TuitionService.SaveFeeTemplates(FeeTemplateList.ToList(), DeletedFees.ToList());
              DeletedFees.Clear();

            // Reload từ DB để set lại read-only
            LoadData();
           
    
        }


       public async Task LoadFeeMonths(int ClassId)
{
    // Lấy ClassModel cho từng học kỳ
    ClassModel classHK1 = FeeClassList.FirstOrDefault(c => c.Id == ClassId && c.Term == "Học kỳ 1");
    ClassModel classHK2 = FeeClassList.FirstOrDefault(c => c.Id == ClassId && c.Term == "Học kỳ 2");

    if (classHK1 == null && classHK2 == null)
        return;

    // Lấy Term từ service
    var termList = AppService.ClassService.GetOrCreateTerm(classHK1?.Year ?? classHK2?.Year ?? "0");
    TermModel T1 = termList.FirstOrDefault(t => t.TermName == "Học kỳ 1");
    TermModel T2 = termList.FirstOrDefault(t => t.TermName == "Học kỳ 2");

    MonthsHK1 = new ObservableCollection<MonthFeeItem>();
    MonthsHK2 = new ObservableCollection<MonthFeeItem>();

    // Hàm gán UpdateTotalAmount
    Action<MonthFeeItem, ObservableCollection<FeeTemplateModel>, ObservableCollection<FeeTemplateModel>> setUpdateAmount =
        (month, baseFees, extraFees) =>
        {
            month.UpdateTotalAmount = (m) =>
            {
                m.Amount = baseFees.Where(f => f.IsSelected).Sum(f => f.Amount)
                             + extraFees.Where(f => f.IsSelected).Sum(f => f.Amount);
            };
        };

    // Học kỳ 1
    if (T1 != null)
    {
        DateTime startDate = DateTime.Parse(T1.StartDate, CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.Parse(T1.EndDate, CultureInfo.InvariantCulture);
        var months1 = GetMonthsInTerm(startDate, endDate);

        foreach (var m in months1)
        {
            var monthItem = new MonthFeeItem
            {
                MonthId = m,
                MonthName = "Tháng " + m,
                Term = T1.Id
            };
            setUpdateAmount(monthItem, BaseFees1, ExtraFees1);
            MonthsHK1.Add(monthItem);
        }
    }

    // Học kỳ 2
    if (T2 != null)
    {
        DateTime startDate = DateTime.Parse(T2.StartDate, CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.Parse(T2.EndDate, CultureInfo.InvariantCulture);
        var months2 = GetMonthsInTerm(startDate, endDate);

        foreach (var m in months2)
        {
            var monthItem = new MonthFeeItem
            {
                MonthId = m,
                MonthName = "Tháng " + m,
                Term = T2.Id
            };
            setUpdateAmount(monthItem, BaseFees2, ExtraFees2);
            MonthsHK2.Add(monthItem);
        }
    }

    // Notify UI
    OnPropertyChanged(nameof(MonthsHK1));
    OnPropertyChanged(nameof(MonthsHK2));

    T1Model = T1;
    T2Model = T2;

}


private List<FeeClassMonthModel> SaveMonthFeeTemplates()
{
    List<FeeClassMonthModel> feeClassMonths = new List<FeeClassMonthModel>();

    // Lấy ClassModel cho từng học kỳ
    ClassModel classHK1 = FeeClassList.FirstOrDefault(c => c.Id == SelectedClass.Id && c.Term == "Học kỳ 1");
    ClassModel classHK2 = FeeClassList.FirstOrDefault(c => c.Id == SelectedClass.Id && c.Term == "Học kỳ 2");

    if (classHK1 == null && classHK2 == null)
        return feeClassMonths;

    // Duyệt 2 học kỳ
    foreach (var term in new[] { 1, 2 })
    {
        var months = term == 1 ? MonthsHK1 : MonthsHK2;
        var fees = term == 1 ? BaseFees1.Concat(ExtraFees1) : BaseFees2.Concat(ExtraFees2);

        var selectedMonths = months.Where(m => m.IsSelected);
        var selectedFees = fees.Where(f => f.IsSelected);

        // Lấy AssignClassId riêng cho từng học kỳ
        int assignClassId = term == 1 ? classHK1?.AssignClassId ?? 0 : classHK2?.AssignClassId ?? 0;

        // Lấy ngày bắt đầu/kết thúc
        string startDate = term == 1 
            ? DateTime.Parse(T1Model.StartDate).ToString("yyyy-MM-dd HH:mm:ss") 
            : DateTime.Parse(T2Model.StartDate).ToString("yyyy-MM-dd HH:mm:ss");

        string endDate = term == 1 
            ? DateTime.Parse(T1Model.EndDate).ToString("yyyy-MM-dd HH:mm:ss") 
            : DateTime.Parse(T2Model.EndDate).ToString("yyyy-MM-dd HH:mm:ss");

        var monthFeeModels = selectedMonths.SelectMany(
            month => selectedFees,
            (month, fee) => new FeeClassMonthModel
            {
                AssignClassId = assignClassId,
                FeeTemplateId = fee.Id,
                MonthId = month.MonthId,
                Term = term,
                Amount = fee.Amount,
                StartDate = startDate,
                EndDate = endDate,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

        feeClassMonths.AddRange(monthFeeModels);
    }

    return feeClassMonths; // <- Trả về ngoài vòng lặp
}



            public static List<int> GetMonthsInTerm(string startStr, string endStr)
            {
                if (!DateTime.TryParseExact(startStr, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) ||
                    !DateTime.TryParseExact(endStr, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
                {
                    throw new ArgumentException($"Ngày không hợp lệ: {startStr} - {endStr}");
                }

                return GetMonthsInTerm(startDate, endDate);
            }
                        private static List<int> GetMonthsInTerm(DateTime start, DateTime end)
            {
                var months = new List<int>();
                DateTime current = new DateTime(start.Year, start.Month, 1);
                DateTime endMonth = new DateTime(end.Year, end.Month, 1);

                while (current <= endMonth)
                {
                    months.Add(current.Month);
                    current = current.AddMonths(1);
                }

                return months;
            }




    }
}
