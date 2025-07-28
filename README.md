...
[HttpGet]
public async Task<IActionResult> CoordinatorMaster(Guid? id, string searchString = "", int page = 1)
{
    var UserId = HttpContext.Request.Cookies["Session"];

    if (string.IsNullOrEmpty(UserId))
        return RedirectToAction("Login", "User");

    var allowedPnos = context.AppPermissionMasters.Select(x => x.Pno).ToList();
    if (!allowedPnos.Contains(UserId))
        return RedirectToAction("Login", "User");

    ViewBag.CreatedBy = UserId;

    // ✅ PNO Dropdown from App_EmployeeMaster
    ViewBag.PnoList = context1.AppEmployeeMasters
        .Select(e => new SelectListItem
        {
            Value = e.Pno,
            Text = e.Pno
        }).ToList();

    // ✅ Department Dropdown from App_DepartmentMaster using raw SQL
    var deptList = new List<SelectListItem>();
    string connectionString = GetRFIDConnectionString();

    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();

        string query = "SELECT DISTINCT DepartmentName FROM App_DepartmentMaster";
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    deptList.Add(new SelectListItem
                    {
                        Value = reader["DepartmentName"].ToString(),
                        Text = reader["DepartmentName"].ToString()
                    });
                }
            }
        }
    }

    ViewBag.DeptList = deptList;

    // ✅ Main Coordinator list
    int pageSize = 5;
    var queryCoordinators = context.AppCoordinatorMasters.AsQueryable();

    if (!string.IsNullOrEmpty(searchString))
        queryCoordinators = queryCoordinators.Where(c => c.Pno.Contains(searchString));

    var data = queryCoordinators.OrderBy(c => c.Pno).ToList();
    var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    ViewBag.pList = pagedData;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
    ViewBag.SearchString = searchString;

    // ✅ For editing a specific record
    if (id.HasValue)
    {
        var model = await context.AppCoordinatorMasters.FindAsync(id.Value);
        if (model == null)
            return NotFound();

        return Json(new
        {
            id = model.Id,
            pno = model.Pno,
            dept = model.DeptName,
            createdby = model.CreatedBy,
            createdon = model.CreatedOn
        });
    }

    return View(new AppCoordinatorMaster());
}
