<form asp-action="ViewerForm" method="get">
	<input type="hidden" name="L2" value="@ViewBag.L2" />
	<input type="hidden" name="Bidding" value="@ViewBag.Bidding" />
	<input type="hidden" name="Flash" value="@ViewBag.Flash" />
	<input type="hidden" name="DETP" value="@ViewBag.DETP" />
	<input type="hidden" name="BE" value="@ViewBag.BE" />
	<input type="hidden" name="Admin" value="@ViewBag.Admin" />
	<input type="hidden" name="MD" value="@ViewBag.MD" />

	<div class="row">
		<div class="col-sm-1">
			<label class="control-label">Fin Year </label>
		</div>
		<div class="col-sm-2">
			<select class="form-control form-control-sm custom-select" name="FinYear">
				<option value="">Select Fin Year</option>
				<option value="25-26" selected="@(ViewBag.FinYear == "25-26" ? "selected" : null)">FY'26</option>
				<option value="24-25" selected="@(ViewBag.FinYear == "24-25" ? "selected" : null)">FY'25</option>
				<option value="23-24" selected="@(ViewBag.FinYear == "23-24" ? "selected" : null)">FY'24</option>
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

ViewBag.FinYear = FinYear;
ViewBag.SearchMonth = SearchMonth;
ViewBag.MD = MD;
ViewBag.L2 = L2;
ViewBag.Flash = Flash;
ViewBag.Bidding = Bidding;
ViewBag.DETP = DETP;
ViewBag.BE = BE;
ViewBag.Admin = Admin;




this is my form , when i click on cards it redirects user to new form along with the filter and shows grid 
<form asp-action="ViewerForm" method="get">
<div class="container mt-4" style="">
    <div class="row ">
        <div class="col-xl-3 col-lg-6">
                <a asp-action="ViewerForm" asp-route-L2="L2 KPIs - Technical Services">
            <div class="card l-bg-cherry">
                <div class="card-statistic-3 p-4">
                   
                    <div class="mb-4">
                        <h6 class="card-title mb-0" name="L2">
                                    L2 KPIs
                            </h6>
                    </div>
                    <div class="row align-items-center mb-2 d-flex">
                        <div class="col-8">
                                    <h7 class="d-flex align-items-center mb-0 head">
                                Technical Services
                            </h7>
                        </div>
                       
                    </div>
                </div>
            </div>
                </a>
            </div>
        <div class="col-xl-3 col-lg-6">
                <a asp-action="ViewerForm" asp-route-Bidding="L3 KPIs - Bidding">
            <div class="card l-bg-blue-dark">
                <div class="card-statistic-3 p-4">
                   
                    <div class="mb-4">
                                <h6 class="card-title mb-0 head">
                               
                                    L3 KPIs
                               
                            </h6>
                    </div>
                    <div class="row align-items-center mb-2 d-flex">
                        <div class="col-8">
                                    <h7 class="d-flex align-items-center mb-0 head">
                                Bidding
                            </h7>
                        </div>
                    </div>
                </div>
            </div>
                </a>
        </div>
</form>

this is my form where grid shows of filter of the above cards

<form asp-action="ViewerForm" method="get">

	
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
					<td>
						@if (!string.IsNullOrEmpty(item.Attachment))
						{
							var parts = item.Attachment.Split('_');
							<!-- Extract file name -->
							var fileName = parts[parts.Length-1];

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

this is my controller 
public async Task<IActionResult> ViewerForm(Guid? id, int page = 1, string Bidding = "", string L2 = "", string Flash = "", string SearchMonth = "", string FinYear = "",string DETP="",string MD="",string BE="",string Admin = "")
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

        if (!string.IsNullOrEmpty(Bidding))
		{
			query = query.Where(a => a.Subject.Contains(Bidding));
		}
		if (!string.IsNullOrEmpty(DETP))
		{
			query = query.Where(a => a.Subject.Contains(DETP));
		}
		if (!string.IsNullOrEmpty(BE))
		{
			query = query.Where(a => a.Subject.Contains(BE));
		}
		if (!string.IsNullOrEmpty(Admin))
		{
			query = query.Where(a => a.Subject.Contains(Admin));
		}
		if (!string.IsNullOrEmpty(MD))
		{
			query = query.Where(a => a.Subject.Contains(MD));
		}
		if (!string.IsNullOrEmpty(L2))
		{
			query = query.Where(a => a.Subject.Contains(L2));
		}
		if (!string.IsNullOrEmpty(Flash))
		{
			query = query.Where(a => a.Subject.Contains(Flash));
		}

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
		ViewBag.MD = MD;
		ViewBag.L2 = L2;
		ViewBag.Flash = Flash;
		ViewBag.Bidding = Bidding;
		ViewBag.DETP = DETP;
		ViewBag.BE = BE;
		ViewBag.Admin = Admin;
		


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
				Month = model.Month
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


breakdown of this i am explaining that firstly when i click on card it redirects me viewerform and shows grid against the filter like L2 KPIs - Technical Services then i search on viwerform Finyear and month then the subject is clear and it shows all the subject not the L2 KPIs - Technical Services
