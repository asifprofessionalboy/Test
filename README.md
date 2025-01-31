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



<td>
    @if (!string.IsNullOrEmpty(item.Attachment))
    {
        var fileName = item.Attachment.Split('_').Last(); <!-- Extract file name -->
        var fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

        <a href="@Url.Action("DownloadFile", "YourControllerName", new { fileName = item.Attachment })"
           target="_blank">
            @fileName
        </a>
    }
</td>


i have this table grid in my view 
<table class="table" id="myTable">
	<thead class="table" style="background-color: #d2b1ff;color: #000000;font-size:15px;">
		<tr>
			<th style="width:12%;">Ref No</th>
			<th style="width:10%;">Fin Year</th>
			<th style="width:25%;">Department</th>
			<th>Subject</th>
			<th>Attachment</th>
		</tr>
	</thead>
	<tbody>
		@if (ViewBag.ListData2 != null)
		{
			@foreach (var item in ViewBag.ListData2)
			{
				<tr>
					<td>
						<a asp-action="ViewerForm"
						   asp-route-id="@item.Id"
						   asp-route-page="@ViewBag.CurrentPage"
						   asp-route-FinYear="@ViewBag.FinYear"
						   asp-route-MD="@ViewBag.MD"
						   asp-route-L2="@ViewBag.L2"
						   asp-route-Flash="@ViewBag.Flash"
						   asp-route-Bidding="@ViewBag.Bidding"
						   asp-route-DETP="@ViewBag.DETP"
						   asp-route-BE="@ViewBag.BE"
						   asp-route-Admin="@ViewBag.Admin"
						   asp-route-SearchMonth="@ViewBag.SearchMonth"
						   class="btn glow"
						   style="text-decoration:none;background-color:;font-weight:;">
							@item.RefNo
						</a>
					</td>

					<td class="">@item.FinYear</td>
					<td>@item.Department</td>
					<td>@item.Subject</td>
					<td>@item.Attachment</td>
				</tr>
			}
		}
		else
		{
			<tr>
				<td colspan="4">No data available</td>
			</tr>
		}
	</tbody>
</table>


in this i have a column attachment , attachment is stored like this in table 
6f04eb4a-5db1-4462-a227-c929dfa10534_23-01-2025_10-18-45_InnovationRPT (7).pdf

i want to show only name InnovationRPT (7).pdf and link on this when i click on this it opens the pdf in new tab to show 
