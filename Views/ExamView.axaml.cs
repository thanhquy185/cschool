using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using cschool.Utils;
using cschool.ViewModels;

namespace cschool.Views;

public partial class ExamView : UserControl
{
    public ExamView()
    {
        InitializeComponent();
        DataContext = new ExamViewModel();

        InfoButton.Click += async (_, _) => await ShowExamDialog(DialogModeEnum.Info);
        // CreateButton.Click += async (_, _) => await ShowExamDialog(DialogModeEnum.Create);
        // UpdateButton.Click += async (_, _) => await ShowExamDialog(DialogModeEnum.Update);
        // LockButton.Click += async (_, _) => await ShowExamDialog(DialogModeEnum.Lock);
    }

    private async Task ShowExamDialog(DialogModeEnum mode)
    {
        var vm = DataContext as ExamViewModel;
        var selectedExam = ExamDataGrid.SelectedItem as ExamModel;

        if (selectedExam == null && mode != DialogModeEnum.Create)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn người dùng để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                // if (vm != null && selectedExam != null)
                // {
                //     var fullExam = AppService.ExamService.GetExamById(selectedExam.Id);
                //     vm.ExamDetails = fullExam ?? selectedExam;
                // }
                // dialog = new ExamInfoDialog(vm);
                break;

            case DialogModeEnum.Create:
                // dialog = new ExamCreateDialog { ExamViewModel = vm };
                break;

            case DialogModeEnum.Update:
                // if (vm != null && selectedExam != null)
                // {
                //     var fullExam = AppService.ExamService.GetExamById(selectedExam.Id);
                //     vm.ExamDetails = fullExam ?? selectedExam;
                // }
                // dialog = new ExamUpdateDialog(vm);
                break;

            case DialogModeEnum.Lock:
                // if (vm != null && selectedExam != null)
                // {
                //     var fullExam = AppService.ExamService.GetExamById(selectedExam.Id);
                //     vm.ExamDetails = fullExam ?? selectedExam;
                // }
                // dialog = new ExamLockDialog(vm);
                break;
        }


        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

}