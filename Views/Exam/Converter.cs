using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace cschool.Views.Exam
{
    public class IntToIndexConverter : IValueConverter
    {
        public static readonly IntToIndexConverter Instance = new IntToIndexConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
                return index + 1; // STT bắt đầu từ 1
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
