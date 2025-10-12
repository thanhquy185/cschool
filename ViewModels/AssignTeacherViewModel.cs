using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using cschool.Models;
using cschool.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Threading;

namespace cschool.ViewModels;

public partial class AssignTeacherViewModel : ViewModelBase
{
    private readonly AssignTeacherService _service;

    public ObservableCollection<AssignTeacher> AssignTeachers { get; } = new();
    public ObservableCollection<Teachers> Teachers { get; } = new();
    public ObservableCollection<Subjects> Subjects { get; } = new();
    public ObservableCollection<Classes> Classes { get; } = new();
    public ObservableCollection<string> DaysOfWeek { get; } = new();

    [ObservableProperty]
    private Teachers? _selectedTeacher;

    [ObservableProperty]
    private Subjects? _selectedSubject;

    [ObservableProperty]
    private Classes? _selectedClass;

    [ObservableProperty]
    private string? _selectedDay;

    [ObservableProperty]
    private bool _isFormVisible;

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private int _quizCount;

    [ObservableProperty]
    private int _oralCount;

    [ObservableProperty]
    private int _start;

    [ObservableProperty]
    private int _end;

    private AssignTeacher? _editingItem;

    // SỬA: Sử dụng RelayCommand của CommunityToolkit
    [RelayCommand]
    private void LoadData()
    {
        try
        {
            var assignTeachers = _service.GetAssignTeachers() ?? new List<AssignTeacher>();
            var teachers = _service.GetTeachers() ?? new List<Teachers>();
            var subjects = _service.GetCourses() ?? new List<Subjects>();
            var classes = _service.GetClasses() ?? new List<Classes>();
            var days = _service.GetDaysOfWeek(DateTime.Now) ?? new List<string>();

            Dispatcher.UIThread.Post(() =>
            {
                AssignTeachers.Clear();
                Teachers.Clear();
                Subjects.Clear();
                Classes.Clear();
                DaysOfWeek.Clear();

                foreach (var a in assignTeachers)
                    AssignTeachers.Add(a);

                foreach (var t in teachers)
                    Teachers.Add(t);

                foreach (var s in subjects)
                    Subjects.Add(s);

                foreach (var c in classes)
                    Classes.Add(c);

                foreach (var d in days)
                    DaysOfWeek.Add(d);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ToggleForm()
    {
        IsFormVisible = !IsFormVisible;
        if (!IsFormVisible)
        {
            _editingItem = null;
            // Reset form values
            SelectedTeacher = null;
            SelectedSubject = null;
            SelectedClass = null;
            SelectedDay = null;
            QuizCount = 0;
            OralCount = 0;
            Start = 0;
            End = 0;
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (SelectedTeacher == null || SelectedSubject == null || SelectedClass == null || string.IsNullOrEmpty(SelectedDay))
            return;

        var assign = new AssignTeacher(
            SelectedClass.Assign_class_Id,
            SelectedTeacher.Id,
            SelectedSubject.Id,
            SelectedSubject.Name,
            SelectedClass.Name,
            SelectedTeacher.Name,
            SelectedClass.Room,
            SelectedDay,
            Start,
            End
        )
        {
            QuizCount = QuizCount,
            OralCount = OralCount
        };

        bool result = _editingItem == null
            ? _service.AddAssignmentTeacher(assign)
            : _service.Update(assign);

        if (result)
        {
            LoadDataCommand.Execute(null);
            ToggleFormCommand.Execute(null);
        }
    }

    [RelayCommand]
    private void Edit(AssignTeacher a)
    {
        _editingItem = a;
        SelectedTeacher = Teachers.FirstOrDefault(t => t.Id == a.Teachers_id);
        SelectedSubject = Subjects.FirstOrDefault(s => s.Id == a.Subject_id);
        SelectedClass = Classes.FirstOrDefault(c => c.Assign_class_Id == a.Assign_class_id);
        SelectedDay = a.Day;
        Start = a.Start;
        End = a.End;
        QuizCount = a.QuizCount;
        OralCount = a.OralCount;
        IsFormVisible = true;
    }

    [RelayCommand]
    private void Delete(AssignTeacher a)
    {
        if (_service.DeleteAssignTeacher(a))
            LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private void Search()
    {
        var results = _service.Search(SearchText ?? "");
        Dispatcher.UIThread.Post(() =>
        {
            AssignTeachers.Clear();
            foreach (var a in results)
                AssignTeachers.Add(a);
        });
    }

    public AssignTeacherViewModel(AssignTeacherService service)
    {
        _service = service;
        LoadDataCommand.Execute(null);
    }
}