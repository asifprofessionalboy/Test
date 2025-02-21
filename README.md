public IActionResult Dashboard()
{
    var sessionUser = HttpContext.Session.GetString("Session");
    if (sessionUser != null)
    {
        var subjects = context.AppSubjectMasters.ToList();
        ViewBag.Subjects = subjects;

        // Fetch unread notifications count for each subject for the logged-in user
        var unreadNotifications = context.AppNotifications
            .Where(n => n.Pno == sessionUser && n.IsViewed == false)
            .GroupBy(n => n.Subject)
            .Select(g => new
            {
                Subject = g.Key,
                Count = g.Count()
            })
            .ToDictionary(x => x.Subject, x => x.Count);

        ViewBag.UnreadNotifications = unreadNotifications;

        return View();
    }
    else
    {
        return RedirectToAction("Login", "User");
    }
}



<div class="col-sm-4">
    <a asp-action="ViewerForm" asp-route-MD="MD Communication pack" class="position-relative">
        <div class="card l-bg-cyan-dark position-relative">
            
            <!-- Badge for Unread Notifications -->
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
        </div>
    </a>
</div>

<div class="col-sm-4">
    <a asp-action="ViewerForm" asp-route-Flash="Flash Report" class="position-relative">
        <div class="card l-bg-purple-dark position-relative">
            
            <!-- Badge for Unread Notifications -->
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
            </div>
        </div>
    </a>
</div>




i have this model for Notification
public partial class AppNotification
{
    public Guid Id { get; set; }
    public string? RefNo { get; set; }
    public string? Pno { get; set; }
    public string? Subject { get; set; }
    public string? ChildSubject { get; set; }
    public bool? IsViewed { get; set; }
}

this is my controller method

public IActionResult Dashboard()
{
	if (HttpContext.Session.GetString("Session") != null)
	{
		var subjects = context.AppSubjectMasters.ToList();
		ViewBag.Subjects = subjects;




		return View();
	}
	else
	{
		return RedirectToAction("Login", "User");
	}
}

i have this cards 
<div class="col-sm-4">
        <a asp-action="ViewerForm" asp-route-MD="MD Communication pack" class="position-relative">
            <div class="card l-bg-cyan-dark position-relative">
              
                <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                    10
                </span>

                <div class="card-statistic-3 p-4">
                    <div class="">
                        <h6 class="card-title mb-0 head" name="MD">
                            MD Communication pack
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

        <a asp-action="ViewerForm" asp-route-Flash="Flash Report" class="position-relative">
            <div class="card l-bg-purple-dark position-relative">
                <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                    10
                </span>
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


in this i want that i want to count Subject which is zero and shows on notification badge for each subject differently , and for each pno who is login 
