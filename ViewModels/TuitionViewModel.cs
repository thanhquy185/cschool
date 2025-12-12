using System.Collections.ObjectModel;
using Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;
using System.Reactive;
using System.Globalization;
using Services;
using ClosedXML.Excel;

namespace ViewModels
{
    public class TuitionViewModel : ViewModelBase
    {
        // Tiêu đề trang
        public string TitlePage { get; } = "Quản lý học phí";
        // Mô tả trang
        public string DescriptionPage { get; } = "Quản lý thông tin học phí";

        public ObservableCollection<ClassModel> FeeClassList { get; set; } = new ObservableCollection<ClassModel>();
        public ObservableCollection<ClassModel> FeeClassListView { get; set; } = new ObservableCollection<ClassModel>();
        public ObservableCollection<FeeTemplateModel> FeeTemplateList { get; set; } = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<TuitionModel> TuitionList { get; set; } = new ObservableCollection<TuitionModel>();
        public ObservableCollection<TuitionModel> TuitionListView { get; set; } = new ObservableCollection<TuitionModel>();
        public ObservableCollection<TuitionModel> TuitionDetail { get; set; } = new ObservableCollection<TuitionModel>();
        public TuitionModel SelectedStudent { get; set; }

        public ObservableCollection<FeeTemplateModel> DeletedFees { get; set; } = new();
        public ObservableCollection<FeeTemplateModel> BaseFees { get; set; } = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<FeeTemplateModel> ExtraFees { get; set; } = new ObservableCollection<FeeTemplateModel>();

        public ObservableCollection<FeeTemplateModel> BaseFees1 { get; set; } = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<FeeTemplateModel> ExtraFees1 { get; set; } = new ObservableCollection<FeeTemplateModel>();

        public ObservableCollection<FeeTemplateModel> BaseFees2 { get; set; } = new ObservableCollection<FeeTemplateModel>();
        public ObservableCollection<FeeTemplateModel> ExtraFees2 { get; set; } = new ObservableCollection<FeeTemplateModel>();
        private ObservableCollection<FeeClassMonthModel> _allStudentFeeMonthDetail = new ObservableCollection<FeeClassMonthModel>();
        public ObservableCollection<FeeClassMonthModel> AllStudentFeeMonthDetail => _allStudentFeeMonthDetail;
        public ObservableCollection<MonthFeeItem> MonthsHK1 { get; set; }
        public ObservableCollection<MonthFeeItem> MonthsHK2 { get; set; }

        public ObservableCollection<FeeTemplateModel> selectedFeeTemplate { get; set; }

        private ObservableCollection<FeeTemplateModel> _tuitionDetailFiltered = new();
        public ObservableCollection<FeeTemplateModel> TuitionDetailFiltered
        {
            get => _tuitionDetailFiltered;
            set => SetProperty(ref _tuitionDetailFiltered, value);
        }
        public ObservableCollection<TermClassModel> TermList { get; set; } = new ObservableCollection<TermClassModel>();
        private TermClassModel? _selectedTerm;
        public TermClassModel? SelectedTerm
        {
            get => _selectedTerm;
            set
            {
                if (_selectedTerm != value)
                {
                    _selectedTerm = value;
                    OnPropertyChanged();
                    LoadMonthsForSelectedTerm();
                }
            }
        }

        // Danh sách học kỳ hiển thị (bao gồm "Tất cả")
        public ObservableCollection<string> TermDisplayList { get; set; } = new ObservableCollection<string>();
        private string? _selectedTermDisplay;
        public string? SelectedTermDisplay
        {
            get => _selectedTermDisplay;
            set
            {
                if (_selectedTermDisplay != value)
                {
                    _selectedTermDisplay = value;
                    OnPropertyChanged();
                    LoadMonthsForSelectedTerm();
                }
            }
        }

        // Danh sách tháng theo học kỳ
        public ObservableCollection<string> MonthList { get; set; } = new ObservableCollection<string>();

