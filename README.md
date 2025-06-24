[HttpGet("DownloadHandler")]
public IActionResult DownloadHandler(string file, string folder)
{
    // Check session
    var user = HttpContext.Session.GetString("UserName");
    if (string.IsNullOrEmpty(user))
    {
        return Content("Session expired or user not logged in.");
    }

    // Validate input
    if (string.IsNullOrEmpty(file))
        return Content("File name not specified.");

    var baseUploadPath = configuration["FileUpload:Path"];

    // Sanitize folder input to prevent path traversal attacks
    folder = string.IsNullOrEmpty(folder) ? "" : folder.Replace("..", "").Replace("/", "").Replace("\\", "");

    var fullPath = string.IsNullOrEmpty(folder)
        ? Path.Combine(baseUploadPath, file)
        : Path.Combine(baseUploadPath, folder, file);

    if (!System.IO.File.Exists(fullPath))
    {
        return Content("File not found.");
    }

    var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
    var contentType = GetContentType(fullPath);
    return File(stream, contentType, Path.GetFileName(fullPath));
}
<a href="/DownloadHandler?file=@fileName&folder=@subFolder" target="_blank">@cleanFileName</a>



[HttpGet("DownloadHandler")]
public IActionResult DownloadHandler([FromQuery] string file)
{
    // Check if session is active
    var user = HttpContext.Session.GetString("UserName");
    if (string.IsNullOrEmpty(user))
    {
        return Content("Session expired or user not logged in.");
    }

    if (string.IsNullOrEmpty(file))
    {
        return Content("File name not specified.");
    }

    var uploadPath = configuration["FileUpload:Path"];
    var filePath = Path.Combine(uploadPath, file);

    if (!System.IO.File.Exists(filePath))
    {
        return Content("File not found.");
    }

    // Send file to browser
    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    var contentType = GetContentType(filePath);
    return File(stream, contentType, Path.GetFileName(filePath));
}

<a href="/DownloadHandler?file=@fileName" target="_blank">@cleanFileName</a>



<div class="col-sm-3">
    @if (!string.IsNullOrWhiteSpace(Model.Attachment))
    {
        var fileList = Model.Attachment.Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (fileList.Any())
        {
            <div>
                <ul class="list-unstyled">
                    @foreach (var fileName in fileList)
                    {
                        var cleanFileName = ExtractOriginalFileName(fileName);

                        <li>
                            <a href="@Url.Action("DownloadFile", "YourControllerName", new { fileName = fileName })" target="_blank">
                                @cleanFileName
                            </a>
                        </li>
                    }
                </ul>
            </div>
        }
    }
</div>

@functions {
    private string ExtractOriginalFileName(string fileNameWithPrefix)
    {
        // Example: "abc123_24-06-2025_12-00-00_myfile.pdf" → "myfile.pdf"
        var lastUnderscoreIndex = fileNameWithPrefix.LastIndexOf('_');
        if (lastUnderscoreIndex > 0)
        {
            return fileNameWithPrefix.Substring(lastUnderscoreIndex + 1);
        }
        return fileNameWithPrefix;
    }
}

"FileUpload": {
  "Path": "D:\\Cybersoft_Doc\\Innovation\\Attachments"
}
if (InnViewModel.Attach != null && InnViewModel.Attach.Any())
{
	var uploadPath = configuration["FileUpload:Path"];

	// Ensure the directory exists
	if (!Directory.Exists(uploadPath))
	{
		Directory.CreateDirectory(uploadPath);
	}

	foreach (var file in InnViewModel.Attach)
	{
		if (file.Length > 0)
		{
			var uniqueId = Guid.NewGuid().ToString();
			var currentDateTime = DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss");
			var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
			var fileExtension = Path.GetExtension(file.FileName);
			var formattedFileName = $"{uniqueId}_{currentDateTime}_{originalFileName}{fileExtension}";
			var fullPath = Path.Combine(uploadPath, formattedFileName);

			using (var stream = new FileStream(fullPath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			InnViewModel.Attachment += $"{formattedFileName},";
		}
	}

	if (!string.IsNullOrEmpty(InnViewModel.Attachment))
	{
		InnViewModel.Attachment = InnViewModel.Attachment.TrimEnd(',');
	}
}

public IActionResult DownloadFile(string fileName)
{
	if (string.IsNullOrEmpty(fileName))
		return BadRequest("File name is missing.");

	var uploadPath = configuration["FileUpload:Path"];
	var filePath = Path.Combine(uploadPath, fileName);

	if (!System.IO.File.Exists(filePath))
	{
		return NotFound("File not found.");
	}

	var memory = new MemoryStream();
	using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
	{
		stream.CopyTo(memory);
	}
	memory.Position = 0;

	return File(memory, GetContentType(filePath), fileName);
}
[HttpGet("DownloadViaHandler")]
public IActionResult DownloadViaHandler([FromQuery] string file)
{
	if (string.IsNullOrEmpty(file)) return Content("No file specified.");

	if (HttpContext.Session.GetString("UserName") == null)
		return Content("Session expired or user not logged in.");

	var uploadPath = configuration["FileUpload:Path"];
	var filePath = Path.Combine(uploadPath, file);

	if (!System.IO.File.Exists(filePath))
		return Content("File not found.");

	var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
	return File(stream, "application/octet-stream", file);
}
<a href="/DownloadViaHandler?file=@fileName">





i have this frontend side 
			
	<div class="col-sm-3">
					@if (Model.Attachment != null)
					{
						<div>
							<ul>
								@foreach (var fileName in Model.Attachment.Split(','))
								{

									var cleanFileName = ExtractFileName(fileName);

									<li>
										<a href="@Url.Action("DownloadFile", new { fileName = fileName })">@cleanFileName</a>
									</li>
								}
							</ul>
						</div>
					}
				</div>
				@functions {
				private string ExtractFileName(string fileNameWithPrefix)
				{

					var parts = fileNameWithPrefix.Split('_');
					if (parts.Length > 1)
					{
						return parts[parts.Length - 1];
					}
					return fileNameWithPrefix;
				}
}
