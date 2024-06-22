using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace PM2EXAMEN7669.Controllers
{
    public class base64Img : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            ImageSource imageSource = null;
            if (value != null)
            {
                String base64 = (String)value;
                byte[] fotobyte = System.Convert.FromBase64String(base64);
                var stream = new MemoryStream(fotobyte);

                imageSource = ImageSource.FromStream(() =>
                {
                    var newStream = new MemoryStream(fotobyte);
                    return newStream;
                });
            }

            return imageSource;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}