
using BarcodeLib;
using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PrintBarcodeSheeat
{
    public class BarCodeConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string)value;
            Barcode barcode = new Barcode() { BarWidth = 5, Height=128,LabelFont=new System.Drawing.Font("asdf",20f) };
            barcode.Alignment = AlignmentPositions.CENTER;
            barcode.IncludeLabel = false;
            using (var ms = new MemoryStream())
            {
                barcode.Encode(BarcodeLib.TYPE.CODE128, text).Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                return bitmapImage;
            }
             
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
