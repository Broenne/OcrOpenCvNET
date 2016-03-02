﻿using System;
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
        //const string path = @"C:\apps\OcrOpenCvNET\MeterImage\MeterImage\BilderMartin\pic1\";
        //const string srcFilename = "GGG.jpg";
        const string path = @"C:\apps\OcrOpenCvNET\MeterImage\MeterImage\BilderMartin\pic2\";
        const string srcFilename = "Snipet_WP_20160226_017.jpg";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            

            try
            {
                //Bitmap image1 = (Bitmap) Image.FromFile(path + srcFilename, true);
                var fileName = path + srcFilename;
                Mat org = new Mat(fileName, ImreadModes.Color);
                // hier umkopier und nach grau anstatt d´doppelt
                Mat src = new Mat(fileName, ImreadModes.GrayScale); // OpenCvSharp 3.x
               
                Console.WriteLine("size: row:" + src.Row + "  height: " + src.Height);

                // ACHTUNG!!!!!
                // Hier morph test!!! diese wird gemacht um die linie weg zu bekommen
                var srcMorph = ErodeMorpholgy(src);
                new Window("MorpH", image: srcMorph);

                // Todo mb: Bild an Kanten begradigen

                HierarchyIndex[] hierarchyIndexes;
                var contours = GetContours(srcMorph, out hierarchyIndexes);
                //var contours = GetContours(src, out hierarchyIndexes);
                Console.WriteLine("pic contours count Start:" + contours);
                // GetValue(contours, adaptiveTreshold, hierarchyIndexes);
                //adaptiveTreshold.SaveImage(path + srcFilename.Replace(".jpg", "_") + "conturs.jpg");


                List<Point[]> drawer = new List<Point[]>();
                foreach (var item in contours)
                {
                    drawer.Add(item);
                }
                DrawContours(drawer, org, hierarchyIndexes, Scalar.Red);


                // Filter contours Höhe > Breite
                var points = ContourRextHeightBiggerWeight(contours);

                var pointSize = ContoursDeleteToSmallOnes(points);
                
                var pointInMiddel = CheckContursInPictureMiddel(pointSize, src);
                // es darf jetzt eigentlich keine rechtecke mehr in rechtecken geben

                var pointsBySizeToOtherRects = PointsBySizeToOtherRects(pointSize);

                var contoursWithoutInner = RemoveInnerContourRects(pointsBySizeToOtherRects);

                //TODOExtractByXabstand(contoursWithoutInner);

                DrawContours(contoursWithoutInner, org, hierarchyIndexes, Scalar.AliceBlue);
                // evtl jetzt die y höhen behandlung???

                // todo ie rechteckhöhe sollte ungefähr gleich sein, bzw die meisten sind die passendne

                // zeichne Rechteck in orginal
                foreach(var drawRect in contoursWithoutInner)
                {
                    Rect rectShowing = Cv2.BoundingRect(drawRect);
                    Cv2.Rectangle(org, rectShowing, Scalar.Green,2,LineTypes.Filled);
                }
                
                Console.WriteLine("pic contours count after first filter:" + pointInMiddel.Count);

               // sortieren der Buchstaben / Contouren
                var orderedList = contoursWithoutInner
                    .OrderBy((x) => Cv2.BoundingRect(x).Left)
                    .ToList();

                foreach (var item in orderedList)
                {
                    var rect = Cv2.BoundingRect(item);
                    Console.WriteLine(rect.Left);
                }

                //////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////


                

                // croppedFaceImage = originalImage(faceRect).clone();
                var digitList = CropAndResizeTheImages(orderedList, src);

                // todo mb. gut wäre, wenn man einen etwas größeren auschnitt zeigen würde
                Console.WriteLine("Found Contours:" + digitList.Count);
                foreach (var digPic in digitList)
                {
                    var windowName = "cropedResize_" + new Random();

                    using (new Window(windowName, image: digPic))
                    {
                        Cv2.WaitKey(0);
                    }
                }


                // todo mb crop the rect conturs
                // Bitmap resPic = (Bitmap)Image.FromFile(fileName, true);
                var matchCount = 0;
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

        private static List<Mat> CropAndResizeTheImages(List<Point[]> orderedList, Mat src)
        {
            List<Mat> digitList = new List<Mat>();
            // todo mb: result list ist hier falsch??!! esmuss ein zwischenergbnis rauskommen
            
            var resultList = CropAndResize(orderedList, src);

            //var responseList = TrainTheDigitsToAList(resultList);


            //// train the list!!!!!!!!!!!!!!!!!!!!!!!!!
            //var kNearest = OpenCvSharp.ML.KNearest.Create();
            //var i = 0;
            //foreach (var item in resultList)
            //{
            //    //(InputArray samples, SampleTypes layout, InputArray responses
            //    kNearest.Train(item, SampleTypes.RowSample, responseList[i]);
            //}

            //Mat ocrResult = new Mat();

            //// sieht noch etwas unlogisch aus aber formt auch die dinger
            //Mat neighborResponses = new Mat();
            //Mat dists = new Mat();
            //var xxx = CropAndResize(orderedList, src);
            //foreach (var ppp in xxx)
            //{
            //    var lll = kNearest.FindNearest(ppp, 1, ocrResult, neighborResponses, dists);
            //}
            

            //// noch nach außerhalb sonst immer dieselbe datei
            //using (

            //    FileStorage fs = new FileStorage(path + "data", FileStorage.Mode.Write))
            //{
            //    //fs.
            //    //fs..WriteObj("result", result); //.GetFileNodeByName(null, nodeName);
            //                                   //matrix = new Mat(fs.Read<CvMat>(param));
            //}
            return digitList;
        }

        private static int TrainOneDigit(Mat digitMat, List<Mat> imageAndNumberInCombination)
        {
            //List<Mat> responseList = new List<Mat>();
            //foreach (var res in resultList)
            //{
            //}
            int key = 0;
            using (new Window("cropedResize", image: digitMat))
            {
                var keyAscii = (char)Cv2.WaitKey(0); // standard liest als ascii
                key = (int)Char.GetNumericValue(keyAscii);
                if (key >= 0 && key <= 9)
                {
                    // todo, das muss jetzt in eine neue Liste
                    var response = new Mat(1, 1, MatType.CV_32FC1, (float) key);// - '0');
                    imageAndNumberInCombination.Add(response);
                    //responseList.Add(response);
                }
            }
            return key;
        }


        private static List<Mat> CropAndResize(List<Point[]> orderedList, Mat src)
        {
            List<Mat> resultFloatList = new List<Mat>();
            List<Mat> cropedResizeList = new List<Mat>();
            foreach (Point[] digit in orderedList)
            {
                var toCrop = Cv2.BoundingRect(digit);
                Mat croped = new Mat(src, toCrop);
                Mat cropedResize = new Mat();

                OpenCvSharp.Size size = new OpenCvSharp.Size(600, 600);
                Cv2.Resize(croped, cropedResize, size); // hier muss jetzt das rect bzw die kontur rein

                cropedResizeList.Add(cropedResize);
                //resultList.Add(result);
            }

            resultFloatList = ConvertToFloatImage(cropedResizeList);

            // todo mb!!!!!!!!!!!!!
            //var responseList = TrainTheDigitsToAList(cropedResizeList);
            return resultFloatList;
        }

        // return the 
        private static List<Mat> ConvertToFloatImage(List<Mat> cropedList)
        {
            List<Mat> resultList = new List<Mat>();
            List<Mat> imageAndNumberInCombination = new List<Mat>();
            foreach (var cropedResize in cropedList)
            { 
                var result = cropedResize.Reshape(1, 1);
                result.ConvertTo(result, MatType.CV_32FC1); //convert to float
                resultList.Add(result);
                TrainOneDigit(cropedResize, imageAndNumberInCombination);
            }
            return resultList;
        }

        private static Mat ErodeMorpholgy(Mat src)
        {
            Mat morphHelper = new Mat();
            var srcMorph = src;
            for (var morphCount = 0; morphCount < 3; morphCount++)
            {
                Cv2.MorphologyEx(srcMorph, morphHelper, MorphTypes.ERODE, new Mat());
                srcMorph = morphHelper;
            }
            return srcMorph;
        }

        private static void TODOExtractByXabstand(List<Point[]> contoursWithoutInner)
        {
// sortiréren an Hand der linken KAnte
            List<int> middle = new List<int>();
            List<int> left = new List<int>();
            foreach (var item in contoursWithoutInner)
            {
                var rect = Cv2.BoundingRect(item);
                left.Add(rect.Left);
                //int xMiddle = (rect.Left - rect.Right)/2;
                //rect.Left;
                //middle.Add(xMiddle);
                //Console.WriteLine(xMiddle);
            }
            left.Sort();

            for (int i = left.Count - 1; i > 0; i--)
            {
                var dis = left[i] - left[i - 1];
                middle.Add(dis);
            }

            //var numbers = new List<int>();
            var median = GetMedian(middle);
            //noch die doppelten löschen? 

            // wenn der abstand unter halben median, dann welchen löschen????
            //List<Point[]> filterByMedian = new List<Point[]>();
            //foreach (var cont in contoursWithoutInner)
            //{
            //    Rect rectInner = Cv2.BoundingRect(cont);
            //    if()
            //}
        }

        private static double GetMedian(List<int> numbers)
        {
            numbers.Sort();
            int numberCount = numbers.Count();
            int halfIndex = numberCount/2;
            double median;
            if ((numberCount%2) == 0)
            {
                var help = (numbers.ElementAt(halfIndex) + numbers.ElementAt(halfIndex - 1));
                median = help/2;
            }
            else
            {
                median = numbers.ElementAt(halfIndex);
            }
            Debug.WriteLine(("Median is: " + median));
            return median;
        }

        private static List<Point[]> RemoveInnerContourRects(List<Point[]> pointsBySizeToOtherRects)
        {
            List<Point[]> contoursWithoutInner = new List<Point[]>();
            var count = 0;
            bool exist = false;
            foreach (var cont in pointsBySizeToOtherRects)
            {
                // es wird bei allen rechtecken geschaut, ob sie in einem anderen liegen
                Rect rectInner = Cv2.BoundingRect(cont);
                exist = false;
                foreach (var hhh in pointsBySizeToOtherRects)
                {
                    // hat rect inner noch was außen?, dann darf es nicht hinzugefügt werden
                    Rect rectOuter = Cv2.BoundingRect(hhh);
                    if (rectInner == rectOuter)
                    {
                        continue;
                    }
                    if (rectInner.Contains(rectOuter))
                    {
                        exist = true;
                        Console.WriteLine("outer one exists");
                    }
                }
                if (exist == false)
                {
                    contoursWithoutInner.Add(cont);
                }
                count++;
            }
            return contoursWithoutInner;
        }

        private static List<Point[]> PointsBySizeToOtherRects(List<Point[]> pointSize)
        {
// Berechnen der duchscnittshöhe
            var avareageHeight = 0.0;
            foreach (var cont in pointSize)
            {
                Rect rect = Cv2.BoundingRect(cont);
                avareageHeight += rect.Height;
            }
            avareageHeight /= pointSize.Count;
            Console.WriteLine("avareageHeight" + avareageHeight);
            List<Point[]> pointsBySizeToOtherRects = new List<Point[]>();
            foreach (var cont in pointSize)
            {
                Rect rect = Cv2.BoundingRect(cont);
                if (rect.Height > avareageHeight*0.7)
                {
                    pointsBySizeToOtherRects.Add(cont);
                }
            }
            return pointsBySizeToOtherRects;
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
                if (mittel > imageHeight/4 && mittel < imageHeight*1/4)
                {
                    pointInMiddel.Add(contour);
                }
            }
            return pointInMiddel;
        }

        private static void DrawContours(List<Point[]> contours, Mat mat, HierarchyIndex[] hierarchyIndexes, Scalar color)
        {
            int contourIndex = 0;
            foreach (var i in contours)
            {
                Cv2.DrawContours(
                    mat,
                    contours,
                    contourIndex,
                    color: color, //.All(j+1),
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

            // todo mb sobel könnte deutlich besser funktionieren?! siehe bild von aforge.net
            //Cv2.Sobel(srcNumberOrg, dstNumberOrg, MatType.CV_16S, 80, 80);
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