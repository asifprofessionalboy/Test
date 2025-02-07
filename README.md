public async Task<IActionResult> EditDocument(Guid? id, int page = 1, string FinYear = "", string SearchMonth = "")
{
    if (HttpContext.Session.GetString("Session") == null)
    {
        return RedirectToAction("Login", "User");
    }

    var Dept = GetDepartmentDD();
    ViewBag.Department = Dept;

    var Month = GetMonthDD();
    ViewBag.Month = Month;

    int pageSize = 5;
    var query = context.AppTechnicalServices.OrderByDescending(x => x.RefNo).AsQueryable();

    if (!string.IsNullOrEmpty(FinYear) && FinYear != "Select Fin Year")
    {
        query = query.Where(a => a.FinYear.Contains(FinYear));
    }

    if (!string.IsNullOrEmpty(SearchMonth))
    {
        query = query.Where(a => a.Month.Contains(SearchMonth));
    }

    var pagedData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    var totalCount = query.Count();

    ViewBag.ListData2 = pagedData;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    ViewBag.FinYear = FinYear;
    ViewBag.SearchMonth = SearchMonth;

    if (id.HasValue)
    {
        var model = await context.AppTechnicalServices.FindAsync(id.Value);
        if (model == null)
        {
            return NotFound();
        }

        var viewModel = new AppTechnicalService
        {
            RefNo = model.RefNo,
            Department = model.Department,
            Subject = model.Subject,
            FinYear = model.FinYear,
            CreatedBy = model.CreatedBy,
            Attachment = model.Attachment,
            Month = model.Month,
            Id = model.Id
        };

        return View(viewModel);
    }

    return View(new AppTechnicalService());
}



not working ,
this is my view after the delete i am redirecting 
public async Task<IActionResult> EditDocument(Guid? id, int page = 1,string FinYear="",string SearchMonth="")
{
	if (HttpContext.Session.GetString("Session") != null)
	{
		var viewModel2 = new AppTechnicalService
		{

			Attach = new List<IFormFile>(),

		};

		int pageSize = 5;
		var query = context.AppTechnicalServices.OrderByDescending(x => x.RefNo).AsQueryable();

        var Dept = GetDepartmentDD();
        ViewBag.Department = Dept;

        var Month = GetMonthDD();
        ViewBag.Month = Month;

        if (!string.IsNullOrEmpty(FinYear) && FinYear != "Select Fin Year")
		{

			if (FinYear == "23-24")
			{
				query = query.Where(a => a.FinYear.Contains(FinYear));
			}
			else if (FinYear == "24-25")
			{
				query = query.Where(a => a.FinYear.Contains(FinYear));
			}
			else if (FinYear == "25-26")
			{
				query = query.Where(a => a.FinYear.Contains(FinYear));
			}



		}

		if (!string.IsNullOrEmpty(SearchMonth))
		{
			query = query.Where(a => a.Month.Contains(SearchMonth));
		}


		var pagedData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
		var totalCount = query.Count();

		ViewBag.ListData2 = pagedData;
		ViewBag.CurrentPage = page;
		ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
		ViewBag.FinYear = FinYear;
		ViewBag.SearchMonth = SearchMonth;
		if (id.HasValue)
		{
			var model = await context.AppTechnicalServices.FindAsync(id.Value);
			if (model == null)
			{
				return NotFound();
			}
			//var Dept = GetDepartmentDD();
			//ViewBag.Department = Dept;

			var viewModel = new AppTechnicalService
			{
				RefNo = model.RefNo,
				Department = model.Department,
				Subject = model.Subject,
				FinYear = model.FinYear,
				CreatedBy = model.CreatedBy,
				Attachment = model.Attachment,
				Month = model.Month,
				Id = model.Id
			};


			return View(viewModel);
		}

		return View(new AppTechnicalService());
	}
	else
	{
		return RedirectToAction("Login", "User");
	}
}

but when redirecting it shows me object reference not set to an object on these 
var DeptDropdown = ViewBag.Department as List<DepartmentDD>;
var MonthDropdown = ViewBag.Month as List<MonthDD>;

this is my delete method 
[HttpPost]
public IActionResult DeleteDocument(Guid id)
{
	var Data = context.AppTechnicalServices.FirstOrDefault(c => c.Id == id);
	if (Data != null)
	{
		context.AppTechnicalServices.Remove(Data);
		context.SaveChanges();
		TempData["Message"] = "Customer deleted successfully!";
	}
	
	return RedirectToAction("EditDocument",new {id=(Guid?)null});
}

this is my view side
<form asp-action="EditDocument" method="get">


	<div class="row">
		<div class="col-sm-1">
			<label class="control-label">Fin Year </label>
		</div>
		<div class="col-sm-2">
			<select class="form-control form-control-sm custom-select" name="FinYear">
				<option value="">Select Fin Year</option>
				@if (ViewBag.FinYear == "25-26")
				{
					<option value="25-26" selected>FY'26</option>
				}
				else
				{
					<option value="25-26">FY'26</option>
				}
				@if (ViewBag.FinYear == "24-25")
				{
					<option value="24-25" selected>FY'25</option>
				}
				else
				{
					<option value="24-25">FY'25</option>
				}
				@if (ViewBag.FinYear == "23-24")
				{
					<option value="23-24" selected>FY'24</option>
				}
				else
				{
					<option value="23-24">FY'24</option>
				}
			</select>
		</div>
		<div class="col-sm-1">
			<label class="control-label">Fin Month </label>
		</div>
		<div class="col-sm-2">
    <select class="form-control form-control-sm custom-select" name="SearchMonth">
        <option value="">Select Month</option>
        @foreach (var item in ViewBag.Month as List<MonthDD>)
        {
            <option value="@item.Month" selected="@(item.Month == ViewBag.SearchMonth ? "selected" : null)">
                @item.Month
            </option>
        }
    </select>
