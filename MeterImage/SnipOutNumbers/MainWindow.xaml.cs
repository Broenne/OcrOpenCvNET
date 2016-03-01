using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using Autofac.Core;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;

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
            //var imageSrc = new ImageHelper(@"C:\apps\OcrOpenCvNET\MeterImage\MeterImage\BilderMartin\pic1\WP_20160226_006.jpg");
            var imageSrc = new ImageHelper(@"C:\apps\OcrOpenCvNET\MeterImage\MeterImage\BilderMartin\pic2\WP_20160226_017.jpg");
            this.image.Source = imageSrc.JpgToBitmapImage();
            // create filter
            //HomogenityEdgeDetector filter = new HomogenityEdgeDetector();
            // apply the filter

            Bitmap imageBm = imageSrc.JpgToBitmap();
            Bitmap gsImage = Grayscale.CommonAlgorithms.BT709.Apply(imageBm);
            var filter = new SobelEdgeDetector();//new CannyEdgeDetector();
            Bitmap edge = filter.Apply(gsImage);

            //// locating objects
            //BlobCounter blobCounter = new BlobCounter();
            //blobCounter.FilterBlobs = true;
            //blobCounter.MinHeight = 5;
            //blobCounter.MinWidth = 5;
            //blobCounter.ProcessImage(edge);
            //Blob[] blobs = blobCounter.GetObjectsInformation();
            //// check for rectangles
            //SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            //// create convex hull searching algorithm
            //GrahamConvexHull hullFinder = new GrahamConvexHull();
            //// Bitmap tempBitmap = new Bitmap(imageBm.Width, imageBm.Height);
            //// lock image to draw on it
            //BitmapData data = imageBm.LockBits(
            //    new Rectangle(0, 0, imageBm.Width, imageBm.Height),
            //        ImageLockMode.ReadWrite, imageBm.PixelFormat);

            //List<IntPoint> edgePoints = new List<IntPoint>();
            //List<System.Drawing.Point> Points = new List<Point>();
            //foreach (var blob in blobs)
            //{
            //    List<IntPoint> leftPoints, rightPoints;//, edgePoints;

            //    // get blob's edge points
            //    blobCounter.GetBlobsLeftAndRightEdges(blob,//blobs[i]
            //        out leftPoints, out rightPoints);
            //    edgePoints.AddRange(leftPoints);
            //    edgePoints.AddRange(rightPoints);
            //    // blob's convex hull
            //    List<IntPoint> hull = hullFinder.FindHull(edgePoints);
            //    AForge.Imaging.Drawing.Polygon( data, hull, System.Drawing.Color.Red );
            //    //    }
            //    //}
            //}
            //imageBm.UnlockBits(data);
            ////Graphics g = Graphics.FromImage(/*imageSrc.myImg*/tempBitmap);
            ////g.DrawPolygon(new System.Drawing.Pen(System.Drawing.Color.Red, 5.0f), Points.ToArray());
            //this.imageGray.Source = imageSrc.BitmapToImageSource(imageBm);
            ////imageSrc.myImg.Save("result.png");

            imageGray.Source = imageSrc.BitmapToImageSource(this.DetectCorners(imageBm));

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


        public Bitmap DetectCorners(Bitmap image)
        {
            // Load image and create everything you need for drawing
            //Bitmap image = new Bitmap(@"myimage.jpg");
            Graphics graphics = Graphics.FromImage(image);
            SolidBrush brush = new SolidBrush(System.Drawing.Color.Red);
            System.Drawing.Pen pen = new System.Drawing.Pen(brush);

            // Create corner detector and have it process the image
            MoravecCornersDetector mcd = new MoravecCornersDetector();
            List<IntPoint> corners = mcd.ProcessImage(image);

            SortedDictionary<uint, int> histogram = new SortedDictionary<uint, int>();

            foreach (var item in corners.Where(x=>x.Y == x.Y))
            {
                var helper = (uint) item.Y;
                helper /= 10;// so jetzt machen wir es ungenau!!!!

                if (histogram.ContainsKey(helper))
                {
                    histogram[helper]++; // anzahl erhöhen in welchem y-bereich es viele gibt
                }
                else {
                    histogram[helper] = 1;
                }
            }

            // hier wird danach sortiert, wo es die meisten xPunkte gibt, allerding geteilt durch da oben
            //histogram.OrderBy(x => x.Value);
            histogram.ToList();
            SortedList<uint, int> histogramSortedList= new SortedList<uint, int>(histogram);
            //var help = histogramSortedList.OrderBy(x => x.Value);
            //histogramSortedList = help;
            var bbb=histogramSortedList.OrderBy(x => x.Value);
            // list umdrehen, nach den meisten....
            List<KeyValuePair<uint,int>> tops = new List<KeyValuePair<uint, int>>();
            uint j =199; //TODO Wie hier an die 199 kommen???
            // achtung, warum schwankt der count?????????
            // count . Nummer ist die ANZHAL!!!!!!!!!!!
            //for (uint i = (uint)bbb.Count() - 1; i > bbb.Count()-20; i--, j--)
            //{
            foreach(var item in bbb)
            { 
                //histogramSortedList.IndexOfKey(23);
                tops.Add(new KeyValuePair<uint, int>(item.Key,item.Value)); // hier nicht value sondern key
            }
            var hhh = tops.ToArray();

            // Top 10
            List<KeyValuePair<uint, int>> Top10 = new List<KeyValuePair<uint, int>>();
            for (int i=tops.Count;i>tops.Count-10;i--)
            {
                Top10.Add(hhh[i-1]);
            }

            //var res = Top10.ToList();
            
            
            pen.Width = 5;
            for(int i=0;i< Top10.Count;i++)
            {
                graphics.DrawLine(pen,new Point(0, (int)Top10[i].Key*10),new Point(image.Width, (int)Top10[i].Key * 10));
                
            }

            //pen.Color = Color.Blue;
            //foreach (IntPoint corner in corners)
            //{
            //    graphics.DrawRectangle(pen, corner.X - 1, corner.Y - 1, 10, 10);
            //}

            // Display
            return image;
        }

        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)

        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }



        //// use the shape checker to extract the corner points
        //if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints))
        //{
        //    // only do things if the corners form a rectangle
        //    if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle)
        //    {
        // here i use the graphics class to draw an overlay, but you
        // could also just use the cornerPoints list to calculate your
        //// x, y, width, height values.
        //Points = new List<System.Drawing.Point>();
                //foreach (var point in edgePoints)//cornerPoints)
                //{
                //    Points.Add(new System.Drawing.Point(point.X, point.Y));


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
