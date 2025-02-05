public IActionResult DownloadFile(string fileName)
{
	var uploadPath = configuration["FileUpload:Path"];
	var filePath = Path.Combine(uploadPath, fileName);

	if (!System.IO.File.Exists(filePath))
	{
		return NotFound();
	}

	var memory = new MemoryStream();
	using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
	{
		stream.CopyTo(memory);
	}
	memory.Position = 0;

	var contentType = GetContentType(filePath);
	var fileExtension = Path.GetExtension(fileName).ToLower();

	if (fileExtension == ".pdf")
	{
		// Open PDF in browser instead of downloading
		return File(memory, contentType, fileName, enableRangeProcessing: true);
	}
	else
	{
		// Force download for other file types
		return File(memory, contentType, fileName);
	}
}
