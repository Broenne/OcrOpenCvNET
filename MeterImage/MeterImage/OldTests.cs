using System;
using System.Drawing;
using OpenCvSharp;
using Point = OpenCvSharp.Point;

class PointArrayHelper
{
    public int Height { get; private set; }
    public int Width { get; private set; }

    public void ArraySize(Mat src, Point[] oneContourArray)
    {
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
        }
        this.Width = maxWidth - minWidth;
        this.Height = maxHeight - minHeight;
    }
}


//// adaptive (dynamic) treshold
//Cv2.AdaptiveThreshold(src, dst, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 11, 2.0);
//    // was bringen die Nummern?
//dst.SaveImage(path + srcFilename.Replace(".jpg", "_") + "adaptiveTreshold.jpg");


//Console.WriteLine("matchCount: " + matchCount);
//                // resPic.Save(path + @"polFil_1_Result.jpg");
//                var resultPic = new Mat(path + @"polFil_1_Result.jpg");
////new Window("resultPic", image: resultPic);
////new Window("adaptiveTreshold", image: adaptiveTreshold);

//private static Mat SrcMorph(Mat adaptiveTreshold, string srcFilename)
//{
//    var srcMorph = adaptiveTreshold;
//    var dstHelper = new Mat();
//    for (int i = 0; i < 100; i++)
//    {
//        if (0 == i % 2)
//        {
//            Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.DILATE, new Mat());
//            srcMorph = dstHelper;
//            Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.DILATE, new Mat());
//        }
//        if (1 == i % 2)
//        {
//            Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.ERODE, new Mat());
//            srcMorph = dstHelper;
//            Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.ERODE, new Mat());
//        }
//        srcMorph = dstHelper;
//    }
//    //for (int i = 0; i < 5; i++)
//    //{
//    //    Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.ERODE, new Mat());
//    //    srcMorph = dstHelper;
//    //}
//    //for (int i = 0; i < 5; i++)
//    //{
//    //    Cv2.MorphologyEx(srcMorph, dstHelper, MorphTypes.DILATE, new Mat());
//    //    srcMorph = dstHelper;
//    //}
//    srcMorph.SaveImage(path + srcFilename.Replace(".jpg", "_") + "morphology.jpg");
//    return srcMorph;
//}

//private static Color RandomColor()
//{
//    Random randomGen = new Random();
//    KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
//    KnownColor randomColorName = names[randomGen.Next(names.Length)];
//    Color randomColor = Color.FromKnownColor(randomColorName);
//    return randomColor;
//}


//for (int contour = 0; contour < contours.Length; contour++)
//{
//    var rect = Cv2.BoundingRect(contours[contour]);
//    if (rect.Height > rect.Width)
//    { 
//        for (int contourOrg = 0; contourOrg < contoursOrg.Length; contourOrg++)
//        {
//            var oneContourOrgArray = contoursOrg[contourOrg];
//            var rectOrg = Cv2.BoundingRect(oneContourOrgArray);
//            // zu kleine konturen des orginal brauchen nicht untersucht werden, woher bekommt man im orginal nur eine Kontur???
//            //if (rect.Width > srcNumberOrg.Width * 0.3 && rect.Height > srcNumberOrg.Height * 0.4)
//                if (rectOrg.Height > rectOrg.Width)
//                {
//                    var meter=contours[contour];
//                    var org = contoursOrg[contourOrg];
//                    double distanceToResult = Cv2.MatchShapes(meter, org, ShapeMatchModes.I3, 0.0);

//                    var roi = new Mat(src, rectOrg); //Crop the image
//                    // http://www.dotnettips.info/post/2131/opencvsharp-18
//                    var results = new Mat();
//                    var neighborResponses = new Mat();
//                    var dists = new Mat();
//                    var resizedImage = new Mat();
//                    var resizedImageFloat = new Mat();
//                    Cv2.Resize(roi, resizedImage, new OpenCvSharp.Size(10, 10)); //resize to 10X10
//                    resizedImage.ConvertTo(resizedImageFloat, MatType.CV_32FC1); //convert to float
//                    var resultResize = resizedImageFloat.Reshape(1, 1);
//                    var kNeareast = OpenCvSharp.ML.KNearest.Create();
//                    var result = kNeareast.FindNearest(resultResize, 1, results, neighborResponses, dists);

//                    Console.WriteLine("res: " + distanceToResult);
//                    // if(distanceToResult > 0 && distanceToResult<2)
//                    if(distanceToResult > 40)
//                    {
//                        var randomColor = Color.Red;//RandomColor();
//                            matchCount++;
//                            foreach (var pix in meter)
//                            {
//                                resPic.SetPixel(pix.X,pix.Y, randomColor);
//                            }
//                        }
//                    }
//            }
//        }
//}


// Test mit "OrginalBild_Null"
//Mat srcNumberOrg = new Mat(path + @"Null.jpg", ImreadModes.GrayScale);
//var srcMorphOrg = SrcMorph(srcNumberOrg, srcFilename);
//HierarchyIndex[] hierarchyIndexesOrg;
//var contoursOrg = GetContours(srcMorphOrg, out hierarchyIndexesOrg);
//Console.WriteLine("orginal contours count:"+ contoursOrg);
//srcMorphOrg.SaveImage(path + "Null_Morph.jpg");


//Mat adaptiveTreshold = new Mat(path + srcFilename.Replace(".jpg", "_") + "adaptiveTreshold.jpg",
//   ImreadModes.GrayScale);
//var srcMorph = SrcMorph(adaptiveTreshold, srcFilename);



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