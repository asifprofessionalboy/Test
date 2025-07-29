..
[HttpGet]
public async Task<IActionResult> EmpTaggingMaster(Guid? id)
{
    var UserId = HttpContext.Request.Cookies["Session"];
    if (string.IsNullOrEmpty(UserId))
        return RedirectToAction("Login", "User");

    ViewBag.CreatedBy = UserId;

    // Get logged-in user's department
    var loggedInEmp = await context1.AppEmployeeMasters.FirstOrDefaultAsync(e => e.Pno == UserId);
    if (loggedInEmp == null)
        return Unauthorized();

    string userDept = loggedInEmp.Department;

    // Filter PNOs by department
    ViewBag.PnoList = context1.AppEmployeeMasters
        .Where(e => e.Department == userDept)
        .Select(e => new SelectListItem
        {
            Value = e.Pno,
            Text = e.Pno
        }).ToList();

    // Get Worksite list via SQL
    List<SelectListItem> worksiteList = new();
    string connectionString = GetRFIDConnectionString();
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();
        string query = "SELECT Id, LocationName FROM App_LocationMaster";
        using (SqlCommand cmd = new SqlCommand(query, conn))
        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                worksiteList.Add(new SelectListItem
                {
                    Value = reader["Id"].ToString(),
                    Text = reader["LocationName"].ToString()
                });
            }
        }
    }

    ViewBag.WorksiteList = worksiteList;

    return View();
}



[HttpPost]
public async Task<IActionResult> EmpTaggingMaster(string Pno, int Position, List<string> WorksiteIds)
{
    var UserId = HttpContext.Request.Cookies["Session"];
    if (string.IsNullOrEmpty(UserId))
        return RedirectToAction("Login", "User");

    // Save in AppEmpPosition
    var empPosition = new AppEmpPosition
    {
        Id = Guid.NewGuid(),
        Pno = Pno,
        Position = Position
    };
    await context.AppEmpPositions.AddAsync(empPosition);

    // Save in AppPositionWorksite
    foreach (var wsId in WorksiteIds)
    {
        var ws = new AppPositionWorksite
        {
            Id = Guid.NewGuid(),
            Position = Position,
            Worksite = wsId, // Save LocationMaster Id
            CreatedBy = UserId,
            CreatedOn = DateTime.Now
        };
        await context.AppPositionWorksites.AddAsync(ws);
    }

    await context.SaveChangesAsync();
    TempData["msg"] = "Tagged Successfully!";
    return RedirectToAction("EmpTaggingMaster");
}
