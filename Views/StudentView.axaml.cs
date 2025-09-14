using Avalonia.Controls;
using cschool.ViewModels;

namespace cschool.Views;

public partial class StudentView : UserControl
{
    public StudentView()
    {
        InitializeComponent();
        DataContext = new StudentViewModel();
    }
}

