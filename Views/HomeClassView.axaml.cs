using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using cschool.Models;
using cschool.Services;
using cschool.ViewModels;
using System.Linq;
using System;
using Avalonia.Threading;

namespace cschool.Views;

public partial class HomeClassView : UserControl
{
    public HomeClassView()
    {
        InitializeComponent();
    }
    

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}