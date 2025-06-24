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
