using Avalonia.Controls;
using ViewModels;
using Avalonia.Interactivity;

namespace Views.Tuition;
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