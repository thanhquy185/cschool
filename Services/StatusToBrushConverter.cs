using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace cschool.Services
{
    public class StatusToBrushConverter : IValueConverter
    {
        // Có thể truyền dictionary mapping trạng thái -> màu
        public Dictionary<string, string>? StatusColorMap { get; set; } = new Dictionary<string, string>
        {
            { "Hoạt động", "#5EC97B" },
            { "Tạm dừng", "#F66B6B" },
        };

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string s) return Brushes.Gray;

            if (StatusColorMap != null && StatusColorMap.TryGetValue(s, out string hex))
            {
                return (IBrush)new SolidColorBrush(Color.Parse(hex));
            }

            return Brushes.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
