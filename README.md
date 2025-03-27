success: function (response) {
    console.log("AJAX Response:", response); // Debugging log

    if (response.success) {
        let imageContainer = $("#imageContainer");
        imageContainer.empty(); // Clear previous images

        response.images.forEach(image => {
            console.log("Image URL:", image.ImageUrl); // Debugging log

            let imgElement = `<img src="${image.ImageUrl}" 
                alt="Captured Image" 
                style="width:150px;height:150px;margin:5px;border:1px solid red;">`;
                
            imageContainer.append(imgElement);
        });
    } else {
        alert(response.message);
    }
}

let imgElement = `<img src="${image.ImageUrl}?t=${new Date().getTime()}" 
    alt="Captured Image" 
    style="width:150px;height:150px;margin:5px;border:1px solid red;">`;


app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=3600");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*"); // Allow all origins
    }
});

success: function (response) {
    console.log("AJAX Response:", response); // Debugging log

    if (response.success) {
        let imageContainer = $("#imageContainer");
        imageContainer.empty(); // Clear previous images

        response.images.forEach(image => {
            console.log("Image Object:", image); // Debugging log
            let imgElement = `<img src="${image.ImageUrl}" alt="Captured Image" style="width:150px;height:150px;margin:5px;">`;
            imageContainer.append(imgElement);
        });
    } else {
        alert(response.message);
    }
}

 
 [HttpPost]
public IActionResult GetImagesByPno([FromForm] string Pno)
{
    try
    {
        Console.WriteLine("Received Pno: " + Pno); // Debug log

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
                    string base64String = Convert.ToBase64String(imageBytes);
                    string imageUrl = $"data:image/jpeg;base64,{base64String}";

                    Console.WriteLine("Generated Image URL: " + imageUrl); // Debug log

                    images.Add(new { FileName = fileName, ImageUrl = imageUrl });
                }
                else
                {
                    Console.WriteLine("File not found: " + filePath); // Debug log
                }
            }

            if (images.Count == 0)
            {
                return Json(new { success = false, message = "No images found for this Pno" });
            }

            return Json(new { success = true, images });
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message); // Debug log
        return Json(new { success = false, message = ex.Message });
    }
}

 
 
public IActionResult GetImages()
 {
     return View();
 }

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
                     string base64String = Convert.ToBase64String(imageBytes);
                     string imageUrl = $"data:image/jpeg;base64,{base64String}";

                     images.Add(new { FileName = fileName, ImageUrl = imageUrl });
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


<form id="fetchImagesForm" asp-action="GetImagesByPno">
    <label>Enter Pno:</label>
    <input type="text" id="Pno" name="Pno" required>
    <button type="submit">Fetch Images</button>
</form>

<div id="imageContainer"></div>

<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script>
    $(document).ready(function () {
        $("#fetchImagesForm").submit(function (event) {
            event.preventDefault(); // Stop default form submission

            let formData = new FormData(this);

            $.ajax({
                url: "/Geo/GetImagesByPno", // Ensure this is the correct endpoint
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {
                        let imageContainer = $("#imageContainer");
                        imageContainer.empty(); // Clear previous images

                        response.images.forEach(image => {
                            let imgElement = `<img src="${image.ImageUrl}" alt="Captured Image" style="width:150px;height:150px;margin:5px;">`;
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
</script>


why i am getting this undefined:1 
            
           GET https://localhost:7153/Geo/undefined 404 (Not Found) and image is not showing
