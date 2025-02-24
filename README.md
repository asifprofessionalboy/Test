this is my model   
public partial class AppNotification
  {
      public Guid Id { get; set; }
      public string? RefNo { get; set; }
      public string? Pno { get; set; }
      public string? Subject { get; set; }
      public string? ChildSubject { get; set; }
      public bool? IsViewed { get; set; }
  }
i have this viewside 

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

in this method i want to execute a update command for notification , when i click on MD Communication pack then the value of IsViewed is set 1 of the Subject and for that Pno of session
and this is my view method  , with that 

	public async Task<IActionResult> ViewerForm(Guid? id, int page = 1, string Bidding = "", string Bi2nd = "",  string Bi4th = "",  string BDWeek = "", string L2 = "", string Flash = "", string Exception = "", string SearchMonth = "", string FinYear = "",string DETP="",string MD="",string BE="",string Admin = "")
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
