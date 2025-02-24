using (IDbConnection connection = new SqlConnection(connectionString))
{
    string updateQuery = @"
        UPDATE App_Notification 
        SET IsViewed = 1 
        WHERE Subject = @Subject AND Pno = @Pno";

    string selectedSubject = MD ?? Bidding ?? Bi2nd ?? Bi4th ?? BDWeek ?? L2 ?? Flash ?? Exception ?? DETP ?? BE ?? Admin;

    if (!string.IsNullOrEmpty(selectedSubject))
    {
        await connection.ExecuteAsync(updateQuery, new { Subject = selectedSubject, Pno = sessionPno });
    }
}



public async Task<IActionResult> ViewerForm(Guid? id, int page = 1, string Bidding = "",string Bi2nd = "",  string Bi4th = "",  string BDWeek = "", string L2 = "", string Flash = "", string Exception = "", string SearchMonth = "", string FinYear = "",string DETP="",string MD="",string BE="",string Admin = "")
{
	if (HttpContext.Session.GetString("Session") != null)
	{
        string sessionPno =  HttpContext.Session.GetString("Session"); 
	
        string connectionString = GetConnectionString();
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            string updateQuery = @"
                UPDATE App_Notification 
                SET IsViewed = 1 
                WHERE Subject = @Subject AND Pno = @Pno";

            await connection.ExecuteAsync(updateQuery, new { Subject = MD,Bidding,Bi2nd,Bi4th,BDWeek,L2,Flash, Exception,DETP,BE,Admin, Pno = sessionPno });
        }


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

        var subjectDDs = GetSubjectDD();
        ViewBag.Subjects = subjectDDs;

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
        if (!string.IsNullOrEmpty(Exception))
        {
            query = query.Where(a => a.Subject.Contains(Exception));
        }
        if (!string.IsNullOrEmpty(Bi2nd))
        {
            query = query.Where(a => a.Subject.Contains(Bi2nd));
        }
        if (!string.IsNullOrEmpty(Bi4th))
        {
            query = query.Where(a => a.Subject.Contains(Bi4th));
        }
        if (!string.IsNullOrEmpty(BDWeek))
        {
            query = query.Where(a => a.Subject.Contains(BDWeek));
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
		ViewBag.Exception = Exception;
		ViewBag.Bi2nd = Bi2nd;
		ViewBag.Bi4th = Bi4th;
		ViewBag.BDWeek = BDWeek;
		


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
        <div class="col-sm-4">
            <a asp-action="ViewerForm" asp-route-MD="MD Communication pack" class="position-relative">
                <div class="card l-bg-cyan-dark position-relative">

                   
                    @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("MD Communication pack"))
                    {
                        <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                            @ViewBag.UnreadNotifications["MD Communication pack"]
                        </span>
                    }

                    <div class="card-statistic-3 p-4">
                        <div class="">
                            <h6 class="card-title mb-0 head">
                                MD Communication pack
                            </h6>
                        </div>
                    </div>
                    <div class="row align-items-center mb-4 d-flex">
                        <div class="col-8">
                            <h7 class="d-flex align-items-center mb-1">

                            </h7>
                        </div>

                    </div>
                </div>
            </a>
        </div>


        <div class="col-sm-4">

            <a asp-action="ViewerForm" asp-route-Flash="Flash Report" class="position-relative">
                <div class="card l-bg-purple-dark position-relative">
                    @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("Flash Report"))
                    {
                        <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                            @ViewBag.UnreadNotifications["Flash Report"]
                        </span>
                    }
                    <div class="card-statistic-3 p-4">

                        <div class="">
                            <h6 class="card-title mb-0 head">

                                Flash Report

                            </h6>
                        </div>
                        <div class="row align-items-center mb-4 d-flex">
                            <div class="col-8">
                                <h7 class="d-flex align-items-center mb-1">

                                </h7>
                            </div>

                        </div>

                    </div>
                </div>
            </a>


        </div>
        

</div>
<div class="row">
    <div class="col-sm-4">

            <a asp-action="ViewerForm" asp-route-Exception="Exception Reporting" class="position-relative">
                <div class="card l-bg-new-dark position-relative">
                    @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("Exception Reporting"))
                    {
                        <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                            @ViewBag.UnreadNotifications["Exception Reporting"]
                        </span>
                    }
                    <div class="card-statistic-3 p-4">

                        <div class="">
                            <h6 class="card-title mb-0 head">

                                Exception Reporting

                            </h6>
                        </div>
                        <div class="row align-items-center mb-4 d-flex">
                            <div class="col-8">
                                <h7 class="d-flex align-items-center mb-1">

                                </h7>
                            </div>

                        </div>

                    </div>
                </div>
            </a>


        </div>
        <div class="col-sm-4">

            <a asp-action="ViewerForm" asp-route-Bi2nd="Bi - Monthly Report(2nd Week)" class="position-relative">
                <div class="card l-bg-new2-dark position-relative">
                    @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("Bi - Monthly Report(2nd Week)"))
                    {
                        <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                            @ViewBag.UnreadNotifications["Bi - Monthly Report(2nd Week)"]
                        </span>
                    }
                    <div class="card-statistic-3 p-4">

                        <div class="">
                            <h6 class="card-title mb-0 head">

                               Bi - Monthly Report(2nd Week)

                            </h6>
                        </div>
                        <div class="row align-items-center mb-4 d-flex">
                            <div class="col-8">
                                <h7 class="d-flex align-items-center mb-1">

                                </h7>
                            </div>

                        </div>

                    </div>
                </div>
            </a>


        </div>

in this i just want one thing that when i click on subject card it updates the subject isViewed with 1 along with their pno of session 
