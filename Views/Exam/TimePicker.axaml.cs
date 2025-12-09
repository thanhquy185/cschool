using Avalonia;
using Avalonia.Controls;
using System;
using System.Linq;

namespace Views.Exam
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

            this.AttachedToVisualTree += (_, __) => InitDefaultValue();
        }

        private void UpdateValue()
        {
            var hour = HourBox.SelectedItem?.ToString() ?? "00";
            var minute = MinuteBox.SelectedItem?.ToString() ?? "00";
            Value = $"{hour}:{minute}";
        }

        private void InitDefaultValue()
        {
            if (string.IsNullOrEmpty(Value))
                Value = "00:00";  // fallback

            var parts = Value.Split(':');

            HourBox.SelectedItem = parts[0];
            MinuteBox.SelectedItem = parts[1];
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