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
