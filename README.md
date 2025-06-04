private double CompareImagesHistogram(Mat img1, Mat img2)
{
    Mat hsv1 = new Mat();
    Mat hsv2 = new Mat();

    CvInvoke.CvtColor(img1, hsv1, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);
    CvInvoke.CvtColor(img2, hsv2, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);

    int[] histSize = { 50, 60 };
    float[] ranges = { 0, 180, 0, 256 }; // flattened RangeF array
    int[] channels = { 0, 1 };

    Mat hist1 = new Mat();
    Mat hist2 = new Mat();

    using (var vec1 = new VectorOfMat())
    using (var vec2 = new VectorOfMat())
    {
        vec1.Push(hsv1);
        vec2.Push(hsv2);

        CvInvoke.CalcHist(vec1, channels, null, hist1, histSize, ranges, true);
        CvInvoke.CalcHist(vec2, channels, null, hist2, histSize, ranges, true);
    }

    CvInvoke.Normalize(hist1, hist1, 0, 1, Emgu.CV.CvEnum.NormType.MinMax);
    CvInvoke.Normalize(hist2, hist2, 0, 1, Emgu.CV.CvEnum.NormType.MinMax);

    return CvInvoke.CompareHist(hist1, hist2, Emgu.CV.CvEnum.HistogramCompMethod.Correl);
}




getting error on these two  
CvInvoke.CalcHist(new Mat[] { hsv1 }, channels, null, hist1, histSize, ranges, true);
 CvInvoke.CalcHist(new Mat[] { hsv2 }, channels, null, hist2, histSize, ranges, true);


Argument 1: cannot convert from 'Emgu.CV.Mat[]' to 'Emgu.CV.IInputArrayOfArrays
Argument 6: cannot convert from 'Emgu.CV.Structure.RangeF[]' to 'float[]'
