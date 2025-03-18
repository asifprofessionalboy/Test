private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        // Convert Bitmap to Mat
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Convert to grayscale
        CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
        {
            // Train recognizer with stored face
            faceRecognizer.Train(new List<Mat> { matStored }, new List<int> { 1 });

            // Predict the captured face
            var result = faceRecognizer.Predict(matCaptured);

            // If the distance score is low, it's a match
            return result.Label == 1 && result.Distance < 80; // Adjust threshold if needed
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
    }
}
private Mat BitmapToMat(Bitmap bitmap)
{
    Mat mat = new Mat();
    using (MemoryStream ms = new MemoryStream())
    {
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        mat = CvInvoke.Imdecode(ms.ToArray(), Emgu.CV.CvEnum.ImreadModes.Color);
    }
    return mat;
}

private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        using (var faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity)) 
        {
            // Convert Bitmaps to Grayscale EmguCV Images
            var grayCaptured = captured.ToImage<Gray, byte>();
            var grayStored = stored.ToImage<Gray, byte>();

            // Train the recognizer with the stored face
            faceRecognizer.Train(new List<Mat> { grayStored.Mat }, new Mat(new int[] { 1 }));

            // Predict the captured face
            var result = faceRecognizer.Predict(grayCaptured);

            // If the distance score is low, it's a match
            return result.Label == 1 && result.Distance < 5000;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Face verification error: " + ex.Message);
        return false;
    }
}



getting error on these 
var grayCaptured = new Image<Gray, byte>(captured);
var grayStored = new Image<Gray, byte>(stored);

Argument 1: cannot convert from 'System.Drawing.Bitmap' to 'byte[*,*,*]'

and on these 
faceRecognizer.Train(new Image<Gray, byte>[] { grayStored }, new int[] { 1 });

Argument 1: cannot convert from 'Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>[]' to 'Emgu.CV.IInputArrayOfArrays'
