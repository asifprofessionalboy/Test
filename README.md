private Mat BitmapToMat(Bitmap bitmap)
{
    // Convert Bitmap to MemoryStream
    using (MemoryStream ms = new MemoryStream())
    {
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        byte[] imageData = ms.ToArray();

        // Decode image from byte array to Mat
        Mat mat = new Mat();
        CvInvoke.Imdecode(new VectorOfByte(imageData), Emgu.CV.CvEnum.ImreadModes.Grayscale, mat);

        return mat;
    }
}



using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

[HttpPost]
public IActionResult AttendanceData(string EntryType, string ImageData)
{
    try
    {
        var UserId = HttpContext.Request.Cookies["Session"];
        string Pno = UserId;

        if (string.IsNullOrEmpty(ImageData))
        {
            return Json(new { success = false, message = "Captured image is required!" });
        }

        // Convert Base64 Image to Byte Array
        byte[] imageBytes = Convert.FromBase64String(ImageData.Split(',')[1]);

        using (var ms = new MemoryStream(imageBytes))
        {
            Bitmap capturedImage = new Bitmap(ms);

            // Retrieve stored image from database
            var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
            if (user == null || user.Image == null)
            {
                return Json(new { success = false, message = "User Image Not Found!" });
            }

            using (var storedStream = new MemoryStream(user.Image))
            {
                Bitmap storedImage = new Bitmap(storedStream);

                // Perform Face Recognition
                bool isFaceMatched = VerifyFace(capturedImage, storedImage);

                if (!isFaceMatched)
                {
                    return Json(new { success = false, message = "Face does not match!" });
                }

                // If face matches, store attendance
                string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                string currentTime = DateTime.Now.ToString("HH:mm");

                if (EntryType == "Punch In")
                {
                    StoreData(currentDate, currentTime, null, Pno);
                }
                else
                {
                    StoreData(currentDate, null, currentTime, Pno);
                }

                return Json(new { success = true, message = "Attendance Marked Successfully!" });
            }
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Error: " + ex.Message });
    }
}

private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Convert to grayscale for better face recognition
        CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        // Initialize LBPH face recognizer
        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
        {
            // Train the recognizer with stored image
            VectorOfMat trainingImages = new VectorOfMat();
            trainingImages.Push(matStored);

            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);

            // Predict the face match
            var result = faceRecognizer.Predict(matCaptured);

            Console.WriteLine($"Prediction Result: Label = {result.Label}, Distance = {result.Distance}");

            // Face match condition: label should be 1 (correct user) and distance should be low
            return result.Label == 1 && result.Distance < 50; // Lower distance means better match
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
    }
}

// Convert Bitmap to Mat
private Mat BitmapToMat(Bitmap bitmap)
{
    Mat mat = new Mat();
    using (MemoryStream ms = new MemoryStream())
    {
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        mat = CvInvoke.Imdecode(ms.ToArray(), Emgu.CV.CvEnum.ImreadModes.Grayscale);
    }
    return mat;
}




this is logic 

  [HttpPost]
  public IActionResult AttendanceData(string EntryType, string ImageData)
  {
      try
      {
          var UserId = HttpContext.Request.Cookies["Session"];
          string Pno = UserId;

          // Convert Base64 Image Data to Byte Array
          byte[] imageBytes = Convert.FromBase64String(ImageData.Split(',')[1]);
          using (var ms = new MemoryStream(imageBytes))
          {
              Bitmap capturedImage = new Bitmap(ms);

              // Retrieve the stored image from the database
              var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
              if (user == null || user.Image == null)
              {
                  return Json(new { success = false, message = "User Image Not Found!" });
              }

              // Convert stored image to Bitmap
              using (var storedStream = new MemoryStream(user.Image))
              {
                  Bitmap storedImage = new Bitmap(storedStream);

                  // Perform Face Recognition
                  bool isFaceMatched = VerifyFace(capturedImage, storedImage);

                  if (!isFaceMatched)
                  {
                      return Json(new { success = false, message = "Face does not match!" });
                  }

                  // If face matches, store attendance
                  string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                  string currentTime = DateTime.Now.ToString("HH:mm");

                  if (EntryType == "Punch In")
                  {
                      StoreData(currentDate, currentTime, null, Pno);
                  }
                  else
                  {
                      StoreData(currentDate, null, currentTime, Pno);
                  }

                  return Json(new { success = true, message = "Attendance Marked Successfully!" });
              }
          }
      }
      catch (Exception ex)
      {
          return Json(new { success = false, message = ex.Message });
      }
  }

  private bool VerifyFace(Bitmap captured, Bitmap stored)
  {
      try
      {
          
          Mat matCaptured = BitmapToMat(captured);
          Mat matStored = BitmapToMat(stored);

         
          CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
          CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

          using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
          {
              
              faceRecognizer.Train(new List<Mat> { matStored }, new List<int> { 1 });

              
              var result = faceRecognizer.Predict(matCaptured);

             
              return result.Label == 1 && result.Distance < 80; 
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

please provide success logic and proper logic 
