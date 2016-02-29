using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge;
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
            var filter = new SobelEdgeDetector();//new CannyEdgeDetector();
            Bitmap edge = filter.Apply(gsImage);

            HoughLineTransformation lineTransform = new HoughLineTransformation();
            // apply Hough line transofrm
            lineTransform.ProcessImage(edge);

            // get lines using relative intensity
            HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(0.5);
            UnmanagedImage fertig = UnmanagedImage.FromManagedImage(imageBm);
            foreach (HoughLine line in lines)
            {
                // get line's radius and theta values
                int r = line.Radius;
                double t = line.Theta;
                // check if line is in lower part of the image
                if (r < 0)
                {
                    t += 180;
                    r = -r;
                }
                // convert degrees to radians
                t = (t / 180) * Math.PI;
                // get image centers (all coordinate are measured relative
                // to center)
                int w2 = edge.Width / 2;
                int h2 = edge.Height / 2;
                double x0 = 0, x1 = 0, y0 = 0, y1 = 0;
                if (line.Theta != 0)
                {
                    // none-vertical line
                    x0 = -w2; // most left point
                    x1 = w2;  // most right point
                    // calculate corresponding y values
                    y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                    y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                }
                else
                {
                    // vertical line
                    x0 = line.Radius;
                    x1 = line.Radius;
                    y0 = h2;
                    y1 = -h2;
                }

                AForge.Imaging.Drawing.Line(fertig,
                     new IntPoint((int)x0 + w2, h2 - (int)y0),
                    new IntPoint((int)x1 + w2, h2 - (int)y1),
                    System.Drawing.Color.Red);
            }
            Bitmap nnn = fertig.ToManagedImage();
            this.imageGray.Source = imageSrc.BitmapToImageSource(nnn);
            //this.imageGray.Source = imageSrc.BitmapToImageSource(gsImage);
            //this.imageGray.Source = imageSrc.JpgToBitmapImage();
            //this.imageGray.Source = imageSrc.BitmapToImageSource(edge);
            //this.imageGray.Source = imageSrc.BitmapToImageSource(lineTransform.ToBitmap());

        }
    }
}
