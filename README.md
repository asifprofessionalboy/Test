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
                    // Read binary image data from .txt file
                    byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                    
                    // Convert to Base64 string
                    string base64String = Convert.ToBase64String(imageBytes);

                    // Create a Base64 image URL
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

$(document).ready(function () {
    $("#fetchImagesForm").submit(function (event) {
        event.preventDefault(); // Stop default form submission

        let formData = new FormData(this);

        $.ajax({
            url: "/Geo/GetImagesByPno", // Make sure this is the correct path
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    let imageContainer = $("#imageContainer");
                    imageContainer.empty(); // Clear previous images

                    response.images.forEach(image => {
                        let imgElement = `<img src="${image.ImageUrl}" alt="Captured Image" style="width:100px;height:100px;margin:5px;">`;
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



this is the output i am getting 
{
  "success": true,
  "images": [
    {
      "fileName": "adce347d-47ff-4c22-aad0-24ecedf3d89a_151514.txt",
      "base64Image": "data:image/png;base64,"
    },
    {
      "fileName": "93988e7b-a085-48ca-a1e5-a36a2bf6f338_151514.txt",
      "base64Image": "data:image/png;base64,"
    },
    {
      "fileName": "7cfe3ee5-2396-4d63-95eb-e12ca63a6568_151514.txt",
      "base64Image": "data:image/png;base64,"
    }
  ]
}

this is my methods


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

and this is my html and js

<form id="fetchImagesForm" asp-action="GetImagesByPno">
    <label>Enter Pno:</label>
    <input type="text" id="Pno" name="Pno" required>
    <button type="submit">Fetch Images</button>
</form>

<div id="imageContainer"></div>


<script>
    $(document).ready(function () {
        $("#fetchImagesForm").submit(function (event) {
            event.preventDefault();

            let formData = new FormData(this);

            $.ajax({
                url: "/Geo/GetImages",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {
                        let imageContainer = $("#imageContainer");
                        imageContainer.empty(); 

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
</script>

i want my output shows images not json data
