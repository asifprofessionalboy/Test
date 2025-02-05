this is my attachment that has a anchor when i clicks on anchor it download the pdf i want only it opens the pdf in new tab
<a asp-action="ViewerForm"
<div class="col-sm-1 align-items-center">
		<label asp-for="Attach" class="control-label">Attachment </label>
	</div>
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
								   target="_blank">
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


this is the link when i click on attachment 
https://localhost:7073/Technical/DownloadFile?fileName=16d60a72-ce03-40b7-84bb-0d4481285824_05-02-2025_09-31-37_InnovationRPT%20(7).pdf


i want to open the pdf on new tab like new page not to download 
