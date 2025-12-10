using System.Collections.ObjectModel;
using Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;
using System.Reactive;
using System.Globalization;
using Services;

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
                    await Task.Run(() => AppService.TuitionService.SaveFeeClassMonths(feeClassMonths));
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
                        Amount = totalAmount
                    };
                    
                    MonthsHK1.Add(monthItem);
                }
                SelectedMonthTuition = MonthsHK1.FirstOrDefault();
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
                    Amount = totalAmount
                };
                
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
            if (SelectedTerm == null || !TermMonthMap.ContainsKey(SelectedTerm.DisplayName)) return;

            foreach (var m in TermMonthMap[SelectedTerm.DisplayName])
                MonthList.Add("Tháng " + m);
        }

        // Filter chi tiết học phí theo tháng
        public void FilterTuitionDetailByMonth()
        {
            Console.WriteLine("===== FilterTuitionDetailByMonth START =====");
            Console.WriteLine($"SelectedMonth = '{SelectedMonth}'");

            if (string.IsNullOrEmpty(SelectedMonth))
            {
                Console.WriteLine("SelectedMonth is null or empty. Returning...");
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

            Console.WriteLine("===== FilterTuitionDetailByMonth END =====");
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
