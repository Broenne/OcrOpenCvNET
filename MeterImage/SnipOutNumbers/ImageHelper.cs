using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SnipOutNumbers
{
    public interface IImageHelper
    {
        BitmapImage JpgToBitmapImage();
    }

    public class ImageHelper : IImageHelper
    {
        private readonly System.Drawing.Image myImg;

        public ImageHelper(string fileName)
        {
            var fileNameLocal = fileName;
            this.myImg = System.Drawing.Image.FromFile(fileNameLocal);
        }

        public BitmapImage JpgToBitmapImage()
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            // Save to a memory stream...
            myImg.Save(ms, ImageFormat.Bmp);
            // Rewind the stream...
            ms.Seek(0, SeekOrigin.Begin);
            // Tell the WPF image to use this stream...
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }

        public Bitmap JpgToBitmap()
        {
            Bitmap bi = new Bitmap(myImg);
            return bi;
        }

    }
}
