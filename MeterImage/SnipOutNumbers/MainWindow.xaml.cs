using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using AForge.Math.Geometry;
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



            // locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobCounter.ProcessImage(edge);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            // check for rectangles
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            foreach (var blob in blobs)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                List<IntPoint> cornerPoints;

                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners form a rectangle
                    //if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle)
                    //{
                        // here i use the graphics class to draw an overlay, but you
                        // could also just use the cornerPoints list to calculate your
                        // x, y, width, height values.
                        var Points = new List<System.Drawing.Point>();
                        foreach (var point in cornerPoints)
                        {
                            Points.Add(new System.Drawing.Point(point.X, point.Y));
                        }
                        
                        Graphics g = Graphics.FromImage(imageSrc.myImg);
                        g.DrawPolygon(new System.Drawing.Pen(System.Drawing.Color.Red, 5.0f), Points.ToArray());

                        imageSrc.myImg.Save("result.png");
                    //}
                }
            }




            //HoughLineTransformation lineTransform = new HoughLineTransformation();
            //// apply Hough line transofrm
            //lineTransform.ProcessImage(edge);

            //var fertig = UnmanagedImage(lineTransform, imageBm, edge);
            //Bitmap WithLines = fertig.ToManagedImage();


            //this.imageGray.Source = imageSrc.BitmapToImageSource(WithLines);
            //this.imageGray.Source = imageSrc.BitmapToImageSource(gsImage);
            //this.imageGray.Source = imageSrc.JpgToBitmapImage();
            //this.imageGray.Source = imageSrc.BitmapToImageSource(edge);
            //this.imageGray.Source = imageSrc.BitmapToImageSource(lineTransform.ToBitmap());

        }

        private static UnmanagedImage UnmanagedImage(HoughLineTransformation lineTransform, Bitmap imageBm, Bitmap edge)
        {
// get lines using relative intensity
            HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(0.5);
            UnmanagedImage fertig = AForge.Imaging.UnmanagedImage.FromManagedImage(imageBm);

            Debug.WriteLine("Found lines: " + lines.Length);
            var count = 0;
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
                t = (t/180)*Math.PI;
                // get image centers (all coordinate are measured relative
                // to center)
                int w2 = edge.Width/2;
                int h2 = edge.Height/2;
                double x0 = 0, x1 = 0, y0 = 0, y1 = 0;
                if (line.Theta != 0)
                {
                    // none-vertical line
                    x0 = -w2; // most left point
                    x1 = w2; // most right point
                    // calculate corresponding y values
                    y0 = (-Math.Cos(t)*x0 + r)/Math.Sin(t);
                    y1 = (-Math.Cos(t)*x1 + r)/Math.Sin(t);
                }
                else
                {
                    // vertical line
                    x0 = line.Radius;
                    x1 = line.Radius;
                    y0 = h2;
                    y1 = -h2;
                }

                if (line.Theta > 70 && line.Theta < 110)
                {
                    if (line.RelativeIntensity > 0.7)
                    {
                        count++;
                        AForge.Imaging.Drawing.Line(fertig,
                            new IntPoint((int) x0 + w2, h2 - (int) y0),
                            new IntPoint((int) x1 + w2, h2 - (int) y1),
                            System.Drawing.Color.Red);
                    }
                }
            }
            Debug.WriteLine("Found intensive: " + count);
            return fertig;
        }
    }
}
