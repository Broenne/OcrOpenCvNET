using System;
using System.Diagnostics;
using System.Drawing;
using OpenCvSharp;
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
                Mat src = new Mat(fileName, ImreadModes.GrayScale); // OpenCvSharp 3.x
                Mat dst = new Mat();

                Console.WriteLine("size: row:" + src.Row + "  height: " + src.Height);

                // adaptive treshold
                Cv2.AdaptiveThreshold(src, dst, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 11, 2.0);
                    // was bringen die Nummern?
                dst.SaveImage(path + srcFilename.Replace(".jpg", "_") + "adaptiveTreshold.jpg");

                var adaptiveTreshold = new Mat(path + srcFilename.Replace(".jpg", "_") + "adaptiveTreshold.jpg",
                    ImreadModes.GrayScale);

                var srcMorph = adaptiveTreshold;
                var dstHelper = new Mat();
                for (int i = 0; i < 50; i++)
                {
                    if (0 == i%2)
                    {
                        Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.DILATE, new Mat());
                        srcMorph = dstHelper;
                        Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.DILATE, new Mat());
                    }
                    if (1 == i%2)
                    {
                        Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.ERODE, new Mat());
                        srcMorph = dstHelper;
                        Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.ERODE, new Mat());
                    }
                    srcMorph = dstHelper;
                }
                for (int i = 0; i < 5; i++)
                {
                    Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.ERODE, new Mat());
                    srcMorph = dstHelper;
                }
                for (int i = 0; i < 5; i++)
                {
                    Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.DILATE, new Mat());
                    srcMorph = dstHelper;
                }
                srcMorph.SaveImage(path + srcFilename.Replace(".jpg", "_") + "morphology.jpg");

                Mat dstCanny = new Mat();
                Cv2.Canny(srcMorph, dstCanny, 80, 80); //src
                dstCanny.SaveImage(path + srcFilename.Replace(".jpg", "_") + "dstCanny.jpg");

                //var hhh= new OpenCvSharp.Point[][];
                Point[][] contours;
                HierarchyIndex[] hierarchyIndexes;
                Cv2.FindContours(
                    dstCanny,
                    out contours,
                    out hierarchyIndexes,
                    mode: RetrievalModes.List, // nur die äußeren
                    method: ContourApproximationModes.ApproxNone);
                if (contours.Length == 0)
                {
                    throw new NotSupportedException("Couldn't find any object in the image.");
                }
                int contourIndex = 0;
                foreach (var i in contours)
                {
                    Cv2.DrawContours(
                        srcMorph,
                        contours,
                        contourIndex,
                        color: Scalar.Red,//.All(j+1),
                        thickness: -1,
                        lineType: LineTypes.Link8,
                        hierarchy: hierarchyIndexes,
                        maxLevel: 0 /*int.MaxValue*/);
                    contourIndex = hierarchyIndexes[contourIndex].Next;
                }
                src.SaveImage(path + srcFilename.Replace(".jpg", "_") + "conturs.jpg");

                Mat srcNumberOrg = new Mat(path + @"Null.jpg", ImreadModes.GrayScale);
                var dstNumberOrg = new Mat();
                Cv2.Canny(srcNumberOrg, dstNumberOrg, 80, 80); //src
                Point[][] contoursOrg;
                HierarchyIndex[] hierarchyIndexesOrg;
                Cv2.FindContours(
                    dstNumberOrg,
                    out contoursOrg,
                    out hierarchyIndexesOrg,
                    mode: RetrievalModes.External, // nur die äußeren
                    method: ContourApproximationModes.ApproxNone);
                

                //Mat resPic = new Mat(path + @"polFil_1.jpg", ImreadModes.GrayScale); // OpenCvSharp 3.x
                //Mat resPic = new Mat(path + @"polFil_1.jpg", ImreadModes.GrayScale); // OpenCvSharp 3.x
                Bitmap resPic = (Bitmap)Image.FromFile(fileName, true);
                for (int contour = 0; contour < contours.Length; contour++)
                {
                    for (int contourOrg = 0; contourOrg < contoursOrg.Length; contourOrg++)
                    {
                        var oneContourArray = contoursOrg[contourOrg];
                        var minHeight = src.Height;
                        var maxHeight = 0;
                        var minWidth = src.Width;
                        var maxWidth = 0;
                        foreach (var point in oneContourArray)
                        {
                            if (point.X > maxHeight) maxHeight = point.X;
                            if (point.X < minHeight) minHeight = point.X;
                            if (point.Y > maxWidth) maxWidth = point.Y;
                            if (point.Y < minWidth) minWidth = point.Y;
                        };
                        var widthContur = maxWidth - minWidth;
                        var heightContur = maxHeight - minHeight;

                        // zu kleine konturen des orginal brauchen nicht untersucht werden, woher bekommt man im orginal nur eine Kontur???
                        if (widthContur > srcNumberOrg.Width * 0.3 && heightContur > srcNumberOrg.Height * 0.4)
                        {
                            double distanceToResult = Cv2.MatchShapes(contours[contour], contoursOrg[contourOrg],ShapeMatchModes.I1,0.0);
                            Console.WriteLine("res: " + distanceToResult);
                            if(distanceToResult < 1)
                            { 
                                foreach (var pix in contours[contour])
                                {
                                    resPic.SetPixel(pix.X,pix.Y,Color.Red);
                                }
                            }
                        }
                    }
                }
                resPic.Save(path + @"polFil_1_Result.jpg");

            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine("There was an error opening the bitmap." +
                    "Please check the path.");
                throw;
            }

            //Console.ReadKey();
        }

        
    }
}
//public double Heights(Point[][] contour)
//{
////    contour[].Length

