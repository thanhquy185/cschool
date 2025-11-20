using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace cschool.Views.DialogHomeClass;

    public partial class HomeClassDetailDialog : Window
    {
        public HomeClassDetailDialog()
        {
            InitializeComponent();
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
