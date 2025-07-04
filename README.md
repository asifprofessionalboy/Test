public static Bitmap MatToBitmap(Mat mat)
{
    using (MemoryStream ms = new MemoryStream())
    {
        mat.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        return new Bitmap(ms);
    }
}

Bitmap resizedBmp = MatToBitmap(face);
private float[] RunFaceNetEmbedding(Bitmap bmp, string modelPath)
{
    var input = new DenseTensor<float>(new[] { 1, 3, 160, 160 });

    for (int y = 0; y < 160; y++)
    {
        for (int x = 0; x < 160; x++)
        {
            var pixel = bmp.GetPixel(x, y);
            input[0, 0, y, x] = (pixel.R / 127.5f) - 1.0f;
            input[0, 1, y, x] = (pixel.G / 127.5f) - 1.0f;
            input[0, 2, y, x] = (pixel.B / 127.5f) - 1.0f;
        }
    }

    using var session = new InferenceSession(modelPath);
    var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", input) };
    using var results = session.Run(inputs);
    return results.First().AsEnumerable<float>().ToArray();
}




Mat detection = net.Forward();
float[,,,] data = (float[,,,])detection.GetData();

for (int i = 0; i < data.GetLength(2); i++)
{
    float confidence = data[0, 0, i, 2];

    if (confidence > 0.6f)
    {
        int x1 = (int)(data[0, 0, i, 3] * mat.Cols);
        int y1 = (int)(data[0, 0, i, 4] * mat.Rows);
        int x2 = (int)(data[0, 0, i, 5] * mat.Cols);
        int y2 = (int)(data[0, 0, i, 6] * mat.Rows);

        Rectangle faceRect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
        Mat face = new Mat(mat, faceRect);
        CvInvoke.Resize(face, face, new Size(160, 160));
        return RunFaceNetEmbedding(face, faceNetPath);
    }
}




getting error on these, cannot convert from 'int[]' to 'bool'	

 float confidence = (float)detections.GetData(new[] { 0, 0, i, 2 });
 int x1 = (int)((float)detections.GetData(new[] { 0, 0, i, 3 }) * mat.Cols);
 int y1 = (int)((float)detections.GetData(new[] { 0, 0, i, 4 }) * mat.Rows);
 int x2 = (int)((float)detections.GetData(new[] { 0, 0, i, 5 }) * mat.Cols);
 int y2 = (int)((float)detections.GetData(new[] { 0, 0, i, 6 }) * mat.Rows);

and on this i am getting this line, error 'Mat' does not contain a definition for 'ToBitmap' and no accessible extension method 'ToBitmap' accepting a first argument of type 'Mat' could be found (are you missing a using directive or an assembly reference?)

Bitmap resizedBmp = faceMat.ToBitmap();
