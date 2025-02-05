this is my month for searching 
<form asp-action="EditDocument" method="get">
<div class="col-sm-2">
	<select class="form-control form-control-sm custom-select" name="SearchMonth">
		<option value="">Select Month</option>
		@foreach (var item in @ViewBag.Month as List<MonthDD>)
		{
			<option value="@item.Month">@item.Month</option>
		}
	</select>
</div>
<div class="col-sm-2">
	<button type="submit" class="btn btn-primary">Search</button>
</div>
</form>

this is my controller 
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

in this i am facing an issue that when i search month the value of the month is clearing from dropdown , i want that if user search June month it selects the option Month June from drodown
