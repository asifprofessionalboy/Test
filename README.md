 public IActionResult FetchImage()
 {
     return View();
 }
 [HttpPost]
 public IActionResult ConvertBinaryToImage(string pno)
 {
     string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CapturedImage");
     using (var connection = new SqlConnection(configuration.GetConnectionString("GEOFENCEDB")))
     {
         connection.Open();
         // Fetch FileName from the database based on Pno
         var query = "SELECT FileName FROM App_ImageDetail WHERE Pno = @Pno";
         string fileName = connection.QueryFirstOrDefault<string>(query, new { Pno = pno });

         if (string.IsNullOrEmpty(fileName))
         {
             return NotFound("No image found for this Pno.");
         }

         string txtFilePath = Path.Combine(folderPath, fileName);
         string imageFilePath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(fileName) + ".jpg");

         if (System.IO.File.Exists(txtFilePath))
         {
             byte[] imageBytes = System.IO.File.ReadAllBytes(txtFilePath);
             System.IO.File.WriteAllBytes(imageFilePath, imageBytes); // Save as JPG

             return Json(new { imagePath = $"/CapturedImage/{Path.GetFileName(imageFilePath)}" });
         }

         return NotFound("Binary file not found.");
     }
 }

<h2>Enter Pno to Fetch Image</h2>

<input type="text" id="pnoInput" placeholder="Enter Pno">
<button onclick="fetchImage()">Fetch Image</button>

<br>
<br>

<img id="capturedImage" src="" alt="Captured Image" style="width: 200px; height: 200px; display: none; border: 1px solid black;">

<script>
    function fetchImage() {
        let pno = document.getElementById("pnoInput").value.trim();

        if (pno === "") {
            alert("Please enter a valid Pno.");
            return;
        }

        fetch(`/Geo/ConvertBinaryToImage`)
            .then(response => response.json())
            .then(data => {
                if (data.imagePath) {
                    document.getElementById("capturedImage").src = data.imagePath;
                    document.getElementById("capturedImage").style.display = "block";
                } else {
                    alert("Image not found.");
                    document.getElementById("capturedImage").style.display = "none";
                }
            })
            .catch(error => console.error("Error fetching image:", error));
    }
</script>

getting this error 
FetchImage:94 
 GET https://localhost:7153/Geo/ConvertBinaryToImage 405 (Method Not Allowed)

FetchImage:105 Error fetching image: SyntaxError: Failed to execute 'json' on 'Response': Unexpected end of JSON input
    at FetchImage:95:40
