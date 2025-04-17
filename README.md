this is my upload image logic 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(string Pno, string Name, string photoData)
        {
            if (!string.IsNullOrEmpty(photoData) && !string.IsNullOrEmpty(Pno) && !string.IsNullOrEmpty(Name))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(photoData.Split(',')[1]);

                   
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");

                    
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                   
                    string fileName = $"{Pno}-{Name}.jpg";
                    string filePath = Path.Combine(folderPath, fileName);

                  
                    System.IO.File.WriteAllBytes(filePath, imageBytes);

                   
                    var person = new AppPerson
                    {
                        Pno = Pno,
                        Name = Name,
                        Image = fileName
                    };

                    context.AppPeople.Add(person);
                    await context.SaveChangesAsync();

                   
                    return Ok(new { success = true, message = "Image uploaded and data saved successfully." });
                }
                catch (Exception ex)
                {
                  
                    return StatusCode(500, new { success = false, message = "Error saving image: " + ex.Message });
                }
            }

            return BadRequest(new { success = false, message = "Missing required fields!" });
        }

in this i want when it saving on table if the Pno exists then update the record otherwise add , only for table 
