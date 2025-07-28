<!-- PNO Dropdown -->
<div class="mb-3">
    <label for="Pno" class="form-label">PNO</label>
    <select name="Coordinators[0].Pno" id="Pno" class="form-control" required>
        <option value="">-- Select PNO --</option>
        @foreach (var item in ViewBag.PnoList as List<SelectListItem>)
        {
            <option value="@item.Value">@item.Text</option>
        }
    </select>
</div>

<!-- Department Dropdown -->
<div class="mb-3">
    <label for="DeptName" class="form-label">Department</label>
    <select name="Coordinators[0].DeptName" id="DeptName" class="form-control" required>
        <option value="">-- Select Department --</option>
        @foreach (var item in ViewBag.DeptList as List<SelectListItem>)
        {
            <option value="@item.Value">@item.Text</option>
        }
    </select>
</div>



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

    // Dropdown for Pno from App_EmployeeMaster
    ViewBag.PnoList = context.AppEmployeeMasters
        .Select(e => new SelectListItem
        {
            Value = e.Pno,
            Text = e.Pno
        })
        .ToList();

    // Dropdown for DeptName from DepartmentMaster
    ViewBag.DeptList = context.DepartmentMasters
        .Select(d => new SelectListItem
        {
            Value = d.DeptName,
            Text = d.DeptName
        })
        .ToList();

    // Rest of your existing logic...
    int pageSize = 5;
    var query = context.AppCoordinatorMasters.AsQueryable();

    if (!string.IsNullOrEmpty(searchString))
        query = query.Where(c => c.Pno.Contains(searchString));

    var data = query.OrderBy(c => c.Pno).ToList();
    var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    ViewBag.pList = pagedData;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
    ViewBag.SearchString = searchString;

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
