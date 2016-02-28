using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using OpenCvSharp;
using OpenCvSharp.ML;
using Point = OpenCvSharp.Point;

//using OpenCvSharp.CPlusPlus;


namespace MeterImage
{
    class Program
    {
        const string path = @"C:\apps\OcrOpenCvNET\MeterImage\MeterImage\BilderMartin\pic1\";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            const string srcFilename = "GGG.jpg";

            try
            {
                //Bitmap image1 = (Bitmap) Image.FromFile(path + srcFilename, true);
                var fileName = path + srcFilename;
                Mat org = new Mat(fileName, ImreadModes.Color);
                // hier umkopier und nach grau anstatt d´doppelt
                Mat src = new Mat(fileName, ImreadModes.GrayScale); // OpenCvSharp 3.x
                Mat dst = new Mat();

                Console.WriteLine("size: row:" + src.Row + "  height: " + src.Height);

                // adaptive (dynamic) treshold
                Cv2.AdaptiveThreshold(src, dst, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 11, 2.0);
                    // was bringen die Nummern?
                dst.SaveImage(path + srcFilename.Replace(".jpg", "_") + "adaptiveTreshold.jpg");
                

                // Todo mb: Bild an Kanten begradigen

                HierarchyIndex[] hierarchyIndexes;
                Point[][] contours = GetContours(src, out hierarchyIndexes);
                Console.WriteLine("pic contours count Start:" + contours);
                // GetValue(contours, adaptiveTreshold, hierarchyIndexes);
                //adaptiveTreshold.SaveImage(path + srcFilename.Replace(".jpg", "_") + "conturs.jpg");

                // Filter contours Höhe > Breite
                var points = ContourRextHeightBiggerWeight(contours);

                var pointSize = ContoursDeleteToSmallOnes(points);

                Console.WriteLine("pic contours count after first filter:" + points.Count);

                //
                var pointInMiddel = CheckContursInPictureMiddel(pointSize, src);

                DrawContours(pointInMiddel, org, hierarchyIndexes);
                // evtl jetzt die y höhen behandlung???

                // todo ie rechteckhöhe sollte ungefähr gleich sein, bzw die meisten sind die passendne

                // zeichne Rechteck in orginal
                foreach(var drawRect in pointInMiddel)
                {
                    Rect rectShowing = Cv2.BoundingRect(drawRect);
                    Cv2.Rectangle(org, rectShowing, Scalar.Green,2,LineTypes.Filled);
                }
                
                Console.WriteLine("pic contours count after first filter:" + pointInMiddel.Count);

                // todo mb crop the rect conturs
                // Bitmap resPic = (Bitmap)Image.FromFile(fileName, true);
                var matchCount = 0;
                
                Console.WriteLine("matchCount: " + matchCount);
                // resPic.Save(path + @"polFil_1_Result.jpg");
                var resultPic = new Mat(path + @"polFil_1_Result.jpg");
                //new Window("resultPic", image: resultPic);
                //new Window("adaptiveTreshold", image: adaptiveTreshold);
                new Window("org", image: org);
                using (new Window("src", image: src))
                {
                    Cv2.WaitKey(0);
                }

            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine("There was an error opening the bitmap." +
                    "Please check the path.");
                throw;
            }

            Console.ReadKey();
        }

        private static List<Point[]> ContoursDeleteToSmallOnes(List<Point[]> points)
        {
            List<Point[]> pointSize = new List<Point[]>();
            foreach (var contour in points)
            {
                Rect rect = Cv2.BoundingRect(contour);

                if (rect.Height > 10 && rect.Width > 10)
                {
                    pointSize.Add(contour);
                }
            }
            return pointSize;
        }

        private static List<Point[]> ContourRextHeightBiggerWeight(Point[][] contours)
        {
            List<Point[]> points = new List<Point[]>();
            foreach (var contour in contours)
            {
                Rect rect = Cv2.BoundingRect(contour);
                if (rect.Height > rect.Width)
                {
                    points.Add(contour);
                }
            }
            return points;
        }

        private static List<Point[]> CheckContursInPictureMiddel(List<Point[]> points, Mat src)
        {
            List<Point[]> pointInMiddel = new List<Point[]>();
            foreach (var contour in points)
            {
                // y mittelwert berechnen
                var mittel = 0;
                foreach (var y in contour)
                {
                    mittel += y.Y;
                }
                mittel /= contour.Length;
                var imageHeight = src.Height;
                if (mittel > imageHeight/3 && mittel < imageHeight*2/3)
                {
                    pointInMiddel.Add(contour);
                }
            }
            return pointInMiddel;
        }

        private static void DrawContours(/*Point[][]*/List<Point[]> contours, Mat mat, HierarchyIndex[] hierarchyIndexes)
        {
            int contourIndex = 0;
            foreach (var i in contours)
            {
                Cv2.DrawContours(
                    mat,
                    contours,
                    contourIndex,
                    color: Scalar.Red, //.All(j+1),
                    thickness: -1,
                    lineType: LineTypes.Link8,
                    hierarchy: hierarchyIndexes,
                    maxLevel: 0 /*int.MaxValue*/);
                contourIndex = hierarchyIndexes[contourIndex].Next;
            }
        }

        private static Point[][] GetContours(Mat srcNumberOrg, out HierarchyIndex[] hierarchyIndexesOrg)
        {
            var dstNumberOrg = new Mat();
            Cv2.Canny(srcNumberOrg, dstNumberOrg, 80, 80); //src
            
            Point[][] contoursOrg;
            // HierarchyIndex[] hierarchyIndexesOrg;
            Cv2.FindContours(
                dstNumberOrg,
                out contoursOrg,
                out hierarchyIndexesOrg,
                mode: RetrievalModes.External, // nur die äußeren
                method: ContourApproximationModes.ApproxNone);
            Console.WriteLine("Conturs found: " + contoursOrg.Length);
            if (contoursOrg.Length == 0)
            {
                throw new NotSupportedException("Couldn't find any object in the image.");
            }

            return contoursOrg;
        }
    }
}