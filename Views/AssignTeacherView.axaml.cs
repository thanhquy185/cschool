using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using cschool.Services;
using cschool.ViewModels;
using cschool.Views.DialogAssignTeacher;

namespace cschool.Views;

public partial class AssignTeacherView : UserControl
{
    // AssignTeacherViewModel ViewModel => DataContext as AssignTeacherViewModel;
    public AssignTeacherView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
   private async void OnAddButtonClick(object? sender, RoutedEventArgs e)
{
    if (DataContext is AssignTeacherViewModel vm)
    {
        var dialog = new AssignTeacherAddDialog
        {
            DataContext = vm   // ✅ GÁN VM CHO DIALOG
        };

        var window = (Window)this.VisualRoot!;
        var result = await dialog.ShowDialog<bool>(window);

        if (result)
        {
            // vm.LoadDataCommand.Execute(null);
        }
    }
}
    private async void OnEditButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AssignTeacherViewModel vm && vm.SelectedAssignTeacher != null)
        {
            DataContext = vm;
            await vm.OpenEditDialogCommand.ExecuteAsync(vm.SelectedAssignTeacher);
        }
    }
    private void SubjectCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = DataContext as AssignTeacherViewModel;
        vm?.SearchNameSubjectCommand.Execute(null);
    }


}
