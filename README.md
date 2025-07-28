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




@model GFAS.Models.AppCoordinatorMaster
@{
    ViewData["Title"] = "Coordinator Master";
}
<div class="card rounded-9">
    <div class="row align-items-center form-group">
        <div class="col-md-9">
            <form method="get" action="@Url.Action("CoordinatorMaster")" style="display:flex;">
                <div class="col-md-4">
                    <input type="text" name="SearchString" class="form-control" value="@ViewBag.SearchString" placeholder="Search by PNO ..." autocomplete="off" />
                </div>
                <div class="col-md-3" style="padding-left:1%;">
                    <button type="submit" class="btn btn-primary">Search</button>
                </div>
            </form>
        </div>
        <div class="col-md-3 mb-2 text-end">
            <button id="showFormButton2" class="btn btn-primary">New</button>
        </div>
    </div>

    <div class="col-md-12">
        <table class="table table-bordered" id="myTable">
            <thead class="table" style="background-color: #d2b1ff;color: #000000;">
                <tr>
                    <th width="50%">PNO</th>
                    <th width="50%">Department</th>
                </tr>
            </thead>
            <tbody>
                @if (ViewBag.pList != null)
                {
                    foreach (var item in ViewBag.pList)
                    {
                        <tr>
                            <td>
                                <a href="javascript:void(0);" data-id="@item.Id" class="OpenFilledForm btn gridbtn" style="text-decoration:none;background-color:#ffffff;font-weight:bolder;color:darkblue;">
                                    @item.Pno
                                </a>
                            </td>
                            <td>@item.DeptName</td>
                        </tr>
                    }
                }
                else
                {
                    <tr>
                        <td colspan="2">No data available</td>
                    </tr>
                }
            </tbody>
        </table>

        <div class="text-center">
            @if (ViewBag.TotalPages > 1)
            {
                <nav aria-label="Page navigation" class="d-flex justify-content-center">
                    <ul class="pagination">
                        <li class="page-item @(ViewBag.CurrentPage == 1 ? "disabled" : "")">
                            <a class="page-link" asp-action="CoordinatorMaster" asp-route-page="@(ViewBag.CurrentPage - 1)" asp-route-searchString="@ViewBag.SearchString">Previous</a>
                        </li>
                        @for (int i = 1; i <= ViewBag.TotalPages; i++)
                        {
                            <li class="page-item @(ViewBag.CurrentPage == i ? "active" : "")">
                                <a class="page-link" asp-action="CoordinatorMaster" asp-route-page="@i" asp-route-searchString="@ViewBag.SearchString">@i</a>
                            </li>
                        }
                        <li class="page-item @(ViewBag.CurrentPage == ViewBag.TotalPages ? "disabled" : "")">
                            <a class="page-link" asp-action="CoordinatorMaster" asp-route-page="@(ViewBag.CurrentPage + 1)" asp-route-searchString="@ViewBag.SearchString">Next</a>
                        </li>
                    </ul>
                </nav>
            }
        </div>
    </div>
</div>

<div id="formContainer" style="display:none;">
    <form asp-action="CoordinatorMaster" asp-controller="Master" method="post">
        @Html.AntiForgeryToken()
        <input type="hidden" name="ActionType" id="actionType" />
        <input type="hidden" name="Coordinators[0].Id" id="Id" value="@Model.Id" />
        <input type="hidden" name="Coordinators[0].CreatedBy" id="CreatedBy" value="@ViewBag.CreatedBy" />
        <input type="hidden" name="Coordinators[0].CreatedOn" id="CreatedOn" value="@Model.CreatedOn" />

        <div class="card mt-3">
            <div class="card-header">Coordinator Master Entry</div>
            <div class="card-body">
                <div class="mb-3">
                    <label for="Pno" class="form-label">PNO</label>
                    <input type="text" name="Coordinators[0].Pno" class="form-control" id="Pno" required />
                </div>
                <div class="mb-3">
                    <label for="DeptName" class="form-label">Department</label>
                    <input type="text" name="Coordinators[0].DeptName" class="form-control" id="DeptName" required />
                </div>
                <div class="text-center">
                    <button type="submit" class="btn btn-success" onclick="setAction('Submit', event)">Submit</button>
                    <button type="submit" class="btn btn-danger" onclick="setAction('Delete', event)">Delete</button>
                </div>
            </div>
        </div>
    </form>
</div>

<script>
    function setAction(action, event) {
        if (action === 'Delete' && !confirm("Are you sure you want to delete this record?")) {
            event.preventDefault();
            return;
        }
        document.getElementById('actionType').value = action;
    }

    $(document).ready(function () {
        $('#showFormButton2').click(function () {
            $('#formContainer').show();
            $('#Pno, #DeptName').val('');
            $('#Id').val('');
        });

        $('.OpenFilledForm').click(function () {
            const id = $(this).data('id');
            $.ajax({
                url: '@Url.Action("CoordinatorMaster", "Master")',
                data: { id: id },
                success: function (data) {
                    $('#Id').val(data.id);
                    $('#Pno').val(data.pno);
                    $('#DeptName').val(data.dept);
                    $('#CreatedBy').val(data.createdby);
                    $('#CreatedOn').val(data.createdon);
                    $('#formContainer').show();
                },
                error: function () {
                    alert("Error loading data");
                }
            });
        });
    });
</script>


public class CoordinatorWrapper
{
    public List<AppCoordinatorMaster> Coordinators { get; set; }
    public string ActionType { get; set; }
}
