using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace CaseFlowManager
{
    public class ProcessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Int32 status = (Int32)value;

            switch (status)
            {
                case 0: return @"Resource\Images\nostart.png";
                case 1: return @"Resource\Images\run.png";
                case 2: return @"Resource\Images\done.png";
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string doc = (string)value;
            if (doc != null)
                return @"Resource\Images\doc.png";
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            if (date != null && date.Year != 1)
                return date.ToShortDateString();
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
