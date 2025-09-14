using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cschool.ViewModels;

public partial class StudentViewModel : ViewModelBase
{
    public ObservableCollection<Student> Students { get; }

    public StudentViewModel()
    {
        var student = new List<Student>
            {
                new Student("Neil",  55),
                new Student("Buzz", 38),
                new Student("James",  44)
            };
        Students = new ObservableCollection<Student>(student);
    }
}