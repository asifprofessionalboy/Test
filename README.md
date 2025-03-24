public void StoreData(string ddMMyy, string tm, string Pno, string capturedImage)
{
    using (var connection = new SqlConnection(configuration.GetConnectionString("RFID")))
    {
        connection.Open();

        if (!string.IsNullOrEmpty(tm))
        {
            // Attendance logic if needed
        }

        if (!string.IsNullOrEmpty(capturedImage))
        {
            Guid ID = Guid.NewGuid(); // Unique ID for the record

            // Convert base64 to binary and save it as a .txt file
            byte[] imageBytes = Convert.FromBase64String(capturedImage.Split(',')[1]);
            string fileName = $"{ID}_{Pno}.txt";
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CapturedImage");
            string filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            System.IO.File.WriteAllBytes(filePath, imageBytes); // Save binary data

            // Insert record into the database
            var query = @"
            INSERT INTO App_ImageDetail(ID, Pno, FileName) 
            VALUES (@ID, @Pno, @FileName)";

            var parameters = new
            {
                ID = ID,
                Pno = Pno,
                FileName = fileName
            };

            connection.Execute(query, parameters);
        }
    }
}

[HttpPost]
public IActionResult AttendanceData([FromBody] AttendanceRequest model)
{
    try
    {
        var UserId = HttpContext.Request.Cookies["Session"];
        var UserName = HttpContext.Request.Cookies["UserName"];
        if (string.IsNullOrEmpty(UserId))
            return Json(new { success = false, message = "User session not found!" });

        string Pno = UserId;
        string Name = UserName;

        string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-{Name}.jpg");
        if (!System.IO.File.Exists(storedImagePath))
        {
            return Json(new { success = false, message = "Stored image not found!" });
        }

        string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");

        // Save captured image
        SaveBase64ImageToFile(model.ImageData, capturedImagePath);

        bool isFaceMatched = false;

        using (Bitmap capturedImage = new Bitmap(capturedImagePath))
        using (Bitmap storedImage = new Bitmap(storedImagePath))
        {
            isFaceMatched = VerifyFace(capturedImage, storedImage);
        }

        if (isFaceMatched)
        {
            string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
            string currentTime = DateTime.Now.ToString("HH:mm");

            // Store Data in Database & Save Captured Image as a Binary File
            StoreData(currentDate, currentTime, Pno, model.ImageData);

            return Json(new { success = true, message = "Attendance recorded successfully." });
        }
        else
        {
            return Json(new { success = false, message = "Face does not match!" });
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}


this is my methods  
[HttpPost]
 public IActionResult AttendanceData([FromBody] AttendanceRequest model)
 {
     try
     {
         var UserId = HttpContext.Request.Cookies["Session"];
         var UserName = HttpContext.Request.Cookies["UserName"];
         if (string.IsNullOrEmpty(UserId))
             return Json(new { success = false, message = "User session not found!" });

         string Pno = UserId;
         string Name = UserName;

         string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-{Name}.jpg");
         if (!System.IO.File.Exists(storedImagePath))
         {
             return Json(new { success = false, message = "Stored image not found!" });
         }


         string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");


         SaveBase64ImageToFile(model.ImageData, capturedImagePath);

         bool isFaceMatched = false;


         using (Bitmap capturedImage = new Bitmap(capturedImagePath))
         using (Bitmap storedImage = new Bitmap(storedImagePath))
         {
             isFaceMatched = VerifyFace(capturedImage, storedImage);
         }


         System.IO.File.Delete(capturedImagePath);

         if (isFaceMatched)
         {
             string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
             string currentTime = DateTime.Now.ToString("HH:mm");

             if (model.Type == "Punch In")
             {
                 StoreData(currentDate, currentTime, null, Pno);
             }
             else
             {
                 StoreData(currentDate, null, currentTime, Pno);
             }

             return Json(new { success = true, message = "Attendance recorded successfully." });
         }
         else
         {
             return Json(new { success = false, message = "Face does not match!" });
         }
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }
this is for StoreData when face is matched , now i want that if face matched then the Capture Image is store as a txt file in path in binary format and store info in Table also. Txt file name as ID_Pno.txt

 public void StoreData(string ddMMyy, string tmIn, string tmOut, string Pno)
 {
     using (var connection = new SqlConnection(configuration.GetConnectionString("RFID")))
     {
         connection.Open();

         if (!string.IsNullOrEmpty(tmIn))
         {
           
         }

         if (!string.IsNullOrEmpty(tmOut))
         {

         }

if (!string.IsNullOrEmpty(capturedImage))
{

    Guid ID = new Guid();

    byte[] imageBytes = Convert.FromBase64String(capturedImage.Split(',')[1]);
    var FileName = $"{ID}_{Pno}.txt";

    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CapturedImage");

    string filePath = Path.Combine(folderPath, FileName);

    if (!Directory.Exists(folderPath))
    {
        Directory.CreateDirectory(folderPath);
    }

    System.IO.File.WriteAllBytes(filePath, imageBytes);

    var query = @"
INSERT INTO App_ImageDetail(ID,Pno,FileName) 
VALUES 
(@ID,
@Pno,  
@FileName)";

    var parameters = new
    {
        ID = ID,
        Pno = Pno,
        FileName = FileName,
    };

    connection.Execute(query, parameters);

}
 }

