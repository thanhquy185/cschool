using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using cschool.Models;
using cschool.Services;
using cschool.ViewModels;

namespace cschool.Views;

public partial class HomeClassView : UserControl
{
    public HomeClassView()
    {
        InitializeComponent();
        
        this.DataContextChanged += (s, e) =>
        {
            var vm = DataContext as HomeClassViewModel;
            if (vm != null)
            {
                // Đăng ký sự kiện khi Subjects thay đổi
                vm.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(HomeClassViewModel.Subjects))
                    {
                        BuildColumns(vm.Subjects);
                    }
                };
                
                // Xây dựng columns nếu Subjects đã có sẵn
                if (vm.Subjects?.Count > 0)
                {
                    BuildColumns(vm.Subjects);
                }
            }
        };
    }

    private void BuildColumns(List<Subjects> subjects)
    {
        if (GridStudents == null || subjects == null) return;

        // Giữ lại các cột cố định (Mã HS, Tên học sinh)
        var fixedColumns = new List<DataGridColumn>();
        foreach (var column in GridStudents.Columns)
        {
            if (column.Header?.ToString() == "Mã HS" || column.Header?.ToString() == "Tên học sinh")
            {
                fixedColumns.Add(column);
            }
        }

        // Xóa tất cả columns cũ
        GridStudents.Columns.Clear();

        // Thêm lại các cột cố định
        foreach (var column in fixedColumns)
        {
            GridStudents.Columns.Add(column);
        }

        // Thêm cột cho từng môn học
        foreach (var subject in subjects)
        {
            GridStudents.Columns.Add(new DataGridTextColumn 
            {
                Header = subject.Name,
                Binding = new Binding($"SubjectScores[{subject.Name}]") 
                { 
                    StringFormat = "{0:F1}",
                    FallbackValue = "-"
                },
                Width = new DataGridLength(80)
            });
        }

        // Thêm các cột tổng hợp
        GridStudents.Columns.Add(new DataGridTextColumn 
        {
            Header = "GPA",
            Binding = new Binding(nameof(HomeClass.GpaTotal)) 
            { 
                StringFormat = "{0:F2}",
                FallbackValue = "-"
            },
            Width = new DataGridLength(80)
        });
        
        GridStudents.Columns.Add(new DataGridTextColumn 
        {
            Header = "Hạnh kiểm",
            Binding = new Binding(nameof(HomeClass.ConductLevel))
            {
                FallbackValue = "-"
            },
            Width = new DataGridLength(100)
        });
        
        GridStudents.Columns.Add(new DataGridTextColumn 
        {
            Header = "Học lực",
            Binding = new Binding(nameof(HomeClass.Academic))
            {
                FallbackValue = "-"
            },
            Width = new DataGridLength(100)
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}