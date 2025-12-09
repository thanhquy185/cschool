using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data.Converters;
using Models;
using Avalonia.Media;

namespace Utils
{
    public class NullToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value != null;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
       public class IndexConverter : IValueConverter
    {
        public static IndexConverter Instance { get; } = new IndexConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return (index + 1).ToString();
            }
            return string.Empty;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    
    // Converter để tính tổng các giá trị
    public class SumConverter : IMultiValueConverter
    {
        public static SumConverter Instance { get; } = new SumConverter();
        
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            int sum = 0;
            foreach (var value in values)
            {
                if (value is int intValue)
                {
                    sum += intValue;
                }
                else if (value is ObservableCollection<Statistical> collection)
                {
                    sum += collection.Count;
                }
            }
            return sum.ToString();
        }
        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    public class YearToForegroundConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int year)
                return year < DateTime.Now.Year ? Brushes.Red : Brushes.Black;
            return Brushes.Black;
        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
