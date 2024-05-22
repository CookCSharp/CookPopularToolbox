/*
 *Description: DoubleToMemoryUnitConverter
 *Author: Chance.zheng
 *Creat Time: 2023/11/30 10:36:44
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
using System.Windows.Data;

namespace CompressTool
{
    public class DoubleToMemoryUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValueType valueType)
            {
                var m = (double)value / 1024D / 1024D;
                var kb = (double)value / 1024D;
                if (Math.Round(m, 2) >= 1)
                    return Math.Round(m, 2) + "M";
                else if ((Math.Round(kb, 2) >= 1))
                    return Math.Round(kb, 2) + "KB";
                else
                    return value + "B";
            }

            return "None";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
