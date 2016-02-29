using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Autofac.Core;
using Image = System.Windows.Controls.Image;

namespace SnipOutNumbers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow() 
        {
            InitializeComponent();
            button.Click += new RoutedEventHandler(btnLoad_Click);
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            // todo Achtung, für die richtige verwendung von autofac use MVVM
            var imageSrc = new ImageHelper(@"C:\apps\OcrOpenCvNET\MeterImage\MeterImage\BilderMartin\pic1\WP_20160226_006.jpg");
            this.image.Source = imageSrc.JpgToBitmapImage();
            // create filter
            //HomogenityEdgeDetector filter = new HomogenityEdgeDetector();
            // apply the filter

            Bitmap imageBm = imageSrc.JpgToBitmap();
            Bitmap gsImage = Grayscale.CommonAlgorithms.BT709.Apply(imageBm);
            CannyEdgeDetector filter = new CannyEdgeDetector();
            Bitmap edge = filter.Apply(gsImage);

            // todo in helper class
            MemoryStream ms = new MemoryStream();
            edge.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            this.imageGray.Source = bi;
            //this.imageGray.Source = (ImageSource)edge;

        }
    }
}
