this is my full code of attachment 
	<div class="col-sm-3">
			@if (!string.IsNullOrEmpty(Model.Attachment))
			{
				<div>
					<ul>
						@foreach (var fileName in Model.Attachment.Split(','))
						{
							var cleanFileName = ExtractFileName(fileName);
							var fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
							var isPdf = fileExtension == ".pdf";

							<li>
								<a href="@Url.Action("DownloadFile", new { fileName = fileName })"
								@(isPdf ? "target=\"_blank\"" : "")>
									@cleanFileName
								</a>
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


</div>

	public IActionResult DownloadFile(string fileName)
	{
		var uploadPath = configuration["FileUpload:Path"];
		var filePath = Path.Combine(uploadPath, fileName);

		if (!System.IO.File.Exists(filePath))
		{
			return NotFound();
		}

		var memory = new MemoryStream();
		using (var stream = new FileStream(filePath, FileMode.Open))
		{
			stream.CopyTo(memory);
		}
		memory.Position = 0;

		return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
	}

	
in this i want that if user clicks on the attachment link it opens the attachment pdf in new tab in browser and download automatically 

simply i want to show the pdf in new tab of browser
