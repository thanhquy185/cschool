using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace cschool.Views.Statistical;

    public partial class StatisticalDetailDialog : Window
    {
        public StatisticalDetailDialog()
        {
            InitializeComponent();
        }
        
          private void InitializeComponent()
         {
             AvaloniaXamlLoader.Load(this);
         }

    }
