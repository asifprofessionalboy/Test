https://github.com/cmusatyalab/openface?utm_source=chatgpt.com

https://cmusatyalab.github.io/openface/models-and-accuracies?utm_source=chatgpt.com

Array detectionArray = detections.GetData(); // Get raw data
float[] detectionData = detectionArray.Cast<float>().ToArray(); // Convert to 1D array





getting this error System.InvalidCastException: 'Unable to cast object of type 'System.Single[,,,]' to type 'System.Single[]'.'
 on this line  float[] detectionData = (float[])detections.GetData(); 
