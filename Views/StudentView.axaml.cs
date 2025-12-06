using Avalonia.Controls;
using ViewModels;
using System.Threading.Tasks;
using System.IO;
using Utils;
using Views.Student;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using System;
using Avalonia.Interactivity;

namespace Views;

public partial class StudentView : UserControl
{
    public StudentView()
    {
        InitializeComponent();
        DataContext = new StudentViewModel();

        InfoButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Info);
        CreateButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Create);
        UpdateButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Update);
        LockButton.Click += async (_, _) => await ShowStudentDialog(DialogModeEnum.Lock);
        ImportExcelButton.Click += async (_, _) => await ImportExcelButton_Click();
        ExportExcelButton.Click += async (_, _) => await ExportExcelButton_Click();
    }

    private async Task ShowStudentDialog(DialogModeEnum mode)
    {
        var vm = DataContext as StudentViewModel;
        var selectedStudent = StudentsDataGrid.SelectedItem as StudentModel;

        if (selectedStudent == null && mode != DialogModeEnum.Create)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn người dùng để thực hiện thao tác!");
            return;
        }

        Window? dialog = null;
        switch (mode)
        {
            case DialogModeEnum.Info:
                if (vm != null && selectedStudent != null)
                {
                    vm.GetStudentByIdCommand.Execute(selectedStudent.Id).ToTask();
                }
                dialog = new StudentInfoDialog(vm);
                break;

            case DialogModeEnum.Create:
                dialog = new StudentCreateDialog{studentViewModel = vm};
                break;

            case DialogModeEnum.Update:
                if (vm != null && selectedStudent != null)
                {
                    vm.GetStudentByIdCommand.Execute(selectedStudent.Id).ToTask();
                }
                dialog = new StudentUpdateDialog(vm);
                break;

            case DialogModeEnum.Lock:
                if (vm != null && selectedStudent != null)
                {
                    vm.GetStudentByIdCommand.Execute(selectedStudent.Id).ToTask();
                }
                dialog = new StudentLockDialog(vm);
                break;
        }


        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

    private async Task ImportExcelButton_Click()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Chọn file Excel để nhập",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Excel Files", Extensions = { "xlsx" } }
            }
        };

        var owner = TopLevel.GetTopLevel(this) as Window;
        var result = await dialog.ShowAsync(owner);
        if (result == null || result.Length == 0)
            return;

        var filePath = result[0];
        if (!File.Exists(filePath))
            return;

        // Gọi vào ViewModel
        var vm = DataContext as StudentViewModel;
        if (vm != null)
            await vm.ImportExcel(filePath);
    }
    
    private async Task ExportExcelButton_Click()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Chọn nơi lưu file Excel",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Excel Files", Extensions = { "xlsx" } }
            },
            InitialFileName = "DanhSachHocSinh.xlsx"
        };

        var owner = TopLevel.GetTopLevel(this) as Window;
        var filePath = await dialog.ShowAsync(owner);

        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var vm = DataContext as StudentViewModel;
        if (vm != null)
        {
            try
            {
                await vm.ExportExcel(filePath);
                await MessageBoxUtil.ShowSuccess("Xuất file Excel thành công!\n", owner: owner);
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowError(ex.Message, owner: owner);
            }
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DataContext is StudentViewModel vm)
        {
            var textBox = sender as TextBox;
            vm.FilterKeyword = textBox?.Text ?? "";
            vm.ApplyFilter();
        }
    }

    private void OnStatusFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is StudentViewModel vm)
        {
            var combo = sender as ComboBox;
            var selectedItem = combo?.SelectedItem as ComboBoxItem;
            var selectedText = selectedItem?.Content?.ToString() ?? "";

            vm.FilterStatus = selectedText == "Chọn Trạng thái" ? "" : selectedText;
            vm.ApplyFilter();
        }
    }

    private void OnResetFilterClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is StudentViewModel vm)
        {
            vm.FilterKeyword = "";
            vm.FilterStatus = "";
            vm.ApplyFilter();

            SearchBox.Text = "";
            StatusFilterBox.SelectedIndex = 0;
        }
    }

}
