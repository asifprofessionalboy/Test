public IActionResult ConvertBinaryToImage(string pno)
{
    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CapturedImage");
    
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

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Fetch Captured Image</title>
</head>
<body>

    <h2>Enter Pno to Fetch Image</h2>
    
    <input type="text" id="pnoInput" placeholder="Enter Pno">
    <button onclick="fetchImage()">Fetch Image</button>

    <br><br>

    <img id="capturedImage" src="" alt="Captured Image" style="width: 200px; height: 200px; display: none; border: 1px solid black;">

    <script>
        function fetchImage() {
            let pno = document.getElementById("pnoInput").value.trim();
            
            if (pno === "") {
                alert("Please enter a valid Pno.");
                return;
            }

            fetch(`/YourController/ConvertBinaryToImage?pno=${pno}`)
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

</body>
</html>




this is my logic for storing my captured Image in binary format as txt file now i want to convert binary to jpg file to display image against the pno

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
