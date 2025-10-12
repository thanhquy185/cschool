using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using cschool.Services;
using cschool.ViewModels;

namespace cschool.Views;

public partial class AssignTeacherView : UserControl
{
    public AssignTeacherView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