////    var minHeight = src.Height;
////    var maxHeight = 0;
////    var minWidth = src.Width;
////    var maxWidth = 0;
////    // do the contour blck
////    foreach (Point[][] pix in contour)
////    {
////        if (pix.X > maxHeight) maxHeight = pix.X;
////        if (pix.X < minHeight) minHeight = pix.X;
////        if (pix.Y > maxWidth) maxWidth = pix.Y;
////        if (pix.Y < minWidth) minWidth = pix.Y;
////        //helperImage.SetPixel(pix.X, pix.Y, Color.Red);
////    }
//    return 42;
//}


//public void draw(Point[][] contour)
//{
//    Mat src = new Mat(path + @"polFil_1.jpg", ImreadModes.GrayScale); // OpenCvSharp 3.x
//    var minHeight = src.Height;
//    var maxHeight = 0;
//    var minWidth = src.Width;
//    var maxWidth = 0;
//    // do the contour blck
//    //foreach (Point[][] pix in contour)
//    //{
//    //    if (pix.X > maxHeight) maxHeight = pix.X;
//    //    if (pix.X < minHeight) minHeight = pix.X;
//    //    if (pix.Y > maxWidth) maxWidth = pix.Y;
//    //    if (pix.Y < minWidth) minWidth = pix.Y;
//    //    //helperImage.SetPixel(pix.X, pix.Y, Color.Red);
//    //}

//    //for (int i = minHeight; i < maxHeight; i++)
//    //{
//    //    helperImage.SetPixel(i, maxWidth, Color.Red);
//    //    helperImage.SetPixel(i, minWidth, Color.Red);
//    //}

//    //for (int i = minWidth; i < maxWidth; i++)
//    //{
//    //    helperImage.SetPixel(minHeight, i, Color.Green);
//    //    helperImage.SetPixel(maxHeight, i, Color.Green);
//    //}

//    //helperImage.Save(path +  "helperImage.jpg");
//    //Console.WriteLine("groesser: " + contour.Length);
//}

//// jetzt hat man das contour array einer kontur
//for (int row = 0; row < src.Rows; row++)
//{
//    for (int column = 0; column < src.Height; column++)
//    {
//        helperImage.SetPixel(row, column, Color.White);
//    }
//}




//foreach (var contour in contours)
//{
//    // wähle nur grße konturen
//    if (contour.Length > 400 /*&& contour.Length < src.Width*/ ) // wenn das bild gut ausgeschnittenn ist,....
//    {


//    }
//}