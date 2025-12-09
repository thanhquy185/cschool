using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ViewModels;
using Views.SubjectClass;
using System.Threading.Tasks;
using Models;
using Utils;
using System.Reactive.Threading.Tasks;


namespace Views;

public partial class SubjectClassView : UserControl
{
    private SubjectClassViewModel _subjectClassViewModel { get; set; }

    public SubjectClassView()
    {
        InitializeComponent();
        this._subjectClassViewModel = new SubjectClassViewModel();
        DataContext = this._subjectClassViewModel;

        // Use FindControl to locate named controls at runtime in case generated fields are not available
        var infoButton = this.FindControl<Button>("InfoButton");
        var createButton = this.FindControl<Button>("CreateButton");
        var importExcelButton = this.FindControl<Button>("ImportExcelButton");
        var exportExcelButton = this.FindControl<Button>("ExportExcelButton");
        var updateScoreColumnButton = this.FindControl<Button>("UpdateButton");

        infoButton.Click += async (_, _) => await ShowSubjectClassDialog(DialogModeEnum.Info);
        createButton.Click += async (_, _) => await ShowSubjectClassDialog(DialogModeEnum.Create);
        updateScoreColumnButton.Click += async (_, _) => await ShowSubjectClassDialog(DialogModeEnum.Update);
        importExcelButton.Click += (_, _) =>
        {
            var vm = DataContext as SubjectClassViewModel;
            if (vm != null)
            {
                vm.ImportFromExcelCommand.Execute().ToTask();
            }
        };
        exportExcelButton.Click += (_, _) =>
        {
            var vm = DataContext as SubjectClassViewModel;
            if (vm != null)
            {
                vm.ExportToExcelCommand.Execute().ToTask();
            }
        };
    }

    private async Task ShowSubjectClassDialog(DialogModeEnum mode)
    {
        var vm = DataContext as SubjectClassViewModel;

        var dataGrid = this.FindControl<DataGrid>("SubjectClassDataGrid");
        var selectedSubjectClass = dataGrid?.SelectedItem as SubjectClassModel;

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
            case DialogModeEnum.Update:
                if (vm != null && selectedSubjectClass != null)
                {
                    vm.LoadScoreColumns();
                    dialog = new SubjectClassUpdateScoreColumn(vm);
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