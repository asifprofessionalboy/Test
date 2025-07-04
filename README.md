public static Bitmap MatToBitmap(Mat mat)
{
    return mat.ToImage<Bgr, byte>().ToBitmap();
}

private float[] RunFaceNetEmbedding(Mat faceMat, string modelPath)
{
    Bitmap resizedBmp = MatToBitmap(faceMat);

    var input = new DenseTensor<float>(new[] { 1, 3, 160, 160 });

    for (int y = 0; y < 160; y++)
    {
        for (int x = 0; x < 160; x++)
        {
            var pixel = resizedBmp.GetPixel(x, y);
            input[0, 0, y, x] = (pixel.R / 127.5f) - 1.0f;
            input[0, 1, y, x] = (pixel.G / 127.5f) - 1.0f;
            input[0, 2, y, x] = (pixel.B / 127.5f) - 1.0f;
        }
    }

    using var session = new InferenceSession(modelPath);
    var inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor("input", input)
    };

    using var results = session.Run(inputs);
    return results.First().AsEnumerable<float>().ToArray();
}



this is my full code i am getting the same error on this line 
mat.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
'Mat' does not contain a definition for 'Bitmap' and no accessible extension method 'Bitmap' accepting a first argument of type 'Mat' could be found (are you missing a using directive or an assembly reference?)
  
  private bool VerifyFace(Bitmap captured, Bitmap stored)
    {
        try
        {
            var face1 = ExtractFaceEmbedding(captured);
            var face2 = ExtractFaceEmbedding(stored);

            if (face1 == null || face2 == null) return false;

            double distance = CalculateEuclideanDistance(face1, face2);

            Console.WriteLine($"Face distance: {distance}");

            return distance < 0.8; 
        }
        catch
        {
            return false;
        }
    }

    private float[] ExtractFaceEmbedding(Bitmap image)
    {
        string protoPath = Path.Combine("wwwroot", "Models", "deploy.prototxt");
        string modelPath = Path.Combine("wwwroot", "Models", "res10_300x300_ssd_iter_140000.caffemodel");
        string faceNetPath = Path.Combine("wwwroot", "Models", "facenet.onnx");

        var mat = BitmapToMat(image);
        var net = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);
        var blob = DnnInvoke.BlobFromImage(mat, 1.0, new Size(300, 300), new MCvScalar(104, 177, 123));
        net.SetInput(blob);

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

        return null;
    }
    public static Bitmap MatToBitmap(Mat mat)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            mat.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return new Bitmap(ms);
        }
    }
    private float[] RunFaceNetEmbedding(Mat faceMat, string modelPath)
    {
       Bitmap resizedBmp = MatToBitmap(faceMat);
        var input = new DenseTensor<float>(new[] { 1, 3, 160, 160 });

        for (int y = 0; y < 160; y++)
        {
            for (int x = 0; x < 160; x++)
            {
                var pixel = resizedBmp.GetPixel(x, y);
                input[0, 0, y, x] = (pixel.R / 127.5f) - 1.0f;
                input[0, 1, y, x] = (pixel.G / 127.5f) - 1.0f;
                input[0, 2, y, x] = (pixel.B / 127.5f) - 1.0f;
            }
        }

        var inputs = new List<NamedOnnxValue>
{
    NamedOnnxValue.CreateFromTensor("input", input)
};

        using var session = new InferenceSession(modelPath);
        using var results = session.Run(inputs);

        return results.First().AsEnumerable<float>().ToArray();
    }

    private double CalculateEuclideanDistance(float[] emb1, float[] emb2)
    {
        double sum = 0;
        for (int i = 0; i < emb1.Length; i++)
            sum += Math.Pow(emb1[i] - emb2[i], 2);
        return Math.Sqrt(sum);
    }
