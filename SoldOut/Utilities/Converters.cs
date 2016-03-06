using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SoldOutCleanser.Utilities
{
    internal class ConditionIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: Improve this. Should really use an IConditionResolver
            if(value is int)
            {
                if ((int)value == 2)
                    return "New";
                else
                    return "Used";
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
