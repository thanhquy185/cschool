using Avalonia;
using Avalonia.Controls;

using cschool.ViewModels;

using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;

using System.Linq;

using System.Collections.ObjectModel;
using cschool.Utils;

using System;


namespace cschool.Views.Class;

public partial class ClassUpdateDialog : Window
{
        private IList<object> selectedAvailableHK1 = new List<object>();
        private IList<object> selectedAvailableHK2 = new List<object>();
    
    public ClassUpdateDialog(ClassViewModel vm)
    {
        DataContext=vm;
        InitializeComponent();
        
    }

        private void MoveStudents(IList<object> selected, ObservableCollection<StudentModel> from, ObservableCollection<StudentModel> to)
        {
            if (selected == null || selected.Count == 0) return;

            foreach (StudentModel s in selected.Cast<StudentModel>())
            {
                if (!to.Contains(s))
                    to.Add(s);
                from.Remove(s);
            }
        }
        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
                private void SchoolYearTextBox_LostFocus(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ClassViewModel vm)
               { vm.ResetState();
                vm.ValidateSchoolYear();}
        }
    
        private void AddStudentsHK1_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ClassViewModel vm)
            {
                MoveStudents(selectedAvailableHK1, vm.StudentsAvailableHK1, vm.StudentInClassHK1);
                selectedAvailableHK1.Clear(); // clear selection cục bộ
                dataGridStudentsAvailableHK1.SelectedItems.Clear(); // clear UI
            }
        }

        private void RemoveStudentsHK1_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ClassViewModel vm)
            {
                MoveStudents(vm.SelectedStudentsHK1, vm.StudentInClassHK1, vm.StudentsAvailableHK1);
                vm.SelectedStudentsHK1.Clear();
                dataGridStudentsInClassHK1.SelectedItems.Clear();
            }
        }

        // ==================== HK2 Buttons ====================
        private void AddStudentsHK2_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ClassViewModel vm)
            {
                MoveStudents(selectedAvailableHK2, vm.StudentsAvailableHK2, vm.StudentInClassHK2);
                selectedAvailableHK2.Clear();
                dataGridStudentsAvailableHK2.SelectedItems.Clear();
            }
        }

        private void RemoveStudentsHK2_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ClassViewModel vm)
            {
                MoveStudents(vm.SelectedStudentsHK2, vm.StudentInClassHK2, vm.StudentsAvailableHK2);
                vm.SelectedStudentsHK2.Clear();
                dataGridStudentsInClassHK2.SelectedItems.Clear();
            }
        }

        // ==================== HK1 SelectionChanged ====================
        // Chọn bên "chưa có lớp"
        private void DataGridStudentsAvailableHK1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                selectedAvailableHK1 = dg.SelectedItems.Cast<object>().ToList();
                // bỏ chọn bên "trong lớp"
                dataGridStudentsInClassHK1.SelectedItems.Clear();
            }
        }

        // Chọn bên "trong lớp"
        private void DataGridStudentsInClassHK1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ClassViewModel vm && sender is DataGrid dg)
            {
                vm.SelectedStudentsHK1 = dg.SelectedItems.Cast<object>().ToList();
                // bỏ chọn bên "chưa có lớp"
                dataGridStudentsAvailableHK1.SelectedItems.Clear();
                selectedAvailableHK1.Clear();
            }
        }

        // ==================== HK2 SelectionChanged ====================
        // Chọn bên "chưa có lớp"
        private void DataGridStudentsAvailableHK2_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                selectedAvailableHK2 = dg.SelectedItems.Cast<object>().ToList();
                // bỏ chọn bên "trong lớp"
                dataGridStudentsInClassHK2.SelectedItems.Clear();
            }
        }

        // Chọn bên "trong lớp"
        private void DataGridStudentsInClassHK2_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ClassViewModel vm && sender is DataGrid dg)
            {
                vm.SelectedStudentsHK2 = dg.SelectedItems.Cast<object>().ToList();
                // bỏ chọn bên "chưa có lớp"
                dataGridStudentsAvailableHK2.SelectedItems.Clear();
                selectedAvailableHK2.Clear();
            }
        }


        private async void Confirm(object? sender, RoutedEventArgs e)
{
    if (DataContext is not ClassViewModel vm)
        return;

    try
    {
        // Gọi command ViewModel
        var result = await vm.ConfirmCommand.Execute().ToTask();

        if (result)
        {
            // Thông báo thành công
            await MessageBoxUtil.ShowSuccess("Lưu lớp học thành công!", owner: this);

            // Đóng dialog, return true để View cha reload
                vm.LoadData();
                
            Close(true);
        }
        else
        {
            await MessageBoxUtil.ShowError(
                "Không thể lưu lớp học!\nVui lòng kiểm tra lại thông tin.",
                owner: this
            );
        }
    }
    catch (Exception ex)
    {
        await MessageBoxUtil.ShowError($"Lỗi hệ thống:\n{ex.Message}", owner: this);
    }
}


}