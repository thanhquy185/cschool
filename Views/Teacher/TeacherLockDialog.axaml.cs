using Avalonia.Controls;
using Avalonia.Interactivity;
using Utils;
using ViewModels;
using Models;
using System;
using System.Reactive.Linq;

namespace Views.Teacher;

public partial class TeacherLockDialog : Window
{
    private TeacherModel? _teacher;
    private TeacherViewModel? _teacherViewModel;

    public TeacherLockDialog(TeacherViewModel? vm)
    {
        InitializeComponent();
        _teacherViewModel = vm;
        _teacher = vm?.SelectedTeacher;
        DataContext = _teacher;
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var result = await _teacherViewModel.LockTeacherCommand.Execute(_teacherViewModel.SelectedTeacher.Id);

            if (result)
            {
                await MessageBoxUtil.ShowSuccess($"Đã khóa giáo viên {_teacherViewModel.SelectedTeacher.Name} thành công!", owner: this);
                await _teacherViewModel.GetTeachersCommand.Execute();
                this.Close();
            }
            else
            {
                await MessageBoxUtil.ShowError("Không thể khóa giáo viên. Vui lòng thử lại!", owner: this);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ConfirmButton_Click: {ex.Message}");
            await MessageBoxUtil.ShowError($"Lỗi: {ex.Message}", owner: this);
        }
    }
}
