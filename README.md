[HttpPost]
public IActionResult GetImagesByPno([FromForm] string Pno)
{
    try
    {
        using (var connection = new SqlConnection(configuration.GetConnectionString("RFID")))
        {
            connection.Open();
            var query = "SELECT FileName FROM App_ImageDetail WHERE Pno = @Pno";
            var fileNames = connection.Query<string>(query, new { Pno }).ToList();

            var images = new List<object>();

            foreach (var fileName in fileNames)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CapturedImage", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                    string base64String = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                    
                    images.Add(new { FileName = fileName, Base64Image = base64String });
                }
            }

            return Json(new { success = true, images });
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

<form id="fetchImagesForm">
    <label>Enter Pno:</label>
    <input type="text" id="Pno" name="Pno" required>
    <button type="submit">Fetch Images</button>
</form>

<div id="imageContainer"></div>
$(document).ready(function () {
    $("#fetchImagesForm").submit(function (event) {
        event.preventDefault(); // Prevent default form submission

        let formData = new FormData(this); // Get form data

        $.ajax({
            url: "/YourController/GetImagesByPno", // Replace 'YourController' with your actual controller name
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    let imageContainer = $("#imageContainer");
                    imageContainer.empty(); // Clear previous images
                    
                    response.images.forEach(image => {
                        let imgElement = `<img src="${image.Base64Image}" alt="Captured Image" style="width:100px;height:100px;margin:5px;">`;
                        imageContainer.append(imgElement);
                    });
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Error fetching images.");
            }
        });
    });
});


this is my controller method PunchIn or PunchOut , this store person Image in table as well as in wwwroot folder as binary 

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

       
            string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
            string currentTime = DateTime.Now.ToString("HH:mm");

            if (model.Type == "Punch In")
            {
                StoreData(currentDate, currentTime, null, Pno, model.ImageData);
            }
            else
            {
                StoreData(currentDate, null, currentTime, Pno, model.ImageData);
            }

            return Json(new { success = true, message = "Attendance recorded successfully." });
        
       
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

 
public void StoreData(string ddMMyy, string tmIn, string tmOut, string Pno,string capturedImage)
{
    using (var connection = new SqlConnection(configuration.GetConnectionString("RFID")))
    {
        connection.Open();

                

        if (!string.IsNullOrEmpty(capturedImage))
        {
            Guid ID = Guid.NewGuid(); 

            
            byte[] imageBytes = Convert.FromBase64String(capturedImage.Split(',')[1]);

            string fileName = $"{ID}_{Pno}.txt";

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CapturedImage");

            string filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            System.IO.File.WriteAllBytes(filePath, imageBytes); 

           
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

this is my table data which stores user info of Image 

ID					Pno	DateTime			FileName
E133D669-9CE4-4BBE-BEA0-456A3EA0444F	159445	2025-03-27 13:40:00.300	 e133d669-9ce4-4bbe-bea0-456a3ea0444f_159445.txt
and this is my txt file which store image in binary 

e133d669-9ce4-4bbe-bea0-456a3ea0444f_159445.txt


i want to fetch image for every different Pno . each day has multiple image and txt file . i want to show all
