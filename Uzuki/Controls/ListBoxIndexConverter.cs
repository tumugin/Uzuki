using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Uzuki.Controls
{
    public class ListBoxIndexConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = values[0];

            if (item == null)
            {
                return null;
            }

            var lb = values[1] as ListBox;
            if (lb == null)
            {
                return null;
            }

            //make it 1 based
            var rv = lb.Items.IndexOf(item) + 1;

            //very important because control
            //we are binding to is expecting a string
            return rv.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
