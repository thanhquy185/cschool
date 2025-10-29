using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using cschool.Models;
using cschool.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Threading;
using System.Threading.Tasks;
using Avalonia;
using cschool.Views.DialogAssignTeacher;
using Avalonia.Controls.ApplicationLifetimes;
using cschool.Utils;

using cschool.ViewModels;
namespace cschool.ViewModels;

public partial class HomeClassViewModel : ViewModelBase
{
    private readonly HomeClassService _service;

    public List<Subjects> Subjects { get; set; } = new();

    [ObservableProperty]
    private ObservableCollection<HomeClass>? students;

    public HomeClassViewModel(HomeClassService service)
    {
        _service = service;

        // 1) Load danh sách môn
        Subjects = _service.GetSubject(assignClassID: 14);

        // 2) Load danh sách học sinh
        LoadStudentsCommand.Execute(null);
    }

    [RelayCommand]
    private void LoadStudents()
    {
        var data = _service.GetStudents(assignClassId: 14);
        Students = new ObservableCollection<HomeClass>(data);
    }
}
