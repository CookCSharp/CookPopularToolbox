/*
 *Description: EnumToBooleanConverter
 *Author: Chance.zheng
 *Creat Time: 2023/11/29 14:24:42
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CompressTool
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var can1 = Enum.TryParse(value.GetType(), value.ToString(), out object valueResult);
            var can2 = Enum.TryParse(value.GetType(), parameter.ToString(), out object parameterResult);

            if (can1 && can2 && valueResult.Equals(parameterResult))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool v && v)
            {
                var can = Enum.TryParse(targetType, parameter.ToString(), out object result);
                if (can) return result;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
