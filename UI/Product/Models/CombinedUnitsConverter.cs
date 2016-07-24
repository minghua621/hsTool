using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace UI.Product.Models
{
    public class CombinedUnitsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string units = (string)values[0];
            ObservableCollection<UnitPriceItemModel> items = (ObservableCollection<UnitPriceItemModel>)values[1];

            string rlt = string.Empty;
            if (!string.IsNullOrEmpty(units))
            {
                string[] names = units.Split(",".ToCharArray());
                foreach (var code in names)
                {
                    UnitPriceItemModel item = items.FirstOrDefault(x => x.Code == code);
                    if (item != null)
                    {
                        rlt += item.Name + ",";
                    }
                }
            }
            return rlt;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