</div>
<div class="col-sm-2">
    <button type="submit" class="btn btn-primary">Search</button>
</div>

	</div>

</form>
	<legend style="width:auto;border:0;font-size:14px;margin:0px 6px 0px 6px;padding:0px 5px 0px 5px;color:#0000FF"><b></b></legend>
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
							<a asp-action="EditDocument"
							   asp-route-id="@item.Id"
						   asp-route-FinYear="@ViewBag.FinYear"
						   asp-route-SearchMonth="@ViewBag.SearchMonth"
							   asp-route-page="@ViewBag.CurrentPage"
							   class="btn glow"
							   style="text-decoration:none;background-color:;font-weight:;">
								@item.RefNo
							</a>
						</td>

						<td class="">@item.FinYear</td>
						<td>@item.Department</td>
						<td>@item.Subject</td>
					<td>
						@if (!string.IsNullOrEmpty(item.Attachment))
						{
							var parts = item.Attachment.Split('_');
							<!-- Extract file name -->
							var fileName = parts[parts.Length - 1];

							<a href="@Url.Action("DownloadFile", "Technical", new { fileName = item.Attachment })"
							   target="_blank" class="fas fa-download" style="font-size:14px;">
								<span style="font-family:arial;font-weight:500;">@fileName</span>
							</a>
						}
					</td>
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
@if (Model != null && Model.RefNo != null)
{
	<form asp-action="EditDocument" id="form2" method="post" enctype="multipart/form-data">

		<div class="card-header text-center" style="background-color:#49477a;color:white;font-weight:bold;">Edit Document</div>
		<div class="col-md-12" style="padding:10px;">


			<fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px;">
				<div class="form-group row">
					<div class="col-sm-1 d-flex align-items-center">
						<label asp-for="FinYear" class="control-label">Fin Year </label>
					</div>

					<div class="col-sm-3">
						<select asp-for="FinYear" class="form-control form-control-sm custom-select" name="FinYear">
							<option value="">Select Fin Year</option>
							<option value="23-24">FY'24</option>
							<option value="24-25">FY'25</option>
							<option value="25-26">FY'26</option>

						</select>
					</div>
					<div class="col-sm-1 d-flex align-items-center">
						<label asp-for="Month" class="control-label">Months </label>
					</div>
					<div class="col-sm-3">
						<select asp-for="Month" class="form-control form-control-sm custom-select">
							<option value="">Select Month</option>
							@foreach (var item in MonthDropdown)
							{
								<option value="@item.Month">@item.Month</option>
							}
						</select>
					</div>

					<input type="hidden" value="@Model.RefNo" name="RefNo"/>
					<input type="hidden" value="@Model.Attachment" name="Attachment"/>
					<input type="hidden" value="@Model.Id" name="Id" id="EditId"/>
					
					<div class="col-sm-1 d-flex align-items-center">
						<label asp-for="Department" class="control-label">Department </label>
					</div>
					<div class="col-sm-3">
						<select asp-for="Department" class="form-control form-control-sm custom-select">
							<option value="">Select Department</option>
							<option value="Admin & CC">Admin & CC</option>
							<option value="Bidding">Bidding</option>
							<option value="BE">BE</option>
							<option value="Data Analytics">Data Analytics</option>
							<option value="BD">BD</option>
							<option value="CRM">CRM</option>
							<option value="DETP">DETP</option>
							<option value="Technical Services">Technical Services</option>
						</select>
					</div>
				</div>

				<div class="form-group row">
					<div class="col-sm-1">
						<label asp-for="Subject" class="control-label">Subject </label>
					</div>
					<div class="col-sm-3">
						<select asp-for="Subject" class="form-control form-control-sm custom-select" name="Subject">
							<option value="">Select Subject</option>
							<option value="Flash Report">Flash Report</option>
							<option value="MD Communication pack">MD Communication pack</option>
							<option value="L2 KPIs - Technical Services">L2 KPIs - Technical Services</option>
							<option value="L3 KPIs - Bidding">L3 KPIs - Bidding</option>
							<option value="L3 KPIs - DETP">L3 KPIs - DETP</option>
							<option value="L3 KPIs - BE,Data Analytics,BD,CRM">L3 KPIs - BE,Data Analytics,BD,CRM</option>
							<option value="L3 KPIs - Admin & CC">L3 KPIs - Admin & CC</option>
						</select>
					</div>
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

				<div class="col-sm-1 align-items-center">
					<label asp-for="Attach" class="control-label">Attachment </label>
				</div>
				<div class="col-sm-3">
					<input asp-for="Attach" type="file" class="form-control form-control-sm" multiple id="fileInput" />
					<span asp-validation-for="Attach" class="text-danger"></span>

				</div>
				<div class="col-sm-10 file" style="">
					<div id="fileNames" class="mt-2" style="font-size:13px;font-family:arial;font-weight:500;color:#3a7cda;"></div>
				</div>
			</div>

			


	
			<div class="form-group row d-flex justify-content-center mt-3">

			<div class="text-center">
			<div class="text-center">
			<input type="submit" value="Save" id="Savebtn" class="btn" style="border-radius:7px"/>
			<input type="submit" value="Delete" id="DeleteButton" class="btn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;" />
			</div>

			</div>
			</div>

		</fieldset>
	</div>

</form>
}
