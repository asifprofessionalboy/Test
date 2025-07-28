..
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

[HttpGet]
public async Task<IActionResult> CoordinatorMaster(Guid? id, string searchString = "", int page = 1)
{
    var UserId = HttpContext.Request.Cookies["Session"];
    if (string.IsNullOrEmpty(UserId))
        return RedirectToAction("Login", "User");

    ViewBag.CreatedBy = UserId;

    // 🔽 Raw SQL for PNO dropdown
    var pnoList = await context.AppEmployeeMasters
        .FromSqlRaw("SELECT DISTINCT Pno FROM App_EmployeeMaster")
        .Select(e => new SelectListItem { Value = e.Pno, Text = e.Pno })
        .ToListAsync();
    ViewBag.PnoList = pnoList;

    // 🔽 Raw SQL for Department dropdown
    var deptList = await context.DepartmentMasters
        .FromSqlRaw("SELECT DISTINCT DeptName FROM DepartmentMaster")
        .Select(d => new SelectListItem { Value = d.DeptName, Text = d.DeptName })
        .ToListAsync();
    ViewBag.DeptList = deptList;

    // ... rest of your paging logic ...
    return View(new AppCoordinatorMaster());
}
