using System.Collections.ObjectModel;
using System.Reactive.Linq;
using cschool.Models;
using cschool.Services;

namespace cschool.ViewModels;

public partial class TeacherViewModel : ViewModelBase
{
    public string Content { get; set; } = "Trang thông tin giáo viên";

    private readonly TeacherService _service;
    public ObservableCollection<Teachers> Teachers { get; } = new();



    
}