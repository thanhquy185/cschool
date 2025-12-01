using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using cschool.ViewModels;
using cschool.Views.SubjectClass;
using System.Threading.Tasks;
using cschool.Models;
using cschool.Utils;
using System;


namespace cschool.Views;

public partial class SubjectClassView : UserControl
{
    // AssignTeacherViewModel ViewModel => DataContext as AssignTeacherViewModel;
    public SubjectClassView()
    {
        InitializeComponent();
        DataContext = new SubjectClassViewModel();

        // Use FindControl to locate named controls at runtime in case generated fields are not available
        var infoButton = this.FindControl<Button>("InfoButton");
        var createButton = this.FindControl<Button>("CreateButton");
        var importExcelButton = this.FindControl<Button>("ImportExcelButton");
        var exportExcelButton = this.FindControl<Button>("ExportExcelButton");

        infoButton.Click += async (_, _) => await ShowSubjectClassDialog(DialogModeEnum.Info);
        createButton.Click += async (_, _) => await ShowSubjectClassDialog(DialogModeEnum.Create); 
        importExcelButton.Click += (_, _) =>
        {
            var vm = DataContext as SubjectClassViewModel;
            if (vm != null)
            {
                vm.ImportFromExcelCommand.Execute().Subscribe();
            }
        };
        exportExcelButton.Click += (_, _) =>
        {
            var vm = DataContext as SubjectClassViewModel;
            if (vm != null)
            {
                vm.ExportToExcelCommand.Execute().Subscribe();
            }
        };
    }

    private async Task ShowSubjectClassDialog(DialogModeEnum mode)
    {
        var vm = DataContext as SubjectClassViewModel;

        var dataGrid = this.FindControl<DataGrid>("SubjectClassDataGrid");
        var selectedSubjectClass = dataGrid?.SelectedItem as SubjectClassModel;
        Console.WriteLine($"Selected SubjectClass: {selectedSubjectClass?.Assign_class_id}, subjectID: {selectedSubjectClass?.SubjectId}");
         if (selectedSubjectClass == null)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn lớp môn học để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                if (vm != null && selectedSubjectClass != null)
                {
                    vm.LoadStudentScores(selectedSubjectClass);
                    dialog = new SubjectClassDetailDialog(vm);
                }
                break;
            case DialogModeEnum.Create:
                if (vm != null && selectedSubjectClass != null)
                {
                    vm.LoadStudentScores(selectedSubjectClass);
                    dialog = new SubjectClassCreateDialog(vm);
                }   
                break;
        }
        if (dialog == null)
        {
            await MessageBoxUtil.ShowError("Không thể mở dialog. Vui lòng thử lại!");
            return;
        }

        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();

        // // Refresh list after create/update/lock
        // if (mode != DialogModeEnum.Info && vm != null)
        // {
        //     vm.LoadData(1); // Assuming teacherId = 1, adjust as needed
        // }
    }

    private void OnResetSearch(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SubjectClassViewModel vm)
        {
            vm.SearchText = string.Empty;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}