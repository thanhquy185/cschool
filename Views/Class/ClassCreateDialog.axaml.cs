using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Models;
using System;
using Avalonia.Input;
using Utils;

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

            if (GradesComboBox.Items.Count > 0)
                GradesComboBox.SelectedIndex = 0;

            YearTextBox.AddHandler(TextInputEvent, OnYearTextInput, RoutingStrategies.Tunnel);
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
                selectedAvailableHK1.Clear();
                dataGridStudentsAvailableHK1.SelectedItems.Clear();
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

        // ==================== SelectionChanged ====================
        private void DataGridStudentsAvailableHK1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                selectedAvailableHK1 = dg.SelectedItems.Cast<object>().ToList();
                dataGridStudentsInClassHK1.SelectedItems.Clear();
            }
        }

        private void DataGridStudentsInClassHK1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ClassViewModel vm && sender is DataGrid dg)
            {
                vm.SelectedStudentsHK1 = dg.SelectedItems.Cast<object>().ToList();
                dataGridStudentsAvailableHK1.SelectedItems.Clear();
                selectedAvailableHK1.Clear();
            }
        }

        private void DataGridStudentsAvailableHK2_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                selectedAvailableHK2 = dg.SelectedItems.Cast<object>().ToList();
                dataGridStudentsInClassHK2.SelectedItems.Clear();
            }
        }

        private void DataGridStudentsInClassHK2_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ClassViewModel vm && sender is DataGrid dg)
            {
                vm.SelectedStudentsHK2 = dg.SelectedItems.Cast<object>().ToList();
                dataGridStudentsAvailableHK2.SelectedItems.Clear();
                selectedAvailableHK2.Clear();
            }
        }

        // ==================== Confirm Button ====================
        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not ClassViewModel vm)
                return;

            if (vm.SelectedClass == null)
                vm.SelectedClass = new ClassModel();

            vm.SelectedClass.Grade = vm.SelectedGrade;

            if (vm.SelectedClassTypeModel != null)
            {
                vm.SelectedClass.ClassTypeId = vm.SelectedClassTypeModel.Id;
                vm.SelectedClass.ClassTypeName = vm.SelectedClassTypeModel.Name;
            }
            else
            {
                await MessageBoxUtil.ShowError("Chọn loại lớp trước!", owner: this);
                return;
            }

            vm.SelectedClass.TeacherHK1 = vm.SelectedTeacherHK1;
            vm.SelectedClass.TeacherHK2 = vm.SelectedTeacherHK2;

            // Validation
            if (string.IsNullOrWhiteSpace(vm.SelectedClass.Name))
            {
                await MessageBoxUtil.ShowError("Tên lớp không được để trống!", owner: this);
                return;
            }

            if (vm.SelectedGrade == 0)
            {
                await MessageBoxUtil.ShowError("Chọn khối lớp trước!", owner: this);
                return;
            }

            if (string.IsNullOrWhiteSpace(vm.Year))
            {
                await MessageBoxUtil.ShowError("Năm học không được để trống!", owner: this);
                return;
            }

            try
            {
                // Lưu lớp
                int classId = await AppService.ClassService.SaveClassAsync(vm.SelectedClass, vm.Year);

                if (classId <= 0)
                {
                    System.Console.WriteLine("Dính chỗ này nè "+ classId);
                    return;
                
                }

                System.Console.WriteLine("Class id: "+ classId);
                

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

                await MessageBoxUtil.ShowSuccess("Lưu lớp và gán học sinh thành công!", owner: this);

                // Load lại dữ liệu
                vm.LoadData();
                vm.LoadClassData();

                this.Close();
            }
           catch (Exception ex)
            {
                // Luôn dùng string thuần
                string errorMessage = ex?.Message ?? "Đã có lỗi xảy ra!";
                // await MessageBoxUtil.ShowError(errorMessage, owner: this);
                Console.WriteLine("Lỗi khi lưu lớp: " + errorMessage);
            }


        }

        // ==================== Year Input Validation ====================
        private void OnYearTextInput(object? sender, TextInputEventArgs e)
        {
            if (!e.Text.All(char.IsDigit))
            {
                e.Handled = true;
            }
        }
    }
}
