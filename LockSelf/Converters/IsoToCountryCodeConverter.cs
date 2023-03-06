using LockSelf.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Linq;

namespace LockSelf.Converters
{
    public class IsoToCountryCodeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            XDocument myxml = XDocument.Parse(Resources.countries);
            var elements = from r in myxml.Descendants("country")
                           select new
                           {
                               Code = r.Attribute("code").Value,
                               Iso = r.Attribute("iso").Value,
                           };

            foreach (var r in elements)
            {
                if(r.Code == value.ToString())
                {
                    return "+" + r.Iso.ToString();
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
