using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using cschool.ViewModels;
namespace cschool.Views.Tuition;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using cschool.ViewModels;
using cschool.Models;
using cschool.ViewModels;
using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;


public partial class TuitionCollectionDialog : Window
{
    public TuitionCollectionDialog(TuitionViewModel viewmodel)
    {
            InitializeComponent();
            DataContext = viewmodel;
    }
    private void Close_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
}      