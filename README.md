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

    int pageSize = 5;
    var query = context.AppCoordinatorMasters.AsQueryable();

    if (!string.IsNullOrEmpty(searchString))
    {
        query = query.Where(c => c.Pno.Contains(searchString));
    }

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

[HttpPost]
public async Task<IActionResult> CoordinatorMaster(CoordinatorWrapper model)
{
    var UserId = HttpContext.Request.Cookies["Session"];

    if (model == null || model.Coordinators == null || !model.Coordinators.Any())
        return BadRequest("No coordinator data received.");

    if (string.IsNullOrEmpty(model.ActionType))
        return BadRequest("No action specified.");

    if (model.ActionType == "Submit")
    {
        foreach (var coordinator in model.Coordinators)
        {
            coordinator.CreatedBy = UserId;
            coordinator.CreatedOn = DateTime.Now;

            var existing = await context.AppCoordinatorMasters.FindAsync(coordinator.Id);
            if (existing != null)
                context.Entry(existing).CurrentValues.SetValues(coordinator);
            else
                await context.AppCoordinatorMasters.AddAsync(coordinator);
        }

        await context.SaveChangesAsync();
        TempData["msg"] = "Coordinator Saved Successfully!";
    }
    else if (model.ActionType == "Delete")
    {
        foreach (var coordinator in model.Coordinators)
        {
            var existing = await context.AppCoordinatorMasters.FindAsync(coordinator.Id);
            if (existing != null)
                context.AppCoordinatorMasters.Remove(existing);
        }

        await context.SaveChangesAsync();
        TempData["Dltmsg"] = "Coordinator Deleted Successfully!";
    }

    return RedirectToAction("CoordinatorMaster");
}
