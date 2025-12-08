using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Views.Statistical;

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
