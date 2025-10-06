using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cschool.ViewModels;

public partial class StudentViewModel : ViewModelBase
{
    public ObservableCollection<StudentModel> Students { get; }

    public StudentViewModel()
    {
        Students = new ObservableCollection<StudentModel>();
        var students = AppService.StudentService.GetStudents();
        foreach (var student in students) Students.Add(student);
    }
}