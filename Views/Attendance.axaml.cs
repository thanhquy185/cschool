using Avalonia.Controls;
using ViewModels;

namespace Views;

public partial class AttendanceView : UserControl
{
    private AttendanceViewModel _attendanceViewModel { get; set; }

    public AttendanceView()
    {
        InitializeComponent();
        this._attendanceViewModel = new AttendanceViewModel();
        DataContext =this. _attendanceViewModel;
    }
}