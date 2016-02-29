using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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
            System.Drawing.Image myImg = System.Drawing.Image.FromFile(@"C:\apps\OcrOpenCvNET\MeterImage\MeterImage\BilderMartin\pic1\WP_20160226_006.jpg");
            
            // ImageSource ...
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
            this.image.Source = bi;
        }
    }
}
