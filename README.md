[HttpGet]
public async Task<IActionResult> EmptaggingMaster(Guid? id, string searchString = "", int page = 1)
{
    var UserId = HttpContext.Request.Cookies["Session"];
    if (string.IsNullOrEmpty(UserId)) return RedirectToAction("Login", "User");

    var allowed = context.AppPermissionMasters.Select(x => x.Pno).ToList();
    if (!allowed.Contains(UserId)) return RedirectToAction("Login", "User");

    ViewBag.CreatedBy = UserId;

    var userDept = context.AppCoordinatorMasters
        .Where(x => x.Pno == UserId)
        .Select(x => x.DeptName)
        .FirstOrDefault();

    ViewBag.PnoList = context.AppCoordinatorMasters
        .Where(e => e.DeptName == userDept)
        .Select(e => new SelectListItem { Value = e.Pno, Text = e.Pno })
        .ToList();

    ViewBag.WorksiteDDList = context.AppLocationMasters
        .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.WorkSite })
        .Distinct().OrderBy(l => l.Text).ToList();

    int pageSize = 5;
    int skip = (page - 1) * pageSize;

    var sql = @"
        SELECT e.Pno, e.Position, l.WorkSite
        FROM App_Emp_Position e
        JOIN App_PositionWorksite w ON e.Position = w.Position
        JOIN App_LocationMaster l ON w.Worksite = CAST(l.Id AS varchar(36))
        WHERE (@search = '' OR e.Pno LIKE '%' + @search + '%')
        ORDER BY e.Pno
        OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY";

    var data = await context.Set<EmpTagDto>()
        .FromSqlRaw(sql,
            new SqlParameter("@search", searchString ?? ""),
            new SqlParameter("@skip", skip),
            new SqlParameter("@size", pageSize))
        .ToListAsync();

    var totalCount = await context.AppEmpPositions
        .CountAsync(e => string.IsNullOrEmpty(searchString) || e.Pno.Contains(searchString));

    ViewBag.pList = data;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    ViewBag.SearchString = searchString;

    return View();
}

[HttpPost]
public async Task<IActionResult> EmptaggingMaster(string Pno, int Position, string Worksite, string ActionType)
{
    var UserId = HttpContext.Request.Cookies["Session"];
    if (string.IsNullOrEmpty(UserId)) return RedirectToAction("Login", "User");

    if (ActionType == "Submit")
    {
        var emp = new AppEmpPosition { Id = Guid.NewGuid(), Pno = Pno, Position = Position };
        context.AppEmpPositions.Add(emp);

        foreach (var ws in Worksite.Split(','))
            context.AppPositionWorksites.Add(new AppPositionWorksite {
                Id = Guid.NewGuid(),
                Position = Position,
                Worksite = ws.Trim(),
                CreatedBy = UserId,
                CreatedOn = DateTime.Now
            });

        await context.SaveChangesAsync();
        TempData["msg"] = "Data saved successfully.";
    }

    return RedirectToAction("EmptaggingMaster");
}


public class EmpTagDto
{
    public string Pno { get; set; }
    public int Position { get; set; }
    public string WorkSite { get; set; }
}


@{
    ViewData["Title"] = "Emp Tagging Master";
    var pnoList = ViewBag.PnoList as List<SelectListItem>;
    var worksiteList = ViewBag.WorksiteDDList as List<SelectListItem>;
}
<div class="card p-3">
    <div class="d-flex justify-content-between mb-3">
        <form method="get" class="d-flex" asp-action="EmptaggingMaster">
            <input type="text" name="SearchString" class="form-control" value="@ViewBag.SearchString" placeholder="Search by PNO..." />
            <button type="submit" class="btn btn-primary ms-2">Search</button>
        </form>
        <button id="showForm" class="btn btn-success">New</button>
    </div>

    <table class="table table-bordered">
        <thead class="table-dark"><tr><th>PNO</th><th>Position</th><th>Worksite</th></tr></thead>
        <tbody>
            @if (ViewBag.pList != null)
            {
                foreach (var item in ViewBag.pList as List<EmpTagDto>)
                {
                    <tr>
                        <td>@item.Pno</td>
                        <td>@item.Position</td>
                        <td>@item.WorkSite</td>
                    </tr>
                }
            }
            else
            {
                <tr><td colspan="3">No records found.</td></tr>
            }
        </tbody>
    </table>

    @if (ViewBag.TotalPages > 1)
    {
        <nav class="d-flex justify-content-center">
            <ul class="pagination">
                <li class="page-item @(ViewBag.CurrentPage == 1 ? "disabled" : "")">
                    <a asp-action="EmptaggingMaster" asp-route-page="@(ViewBag.CurrentPage-1)" asp-route-searchString="@ViewBag.SearchString" class="page-link">Previous</a>
                </li>
                @for (int i = 1; i <= ViewBag.TotalPages; i++)
                {
                    <li class="page-item @(ViewBag.CurrentPage==i ? "active" : "")">
                        <a asp-route-page="@i" asp-route-searchString="@ViewBag.SearchString" asp-action="EmptaggingMaster" class="page-link">@i</a>
                    </li>
                }
                <li class="page-item @(ViewBag.CurrentPage==ViewBag.TotalPages ? "disabled" : "")">
                    <a asp-action="EmptaggingMaster" asp-route-page="@(ViewBag.CurrentPage+1)" asp-route-searchString="@ViewBag.SearchString" class="page-link">Next</a>
                </li>
            </ul>
        </nav>
    }
</div>

<div id="form" style="display:none;" class="card mt-4">
    <form method="post" asp-action="EmptaggingMaster">
        @Html.AntiForgeryToken()
        <input type="hidden" name="CreatedBy" value="@ViewBag.CreatedBy" />
        <input type="hidden" id="worksiteHidden" name="Worksite" />
        <input type="hidden" id="actionType" name="ActionType" />

        <div class="card-body row">
            <div class="col-md-4">
                <label>PNO</label>
                <select name="Pno" class="form-control" required>
                    <option value="">--Select--</option>
                    @foreach(var i in pnoList)
                        <option value="@i.Value">@i.Text</option>
                </select>
            </div>
            <div class="col-md-2">
                <label>Position</label>
                <input type="number" name="Position" class="form-control" required />
            </div>
            <div class="col-md-6">
                <label>Worksites</label>
                <div class="border p-2" style="max-height:150px;overflow-y:auto;">
                    @foreach(var w in worksiteList)
                    {
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input ws-chk" value="@w.Value" id="w_@w.Value" />
                            <label class="form-check-label" for="w_@w.Value">@w.Text</label>
                        </div>
                    }
                </div>
            </div>
        </div>
        <div class="card-footer">
            <button type="submit" class="btn btn-success" onclick="submitAction('Submit')">Submit</button>
        </div>
    </form>
</div>

@section Scripts {
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script>
        function submitAction(act) {
            $('#actionType').val(act);
            var arr = [];
            $('.ws-chk:checked').each(function(){ arr.push($(this).val()); });
            $('#worksiteHidden').val(arr.join(','));
        }
        $(function(){ $('#showForm').click(()=>$('#form').slideDown()); });
    </script>
}
