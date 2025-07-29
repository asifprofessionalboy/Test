[HttpGet]
public async Task<IActionResult> EmptaggingMaster(Guid? id, string searchString = "", int page = 1)
{
    var UserId = HttpContext.Request.Cookies["Session"];
    if (string.IsNullOrEmpty(UserId))
        return RedirectToAction("Login", "User");

    var allowedPnos = context.AppPermissionMasters.Select(x => x.Pno).ToList();
    if (!allowedPnos.Contains(UserId))
        return RedirectToAction("Login", "User");

    ViewBag.CreatedBy = UserId;

    // Get the department of the logged-in user
    var loggedUserDept = context1.AppEmployeeMasters
        .Where(x => x.Pno == UserId)
        .Select(x => x.DeptName)
        .FirstOrDefault();

    // Populate PNO dropdown for same department only
    ViewBag.PnoList = context1.AppEmployeeMasters
        .Where(e => e.DeptName == loggedUserDept)
        .Select(e => new SelectListItem
        {
            Value = e.Pno,
            Text = e.Pno
        }).ToList();

    // Populate Worksite checkboxes
    ViewBag.WorksiteDropdown = context1.AppWorksiteMasters
        .Select(w => new SelectListItem
        {
            Value = w.Worksite,
            Text = w.Worksite
        }).ToList();

    // Paging logic - optional
    int pageSize = 5;
    var query = context.AppEmpPositions.AsQueryable();
    var data = query.OrderBy(x => x.Pno).ToList();
    var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    ViewBag.pList = pagedData;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
    ViewBag.SearchString = searchString;

    return View(new EmpTagViewModel());
}



[HttpPost]
public async Task<IActionResult> EmptaggingMaster(string Pno, int Position, string Worksite, string ActionType)
{
    var UserId = HttpContext.Request.Cookies["Session"];
    if (string.IsNullOrEmpty(UserId))
        return RedirectToAction("Login", "User");

    if (ActionType == "Submit")
    {
        // Save to AppEmpPosition
        var empPos = new AppEmpPosition
        {
            Id = Guid.NewGuid(),
            Pno = Pno,
            Position = Position
        };
        context.AppEmpPositions.Add(empPos);

        // Save multiple worksites
        var worksites = Worksite.Split(','); // comma-separated from hidden field
        foreach (var ws in worksites)
        {
            var posWorksite = new AppPositionWorksite
            {
                Id = Guid.NewGuid(),
                Position = Position,
                Worksite = ws.Trim(),
                CreatedBy = UserId,
                CreatedOn = DateTime.Now
            };
            context.AppPositionWorksites.Add(posWorksite);
        }

        await context.SaveChangesAsync();
        TempData["msg"] = "Data saved successfully.";
    }

    return RedirectToAction("EmptaggingMaster");
}
