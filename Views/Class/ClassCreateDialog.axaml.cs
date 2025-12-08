using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Models;
using System;

namespace Views.Class
{
    public partial class ClassCreateDialog : Window
    {
        // Selection cục bộ cho "chưa có lớp"
        private IList<object> selectedAvailableHK1 = new List<object>();
        private IList<object> selectedAvailableHK2 = new List<object>();

        public ClassCreateDialog(ClassViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        // ==================== Close ====================
        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }


        // ==================== MoveStudents ====================
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

        // ==================== HK1 Buttons ====================
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

        // ==================== School Year Validation ====================
        private void SchoolYearTextBox_LostFocus(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ClassViewModel vm)
                vm.ValidateSchoolYear();
        }

        // ==================== Confirm Button ====================
private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
{
    if (DataContext is not ClassViewModel vm)
        return;

    if (vm.SelectedClass == null)
        vm.SelectedClass = new ClassModel();

    // Gán thông tin lớp
    vm.SelectedClass.Grade = vm.SelectedGrade;

    if (vm.SelectedClassTypeModel != null)
    {
        vm.SelectedClass.ClassTypeId = vm.SelectedClassTypeModel.Id;
        vm.SelectedClass.ClassTypeName = vm.SelectedClassTypeModel.Name;
    }
    else
    {
        Console.WriteLine("Chọn loại lớp trước!");
        return;
    }

    vm.SelectedClass.TeacherHK1 = vm.SelectedTeacherHK1;
    vm.SelectedClass.TeacherHK2 = vm.SelectedTeacherHK2;

    // Kiểm tra thông tin bắt buộc
    if (string.IsNullOrWhiteSpace(vm.SelectedClass.Name))
    {
        Console.WriteLine("Tên lớp không được để trống!");
        return;
    }

    if (vm.SelectedGrade == 0)
    {
        Console.WriteLine("Chọn khối lớp trước!");
        return;
    }

    if (string.IsNullOrWhiteSpace(vm.Year))
    {
        Console.WriteLine("Năm học không được để trống!");
        return;
    }

    try
    {
        // Lưu lớp
        int classId = await AppService.ClassService.SaveClassAsync(vm.SelectedClass, vm.Year);
        Console.WriteLine("Lưu lớp học thành công!");

        // Gán học sinh HK1
        if (vm.StudentInClassHK1.Count > 0)
        {
            await AppService.ClassService.AssignStudentsToClassAsync(
                classId, 
                1, 
                vm.Year, 
                vm.StudentInClassHK1.ToList()
            );
        }

        // Gán học sinh HK2
        if (vm.StudentInClassHK2.Count > 0)
        {
            await AppService.ClassService.AssignStudentsToClassAsync(
                classId, 
                2, 
                vm.Year, 
                vm.StudentInClassHK2.ToList()
            );
        }

        Console.WriteLine("Lưu lớp và gán học sinh thành công!");
        this.Close(); // đóng dialog nếu muốn
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi khi lưu lớp: " + ex.Message);
    }
}



        

        

        
    }
}
