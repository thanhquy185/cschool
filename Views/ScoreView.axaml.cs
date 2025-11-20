using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using cschool.Services;
using cschool.ViewModels;
using cschool.Views.DialogAssignTeacher;

namespace cschool.Views;

public partial class ScoreView : UserControl
{
    // AssignTeacherViewModel ViewModel => DataContext as AssignTeacherViewModel;
    public ScoreView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}