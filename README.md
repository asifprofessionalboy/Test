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
