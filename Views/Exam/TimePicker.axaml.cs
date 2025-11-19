using Avalonia;
using Avalonia.Controls;
using System;
using System.Linq;

namespace cschool.Views.Exam
{
    public partial class TimePicker : UserControl
    {
        public static readonly StyledProperty<string> ValueProperty =
            AvaloniaProperty.Register<TimePicker, string>(nameof(Value), defaultValue:"00:00");

        public string Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public TimePicker()
        {
            InitializeComponent();

            // Load dữ liệu
            HourBox.ItemsSource = Enumerable.Range(0, 24).Select(i => i.ToString("D2"));
            MinuteBox.ItemsSource = Enumerable.Range(0, 60).Select(i => i.ToString("D2"));

            HourBox.SelectionChanged += (s, e) => UpdateValue();
            MinuteBox.SelectionChanged += (s, e) => UpdateValue();
        }

        private void UpdateValue()
        {
            Value = $"{HourBox.SelectedItem}:{MinuteBox.SelectedItem}";
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ValueProperty && e.NewValue is string v)
            {
                // Khi ViewModel -> View
                var parts = v.Split(':');
                if (parts.Length == 2)
                {
                    HourBox.SelectedItem = parts[0];
                    MinuteBox.SelectedItem = parts[1];
                }
            }
        }

        public TimeSpan SelectedTime =>
            TimeSpan.Parse(Value);
    }
}
