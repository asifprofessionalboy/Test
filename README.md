private float[] RunFaceNetEmbedding(Mat faceMat, string modelPath)
{
    Bitmap resizedBmp = MatToBitmap(faceMat);
    var input = new DenseTensor<float>(new[] { 1, 3, 112, 112 });

    for (int y = 0; y < 112; y++)
    {
        for (int x = 0; x < 112; x++)
        {
            var pixel = resizedBmp.GetPixel(x, y);
            input[0, 0, y, x] = (pixel.R / 127.5f) - 1.0f;
            input[0, 1, y, x] = (pixel.G / 127.5f) - 1.0f;
            input[0, 2, y, x] = (pixel.B / 127.5f) - 1.0f;
        }
    }

    // ✅ Load the ONNX model
    using var session = new InferenceSession(modelPath);

    // ✅ Get the actual input name dynamically
    string inputName = session.InputMetadata.Keys.First();

    var inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor(inputName, input)
    };

    using var results = session.Run(inputs);

    return results.First().AsEnumerable<float>().ToArray();
}




Microsoft.ML.OnnxRuntime.OnnxRuntimeException: '[ErrorCode:InvalidArgument] Input name: 'input' is not in the metadata'