        private string? _selectedMonth;
        public string? SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (_selectedMonth != value)
                {
                    _selectedMonth = value;
                    OnPropertyChanged();
                    FilterTuitionDetailByMonth();
                }
            }
        }

        // Text tìm kiếm chung
        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }
        }

        // Tìm kiếm danh sách học sinh trong Quản lý học phí
        private string? _studentSearchText;
        public string? StudentSearchText
        {
            get => _studentSearchText;
            set
            {
                if (_studentSearchText != value)
                {
                    _studentSearchText = value;
                    OnPropertyChanged();
                    FilterTuitionSummary();
                }
            }
        }

        private string? _selectedFilterClassName;
        public string? SelectedFilterClassName
        {
            get => _selectedFilterClassName;
            set
            {
                if (_selectedFilterClassName != value)
                {
                    _selectedFilterClassName = value;
                    OnPropertyChanged();
                    FilterTuitionSummary();
                }
            }
        }

        private string? _selectedFilterClassYear;
        public string? SelectedFilterClassYear
        {
            get => _selectedFilterClassYear;
            set
            {
                if (_selectedFilterClassYear != value)
                {
                    _selectedFilterClassYear = value;
                    OnPropertyChanged();
                    FilterTuitionSummary();
                }
            }
        }

        // Tìm kiếm danh sách lớp (Mức học phí tab)
        private string? _feeClassSearchText;
        public string? FeeClassSearchText
        {
            get => _feeClassSearchText;
            set
            {
                if (_feeClassSearchText != value)
                {
                    _feeClassSearchText = value;
                    OnPropertyChanged();
                    FilterFeeClassList();
                }
            }
        }

        // Filter bộ lọc chi tiết mức phí theo khối, năm học, lớp
        public ObservableCollection<string> ClassLevelList { get; set; } = new ObservableCollection<string>();
        private string? _selectedClassLevel;
        public string? SelectedClassLevel
        {
            get => _selectedClassLevel;
            set
            {
                if (_selectedClassLevel != value)
                {
                    _selectedClassLevel = value;
                    OnPropertyChanged();
                    LoadClassYearsForLevel();
                }
            }
        }

        public ObservableCollection<string> ClassYearList { get; set; } = new ObservableCollection<string>();
        private string? _selectedClassYear;
        public string? SelectedClassYear
        {
            get => _selectedClassYear;
            set
            {
                if (_selectedClassYear != value)
                {
                    _selectedClassYear = value;
                    OnPropertyChanged();
                    LoadClassNamesForYearAndLevel();
                }
            }
        }

        public ObservableCollection<string> ClassNameList { get; set; } = new ObservableCollection<string>();
        private string? _selectedClassName;
        public string? SelectedClassName
        {
            get => _selectedClassName;
            set
            {
                if (_selectedClassName != value)
                {
                    _selectedClassName = value;
                    OnPropertyChanged();
                    FilterFeeDetailByClass();
                }
            }
        }

        // Chi tiết học phí (DataGrid)
        public ObservableCollection<FeeClassMonthModel> StudentFeeMonthDetail { get; set; } = new ObservableCollection<FeeClassMonthModel>();

        // Map Term -> List tháng
        public Dictionary<string, List<int>> TermMonthMap { get; set; } = new Dictionary<string, List<int>>();

        // Load tháng khi chọn học kỳ


        // Tổng tiền hiển thị
        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }


        public decimal DefaultFeeHK1 { get; set; }
        public decimal DefaultFeeHK2 { get; set; }
        private TermClassModel T1Model;
        private TermClassModel T2Model;



        // ReactiveCommand cho Save
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveMonthFeeCommand { get; }

        public TuitionModel SelectedFeeCollection { get; set; }
        public ReactiveCommand<Unit, Unit> ApplyDefaultHK1Command { get; }
        public ReactiveCommand<Unit, Unit> ApplyDefaultHK2Command { get; }



        private ClassModel _selectedClass;
        public ClassModel SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (_selectedClass != value)
                {
                    _selectedClass = value;
                    OnPropertyChanged();

                    if (value != null)
                    {
                        System.Console.WriteLine("Selected Class changed: " + value.Name);
                        LoadFeeMonths(value.Id);
                    }
                }
            }
        }
        
       private MonthFeeItem? _selectedMonthTuition;
        public MonthFeeItem? SelectedMonthTuition
        {
            get => _selectedMonthTuition;
            set
            {
                if (_selectedMonthTuition != value)
                {
                    _selectedMonthTuition = value;
                    OnPropertyChanged();

                    if (value != null)
                    {
                        System.Console.WriteLine("Selected Month changed: " + value.MonthName);
                        LoadSelectedMonthDetail();
                    }
                        // LoadFeesForSelectedMonth(value);
                }
            }
        }


        public TuitionViewModel()
        {
            LoadData();
            LoadSelectedMonthDetail();
            // Lấy full list học sinh
            var listStudent = AppService.TuitionService.GetAllStudents();
            TuitionList.Clear();
            foreach (var student in listStudent)
            {
                TuitionList.Add(student);
            }

            // Lọc cho ListView: mỗi học sinh mỗi năm chỉ 1 record
            var distinctTuition = TuitionList
                .GroupBy(t => new { t.StudentId, t.StudentName, t.ClassYear }) // nhóm theo học sinh + năm
                .Select(g => g.First())  // lấy bản ghi đầu tiên trong nhóm
                .ToList();

            // Thêm vào TuitionListView (giả sử là ObservableCollection hoặc List)
            TuitionListView.Clear();
            foreach (var tuition in distinctTuition)
            {
                TuitionListView.Add(tuition);
            }

            SaveCommand = ReactiveCommand.Create(SaveFeeTemplates);
            SaveMonthFeeCommand = ReactiveCommand.CreateFromTask(async () =>
                {

                    var feeClassMonths = SaveMonthFeeTemplates();

                    // gọi service lưu (nếu service thread-safe, không thao tác UI thì ok)
                    var saved = await Task.Run(() => AppService.TuitionService.SaveFeeClassMonths(feeClassMonths));
                    if (!saved)
                    {
                        Console.WriteLine("❌ SaveFeeClassMonths returned false");
                    }
                });


        }

        public async Task LoadData()
        {
            FeeClassList.Clear();
            FeeTemplateList.Clear();
            FeeClassListView.Clear();
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


            var classes = AppService.ClassService.GetClasses();
            foreach (var c in classes)
            {
                FeeClassList.Add(c);
            }

            // Populate class filter lists (Grade/Year/Name)
            ClassLevelList.Clear();
            ClassLevelList.Add("Tất cả");
            foreach (var g in FeeClassList.Select(c => c.Grade.ToString()).Distinct().OrderBy(x => x))
                ClassLevelList.Add(g);

            ClassYearList.Clear();
            ClassYearList.Add("Tất cả");
            foreach (var y in FeeClassList.Select(c => c.Year).Distinct().OrderBy(y => y))
                ClassYearList.Add(y);

            ClassNameList.Clear();
            ClassNameList.Add("Tất cả");
            foreach (var n in FeeClassList.Select(c => c.Name).Distinct().OrderBy(n => n))
                ClassNameList.Add(n);

            var distinctClasses = FeeClassList
                .GroupBy(c => new { c.Name, c.Year })
                .Select(g => g.First())
                .ToList();
            foreach (var distinctClass in distinctClasses)
            {
                FeeClassListView.Add(distinctClass);
            }
        }

        public async Task LoadSelectedMonthDetail()
        {
            
            if (SelectedMonthTuition == null) return;

            int monthId = SelectedMonthTuition.MonthId;

            // Lọc dữ liệu từ _allStudentFeeMonthDetail
            var filteredDetails = _allStudentFeeMonthDetail
                .Where(f => f.MonthId == monthId)
                .ToList();
           var baseFees = SelectedTerm.TermName == "Học kỳ 1" ? BaseFees1 : BaseFees2;

            StudentFeeMonthDetail.Clear();
          foreach (var detail in filteredDetails)
            {
                var fee = baseFees.FirstOrDefault(bf => bf.Id == detail.FeeTemplateId);
                if (fee != null)
                {
                    fee.IsSelected = true;
                }
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

            LoadData();


        }

        public async Task LoadFeeMonths(int ClassId)
        {
            // Lấy Class    
            // Model cho từng học kỳ

            ClassModel classHK1 = FeeClassList.FirstOrDefault(c => c.Id == ClassId && c.Term == "Học kỳ 1");
            ClassModel classHK2 = FeeClassList.FirstOrDefault(c => c.Id == ClassId && c.Term == "Học kỳ 2");
            var FeeHK1 = new ObservableCollection<FeeClassMonthModel>();
            var FeeHK2 = new ObservableCollection<FeeClassMonthModel>();
            var savedFeeMonths1 = AppService.TuitionService.GetSavedFeeClassMonths(classHK1?.AssignClassId ?? 0);
                    if (savedFeeMonths1 != null)
                    {
                        foreach (var feeMonth in savedFeeMonths1)
                        {
                            FeeHK1.Add(feeMonth);
                        }
                    }
                var savedFeeMonths2 = AppService.TuitionService.GetSavedFeeClassMonths(classHK2?.AssignClassId ?? 0);
                    if (savedFeeMonths2 != null)
                    {
                        foreach (var feeMonth in savedFeeMonths2)
                        {
                            FeeHK2.Add(feeMonth);
                        }
                    }
            
            // Load toàn bộ dữ liệu fee vào _allStudentFeeMonthDetail cho dialog chi tiết
            _allStudentFeeMonthDetail.Clear();
            foreach (var fee in FeeHK1.Concat(FeeHK2))
            {
                _allStudentFeeMonthDetail.Add(fee);
            }
            
            if (classHK1 == null && classHK2 == null)
                return;

            // Lấy Term từ service
            var termList = AppService.ClassService.GetOrCreateTerm(classHK1?.Year ?? classHK2?.Year ?? "0");
            TermClassModel T1 = termList.FirstOrDefault(t => t.TermName == "Học kỳ 1");
            TermClassModel T2 = termList.FirstOrDefault(t => t.TermName == "Học kỳ 2");

            MonthsHK1 = new ObservableCollection<MonthFeeItem>();
            MonthsHK2 = new ObservableCollection<MonthFeeItem>();

           
            // Học kỳ 1
            if (T1 != null)
            {
                DateTime startDate = DateTime.Parse(T1.StartDate, CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.Parse(T1.EndDate, CultureInfo.InvariantCulture);
                var months1 = GetMonthsInTerm(startDate, endDate);

                foreach (var m in months1)
                {
                    // Lọc các fee cho tháng m
                    var filteredDetails = FeeHK1
                        .Where(f => f.MonthId == m)
                        .ToList();

                    // Tính tổng tiền cho tháng
                    decimal totalAmount = filteredDetails.Sum(f => f.Amount);

                    // Tạo MonthFeeItem
                    var monthItem = new MonthFeeItem
                    {
                        MonthId = m,
                        MonthName = "Tháng " + m,
                        Term = T1.Id,
                        Amount = totalAmount,
                        IsSelected = false  // Thêm default value
                    };
                    
                    MonthsHK1.Add(monthItem);
                }
                SelectedMonthTuition = MonthsHK1.FirstOrDefault();
                if (SelectedMonthTuition != null)
                    LoadSelectedFeeForMonth(SelectedMonthTuition);
            }

            // Học kỳ 2
            if (T2 != null)
            {
                DateTime startDate = DateTime.Parse(T2.StartDate, CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.Parse(T2.EndDate, CultureInfo.InvariantCulture);
                var months2 = GetMonthsInTerm(startDate, endDate);

                foreach (var m in months2)
                {
                    // Lọc các fee cho tháng m
                    var filteredDetails = FeeHK2
                        .Where(f => f.MonthId == m)
                        .ToList();

                    // Tính tổng tiền cho tháng
                    decimal totalAmount = filteredDetails.Sum(f => f.Amount);

                    // Tạo MonthFeeItem
                    var monthItem = new MonthFeeItem
                    {
                        MonthId = m,
                        MonthName = "Tháng " + m,
                        Term = T2.Id,
                        Amount = totalAmount,
                        IsSelected = false  // Thêm default value
                    };
                    
                    MonthsHK2.Add(monthItem);
                }
            }

            // Notify UI
            OnPropertyChanged(nameof(MonthsHK1));
            OnPropertyChanged(nameof(MonthsHK2));

            // Load TermList và MonthList cho dialog chi tiết
            TermList.Clear();
            if (T1 != null)
                TermList.Add(T1);
            if (T2 != null)
                TermList.Add(T2);
            
            MonthList.Clear();
            // Lấy tất cả tháng từ MonthsHK1 và MonthsHK2
            foreach (var month in MonthsHK1.Concat(MonthsHK2).DistinctBy(m => m.MonthId))
            {
                MonthList.Add(month.MonthName);
            }

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

            // XÓA dữ liệu cũ trong tuition_monthly trước khi lưu
            var assignId1 = classHK1?.AssignClassId ?? 0;
            var assignId2 = classHK2?.AssignClassId ?? 0;
            if (assignId1 > 0)
                AppService.TuitionService.DeleteTuitionMonthlyByAssignClass(assignId1);
            if (assignId2 > 0)
                AppService.TuitionService.DeleteTuitionMonthlyByAssignClass(assignId2);

            // Duyệt 2 học kỳ
            foreach (var term in new[] { 1, 2 })
            {
                var months = term == 1 ? MonthsHK1 : MonthsHK2;
                var fees = term == 1 ? BaseFees1.Concat(ExtraFees1) : BaseFees2.Concat(ExtraFees2);

                var selectedMonths = months.Where(m => m.IsSelected);
                var selectedFees = fees.Where(f => f.IsSelected);

                // Lấy AssignClassId riêng cho từng học kỳ
                int assignClassId = term == 1 ? classHK1?.AssignClassId ?? 0 : classHK2?.AssignClassId ?? 0;

                // XÓA DỮ LIỆU CŨ cho từng tháng được chọn trước khi lưu dữ liệu mới
                foreach (var month in selectedMonths)
                {
                    AppService.TuitionService.DeleteFeeClassMonthsByClassAndMonth(assignClassId, month.MonthId);
                }

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

            // Diagnostic logging: what we are about to save
            Console.WriteLine("[DEBUG] SaveMonthFeeTemplates: selected months and fees to save:");
            foreach (var m in MonthsHK1 ?? new ObservableCollection<MonthFeeItem>())
            {
                if (m.IsSelected)
                    Console.WriteLine($"  HK1 Month: {m.MonthName} (Id={m.MonthId}), Amount={m.Amount}");
            }
            foreach (var m in MonthsHK2 ?? new ObservableCollection<MonthFeeItem>())
            {
                if (m.IsSelected)
                    Console.WriteLine($"  HK2 Month: {m.MonthName} (Id={m.MonthId}), Amount={m.Amount}");
            }

            Console.WriteLine("[DEBUG] Selected fees HK1:");
            foreach (var f in BaseFees1.Concat(ExtraFees1))
                if (f.IsSelected) Console.WriteLine($"  Fee: {f.Name} (Id={f.Id}), Amount={f.Amount}");

            Console.WriteLine("[DEBUG] Selected fees HK2:");
            foreach (var f in BaseFees2.Concat(ExtraFees2))
                if (f.IsSelected) Console.WriteLine($"  Fee: {f.Name} (Id={f.Id}), Amount={f.Amount}");

            Console.WriteLine($"[DEBUG] Total feeClassMonths to insert: {feeClassMonths.Count}");

            return feeClassMonths; // <- Trả về ngoài vòng lặp
        }
        public async Task LoadSelectedFeeForMonth(MonthFeeItem monthItem)
        {

              var ft = AppService.TuitionService.GetFeeTemplates();
            BaseFees1.Clear();
            BaseFees2.Clear();
            foreach (var ftTemplate in ft)
            {
             
    

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
            if (monthItem == null) return;

            System.Console.WriteLine("Loading fees for month: " + monthItem.MonthName+ " (ID: " + monthItem.MonthId + ")");
            System.Console.WriteLine("Selected Class ID: " + SelectedClass?.Id);

            // Lấy assign_class_id theo từng học kỳ
            var classHK1 = FeeClassList.FirstOrDefault(c => c.Id == SelectedClass.Id && c.Term == "Học kỳ 1");
            var classHK2 = FeeClassList.FirstOrDefault(c => c.Id == SelectedClass.Id && c.Term == "Học kỳ 2");

            int assignHK1 = classHK1?.AssignClassId ?? 0;
            int assignHK2 = classHK2?.AssignClassId ?? 0;

            System.Console.WriteLine("AssignClassId HK1: " + assignHK1);

            System.Console.WriteLine("AssignClassId HK2: " + assignHK2);
            // Lấy dữ liệu từ DB
            var savedFeesHK1 = await Task.Run(() => AppService.TuitionService.GetSavedFeeClassMonths(assignHK1));
            var savedFeesHK2 = await Task.Run(() => AppService.TuitionService.GetSavedFeeClassMonths(assignHK2));
          

            foreach (var fee in BaseFees1)
            {
                fee.IsSelected = false;
            }
            foreach (var fee in BaseFees2)
            {
                fee.IsSelected = false;
            }
            // Load fee HK1
            foreach (var fee in BaseFees1)
                {fee.IsSelected = savedFeesHK1.Any(f => f.FeeTemplateId == fee.Id && f.MonthId == monthItem.MonthId);
                System.Console.WriteLine("Name: " + fee.Name + "isselected" + fee.IsSelected);}


            // Load fee HK2
            foreach (var fee in BaseFees2)
                fee.IsSelected = savedFeesHK2.Any(f => f.FeeTemplateId == fee.Id && f.MonthId == monthItem.MonthId);
            OnPropertyChanged(nameof(BaseFees1));  
            OnPropertyChanged(nameof(BaseFees2)); 



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
        public void LoadStudentTuitionDetail(int studentId)
        {
            try
            {
                // 1️⃣ Khởi tạo các collection nếu chưa
                TuitionList ??= new ObservableCollection<TuitionModel>();
                TuitionDetail ??= new ObservableCollection<TuitionModel>();
                StudentFeeMonthDetail ??= new ObservableCollection<FeeClassMonthModel>();
                TermList ??= new ObservableCollection<TermClassModel>();
                MonthList ??= new ObservableCollection<string>();
                TermMonthMap ??= new Dictionary<string, List<int>>();

                // 2️⃣ Lấy tất cả học sinh
                var listStudentTuition = AppService.TuitionService.GetAllStudents();
                if (listStudentTuition == null)
                {
                    Console.WriteLine("GetAllStudents() trả về null");
                    return;
                }

                // 3️⃣ Clear dữ liệu cũ
                TuitionList.Clear();
                TuitionDetail.Clear();
                StudentFeeMonthDetail.Clear();
                TermList.Clear();
                MonthList.Clear();
                TermMonthMap.Clear();
                _allStudentFeeMonthDetail.Clear();

                // 4️⃣ Thêm học sinh vào TuitionList
                foreach (var student in listStudentTuition)
                {
                    if (student != null)
                        TuitionList.Add(student);
                }

                // 5️⃣ Lọc theo studentId
                var studentTuition = TuitionList
                    .Where(t => t.StudentId == studentId)
                    .OrderBy(t => t.MonthId)
                    .ToList();

                if (studentTuition.Count == 0)
                {
                    Console.WriteLine($"Không tìm thấy học sinh với StudentId={studentId}");
                    return;
                }

                // 6️⃣ Lấy học kỳ, tháng, và dữ liệu fee từng tháng
                var allTerms = new List<TermClassModel>();

                var processedAssignClassIds = new HashSet<int>();

                foreach (var tuition in studentTuition)
                {
                    var assignClass = AppService.ClassService.GetAssignClassById(tuition.AssignClassId);
                    if (assignClass == null) continue;

                    // Nếu AssignClassId này đã xử lý rồi thì bỏ qua
                    if (processedAssignClassIds.Contains(assignClass.Id))
                        continue;

                    processedAssignClassIds.Add(assignClass.Id);

                    var term = AppService.ClassService.GetTermById(assignClass.TermId);
                    if (term == null) continue;

                    // Lưu học kỳ nếu chưa có
                    if (!allTerms.Any(t => t.Id == term.Id))
                        allTerms.Add(term);

                    // Lấy tháng của học kỳ
                    DateTime startDate = DateTime.Parse(term.StartDate, CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.Parse(term.EndDate, CultureInfo.InvariantCulture);
                    var months = GetMonthsInTerm(startDate, endDate); // List<int>

                    // Map Term -> List tháng
                    if (!TermMonthMap.ContainsKey(term.DisplayName))
                        TermMonthMap[term.DisplayName] = months;

                    // Lấy dữ liệu học phí thực tế từ DB
                    var savedFeeMonths = AppService.TuitionService.GetSavedFeeClassMonths(assignClass.Id);
                    if (savedFeeMonths != null)
                    {
                        foreach (var feeMonth in savedFeeMonths)
                        {
                            _allStudentFeeMonthDetail.Add(feeMonth);
                        }
                    }
                }


                // 7️⃣ Đổ TermList và chọn học kỳ mặc định
                foreach (var t in allTerms.OrderBy(t => t.StartDate))
                    TermList.Add(t);

                SelectedTerm = TermList.FirstOrDefault();

                // 8️⃣ Load MonthList theo học kỳ đầu tiên
                LoadMonthsForSelectedTerm();

                // 9️⃣ Đổ TuitionDetail (tổng hợp học phí)
                foreach (var tuition in studentTuition)
                {
                    TuitionDetail.Add(tuition);
                    Console.WriteLine($"[SUMMARY] StudentName: {tuition.StudentName}, Class: {tuition.ClassName}, TotalAmount: {tuition.TotalAmount}");
                }

                Console.WriteLine("LoadStudentTuitionDetail completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadStudentTuitionDetail exception: " + ex.Message);
            }
        }
        private void LoadMonthsForSelectedTerm()
        {
            MonthList.Clear();

            // Nếu chọn "Tất cả" -> hiển thị tất cả tháng
            if (SelectedTermDisplay == "Tất cả")
            {
                MonthList.Add("Tất cả");
                foreach (var month in MonthsHK1.Concat(MonthsHK2).DistinctBy(m => m.MonthId).OrderBy(m => m.MonthId))
                {
                    MonthList.Add(month.MonthName);
                }
                SelectedMonth = "Tất cả";
                return;
            }

            // Nếu chọn học kỳ cụ thể
            if (string.IsNullOrEmpty(SelectedTermDisplay)) return;

            MonthList.Add("Tất cả");
            if (SelectedTermDisplay == "Học kỳ 1")
            {
                foreach (var month in MonthsHK1.OrderBy(m => m.MonthId))
                    MonthList.Add(month.MonthName);
            }
            else if (SelectedTermDisplay == "Học kỳ 2")
            {
                foreach (var month in MonthsHK2.OrderBy(m => m.MonthId))
                    MonthList.Add(month.MonthName);
            }

            SelectedMonth = "Tất cả";
        }

        // Load danh sách năm học cho khối đã chọn
        private void LoadClassYearsForLevel()
        {
            ClassYearList.Clear();
            if (string.IsNullOrEmpty(SelectedClassLevel) || SelectedClassLevel == "Tất cả")
            {
                ClassYearList.Add("Tất cả");
                var years = FeeClassList.DistinctBy(c => c.Year).Select(c => c.Year).OrderBy(y => y);
                foreach (var year in years)
                    ClassYearList.Add(year);
                SelectedClassYear = "Tất cả";
                return;
            }

            ClassYearList.Add("Tất cả");
            var yearsForLevel = FeeClassList
                .Where(c => c.Grade.ToString() == SelectedClassLevel)
                .DistinctBy(c => c.Year)
                .Select(c => c.Year)
                .OrderBy(y => y);
            foreach (var year in yearsForLevel)
                ClassYearList.Add(year);

            SelectedClassYear = "Tất cả";
        }

        // Load danh sách lớp cho khối và năm học đã chọn
        private void LoadClassNamesForYearAndLevel()
        {
            ClassNameList.Clear();
            if ((string.IsNullOrEmpty(SelectedClassLevel) || SelectedClassLevel == "Tất cả") &&
                (string.IsNullOrEmpty(SelectedClassYear) || SelectedClassYear == "Tất cả"))
            {
                ClassNameList.Add("Tất cả");
                var names = FeeClassList.DistinctBy(c => c.Name).Select(c => c.Name).OrderBy(n => n);
                foreach (var name in names)
                    ClassNameList.Add(name);
                SelectedClassName = "Tất cả";
                return;
            }

            ClassNameList.Add("Tất cả");
            var query = FeeClassList.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedClassLevel) && SelectedClassLevel != "Tất cả")
                query = query.Where(c => c.Grade.ToString() == SelectedClassLevel);

            if (!string.IsNullOrEmpty(SelectedClassYear) && SelectedClassYear != "Tất cả")
                query = query.Where(c => c.Year == SelectedClassYear);

            var names2 = query.DistinctBy(c => c.Name).Select(c => c.Name).OrderBy(n => n);
            foreach (var name in names2)
                ClassNameList.Add(name);

            SelectedClassName = "Tất cả";
        }

        // Filter danh sách học sinh (Quản lý học phí) theo tên, lớp, năm
        public void FilterTuitionSummary()
        {
            try
            {
                if (TuitionList == null) return;

                var q = TuitionList.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(StudentSearchText))
                {
                    var s = StudentSearchText.Trim().ToLowerInvariant();
                    q = q.Where(t => (!string.IsNullOrEmpty(t.StudentName) && t.StudentName.ToLowerInvariant().Contains(s)));
                }

                if (!string.IsNullOrWhiteSpace(SelectedFilterClassName) && SelectedFilterClassName != "Tất cả")
                {
                    q = q.Where(t => (t.ClassName ?? string.Empty) == SelectedFilterClassName);
                }

                if (!string.IsNullOrWhiteSpace(SelectedFilterClassYear) && SelectedFilterClassYear != "Tất cả")
                {
                    q = q.Where(t => (t.ClassYear ?? string.Empty) == SelectedFilterClassYear);
                }

                // Deduplication: mỗi học sinh mỗi năm chỉ 1 record
                var distinctFiltered = q
                    .GroupBy(t => new { t.StudentId, t.StudentName, t.ClassYear })
                    .Select(g => g.First())
                    .ToList();

                TuitionListView.Clear();
                foreach (var item in distinctFiltered)
                    TuitionListView.Add(item);
            }
            catch (Exception)
            {
                // ignore filter exceptions to keep UI responsive
            }
        }

        // Filter danh sách lớp (Mức học phí tab) theo tên lớp + khối
        public void FilterFeeClassList()
        {
            try
            {
                if (FeeClassList == null) return;

                var q = FeeClassList.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(FeeClassSearchText))
                {
                    var s = FeeClassSearchText.Trim().ToLowerInvariant();
                    q = q.Where(c => 
                        (!string.IsNullOrEmpty(c.Name) && c.Name.ToLowerInvariant().Contains(s)) ||
                        (!string.IsNullOrEmpty(c.Grade.ToString()) && c.Grade.ToString().ToLowerInvariant().Contains(s))
                    );
                }

                // Deduplication: mỗi lớp (Name + Year) chỉ 1 record
                var distinctFiltered = q
                    .GroupBy(c => new { c.Name, c.Year })
                    .Select(g => g.First())
                    .ToList();

                FeeClassListView.Clear();
                foreach (var item in distinctFiltered)
                    FeeClassListView.Add(item);
            }
            catch (Exception)
            {
                // ignore filter exceptions
            }
        }

        // Filter chi tiết phí theo lớp
        private void FilterFeeDetailByClass()
        {
            StudentFeeMonthDetail.Clear();

            var filtered = AllStudentFeeMonthDetail.AsEnumerable();

            // Filter theo lớp nếu chọn cụ thể
            if (!string.IsNullOrEmpty(SelectedClassName) && SelectedClassName != "Tất cả")
            {
                var selectedClass = FeeClassList.FirstOrDefault(c => c.Name == SelectedClassName);
                if (selectedClass != null)
                    filtered = filtered.Where(f => f.AssignClassId == selectedClass.AssignClassId);
            }

            foreach (var fee in filtered)
                StudentFeeMonthDetail.Add(fee);
        }

        // Filter chi tiết học phí theo tháng
        public void FilterTuitionDetailByMonth()
        {
            Console.WriteLine("===== FilterTuitionDetailByMonth START =====");
            Console.WriteLine($"SelectedMonth = '{SelectedMonth}'");

            if (string.IsNullOrEmpty(SelectedMonth) || SelectedMonth == "Tất cả")
            {
                Console.WriteLine("SelectedMonth is null or 'Tất cả'. Showing all...");
                StudentFeeMonthDetail.Clear();
                foreach (var f in _allStudentFeeMonthDetail)
                {
                    StudentFeeMonthDetail.Add(f);
                }
                return;
            }

            int monthId;
            if (!int.TryParse(SelectedMonth.Replace("Tháng ", ""), out monthId))
            {
                Console.WriteLine($"Cannot parse month from SelectedMonth='{SelectedMonth}'");
                return;
            }

            Console.WriteLine($"Parsed monthId = {monthId}");

            Console.WriteLine("Original StudentFeeMonthDetail:");
            foreach (var f in _allStudentFeeMonthDetail)
            {
                Console.WriteLine($"  AssignClassId={f.AssignClassId}, MonthId={f.MonthId}, Amount={f.Amount}, Start={f.StartDate}, End={f.EndDate}");
            }

            var filtered = _allStudentFeeMonthDetail.Where(f => f.MonthId == monthId).ToList();

            Console.WriteLine("Filtered items:");
            foreach (var f in filtered)
            {
                Console.WriteLine($"  AssignClassId={f.AssignClassId}, MonthId={f.MonthId}, Amount={f.Amount}, Start={f.StartDate}, End={f.EndDate}");
            }

            StudentFeeMonthDetail.Clear();
            foreach (var f in filtered)
                StudentFeeMonthDetail.Add(f);

            // Thêm hàng tổng tiền ở cuối
            decimal totalMonth = filtered.Sum(f => f.Amount);
            StudentFeeMonthDetail.Add(new FeeClassMonthModel
            {
                MonthId = 0,
                FeeTemplateName = "TỔNG CỘNG",
                Amount = totalMonth,
                StartDate = "",
                EndDate = ""
            });

            Console.WriteLine("===== FilterTuitionDetailByMonth END =====");
        }

       public void LoadFeeMonthsSummary()
        {
            try
            {
                Console.WriteLine("===== LoadFeeMonthsSummary START =====");

                StudentFeeMonthDetail.Clear();

                // 1) Lấy danh sách học sinh đã đóng tiền theo tháng
                var students = AppService.TuitionService.GetAllStudents();   // trả về List<TuitionModel>

                // 2) Ghép thông tin FeeMonth + thông tin Student
                var monthFees = _allStudentFeeMonthDetail
                    .GroupBy(f => f.MonthId)
                    .Select(g =>
                    {
                        int monthId = g.Key;

                        // Tìm thông tin học sinh theo StudentId + MonthId
                        var studentPay = students
                            .FirstOrDefault(s => s.MonthId == monthId && s.StudentId == SelectedStudent.StudentId);

                        string paymentStatus = studentPay?.IsPaid == true ? "Đã thu" : "Chưa thu";

                        return new FeeClassMonthModel
                        {
                            MonthId = monthId,
                            FeeTemplateName = $"Tổng phí tháng {monthId}",
                            Amount = g.Sum(x => x.Amount),
                            StartDate = g.FirstOrDefault()?.StartDate ?? "",
                            EndDate = g.FirstOrDefault()?.EndDate ?? "",
                            PaymentStatus = paymentStatus
                        };
                    })
                    .ToList();

                // 3) Add item vào ObservableCollection
                foreach (var fee in monthFees)
                {
                    StudentFeeMonthDetail.Add(fee);
                }

                // 4) tổng cộng
                decimal totalAll = monthFees.Sum(f => f.Amount);
                StudentFeeMonthDetail.Add(new FeeClassMonthModel
                {
                    MonthId = 0,
                    FeeTemplateName = "TỔNG CỘNG",
                    Amount = totalAll,
                    StartDate = "",
                    EndDate = "",
                    PaymentStatus = ""
                });

                Console.WriteLine("===== LoadFeeMonthsSummary END =====");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi LoadFeeMonthsSummary: {ex.Message}");
            }
        }


        // Xuất Excel chi tiết học phí
        public async Task ExportExcel(string filePath)
        {
            try
            {
                if (StudentFeeMonthDetail == null || !StudentFeeMonthDetail.Any())
                    throw new InvalidOperationException("Không có dữ liệu để xuất!");

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Chi tiết học phí");

                    // ===== HEADER =====
                    string[] headers = { "Diễn giải", "Số tiền", "Từ ngày", "Đến ngày" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    // ===== DATA =====
                    int row = 2;
                    foreach (var fee in StudentFeeMonthDetail)
                    {
                        worksheet.Cell(row, 1).Value = fee.FeeTemplateName;
                        worksheet.Cell(row, 2).Value = fee.Amount;
                        worksheet.Cell(row, 3).Value = fee.StartDate;
                        worksheet.Cell(row, 4).Value = fee.EndDate;

                        for (int col = 1; col <= headers.Length; col++)
                            worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row++;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xuất Excel: " + ex.Message, ex);
            }
        }

   
    }

}
