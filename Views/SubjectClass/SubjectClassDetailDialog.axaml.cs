using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ViewModels;

namespace Views.SubjectClass;

    public partial class SubjectClassDetailDialog : Window
    {
        public SubjectClassDetailDialog(SubjectClassViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }


