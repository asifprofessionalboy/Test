@model GFAS.Models.EmpTagViewModel
@{
    ViewData["Title"] = "Emp Tagging Master";
    var pnoList = ViewBag.PnoList as List<SelectListItem>;
       var createdBy = ViewBag.CreatedBy as string;
    var WorksiteDropdown = ViewBag.WorksiteDDList;
}

<div class="card p-3">
    <div class="d-flex justify-content-between mb-3">
        <form method="get" action="@Url.Action("EmptaggingMaster")" class="d-flex">
            <input type="text" name="SearchString" class="form-control" value="@ViewBag.SearchString" placeholder="Search by PNO ..." />
            <button type="submit" class="btn btn-primary ms-2">Search</button>
        </form>
        <button id="showFormButton2" class="btn btn-success">New</button>
    </div>

    <table class="table table-bordered">
        <thead class="table-dark">
            <tr>
                <th>PNO</th>
                <th>Position</th>
                <th>Worksite</th>
            </tr>
        </thead>
        <tbody>
            @if (ViewBag.pList != null)
            {
                foreach (var item in ViewBag.pList)
                {
                    <tr>
                        <td>@item.Pno</td>
                        <td>@item.Postion</td>
                        <td>@item.Worksite</td>
                    </tr>
                }
            }
            else
            {
                <tr><td colspan="3">No records found.</td></tr>
            }
        </tbody>
    </table>

    <div class="text-center">


        <div class="text-center">


            @if (ViewBag.TotalPages > 1)
            {
                <nav aria-label="Page navigation" style="font-size:12px;" class="d-flex justify-content-center">
                    <ul class="pagination">

                        <li class="page-item @(ViewBag.CurrentPage == 1 ? "disabled" : "")">
                            <a class="page-link" asp-action="PositionMaster"
                               asp-route-page="@(ViewBag.CurrentPage - 1)"
                               asp-route-searchString="@ViewBag.SearchValue">
                                Previous
                            </a>
                        </li>


                        @for (int i = Math.Max(1, ViewBag.CurrentPage - 1); i <= Math.Min(ViewBag.CurrentPage + 1, ViewBag.TotalPages); i++)
                        {
                            <li class="page-item @(ViewBag.CurrentPage == i ? "active" : "")">
                                <a class="page-link" asp-action="PositionMaster"
                                   asp-route-page="@i"
                                   asp-route-searchString="@ViewBag.SearchValue">
                                    @i
                                </a>
                            </li>
                        }


                        <li class="page-item @(ViewBag.CurrentPage == ViewBag.TotalPages ? "disabled" : "")">
                            <a class="page-link" asp-action="PositionMaster"
                               asp-route-page="@(ViewBag.CurrentPage + 1)"
                               asp-route-searchString="@ViewBag.SearchValue">
                                Next
                            </a>
                        </li>
                    </ul>
                </nav>
            }

        </div>


    </div>
</div>

<!-- Form Container -->
<div id="formContainer" style="display:none;" class="card mt-4">
    <form asp-action="EmptaggingMaster" method="post">
        @Html.AntiForgeryToken()
        <input type="hidden" name="ActionType" id="actionType" />
        <input type="hidden" name="CreatedBy" value="@createdBy" />
        <input type="hidden" name="Worksite" id="Worksite" />

        <div class="card-header bg-primary text-white">Add Employee Tagging</div>
        <div class="card-body row">

            <div class="col-md-4 mb-3">
                <label for="Pno">PNO</label>
                <select class="form-control" name="Pno" id="Pno" required>
                    <option value="">-- Select PNO --</option>
                    @foreach (var item in pnoList)
                    {
                        <option value="@item.Value">@item.Text</option>
                    }
                </select>
            </div>

            <div class="col-md-2 mb-3">
                <label for="Position">Position</label>
                <input type="number" name="Position" class="form-control" required />
            </div>

            <div class="col-sm-3">
                <div class="dropdown">
                    <input class="dropdown-toggle form-control form-control-sm custom-select" placeholder="" type="button"
                           id="worksiteDropdown" data-bs-toggle="dropdown" aria-expanded="false" />



                    <ul class="dropdown-menu w-100" aria-labelledby="worksiteDropdown" id="locationList">
                        @foreach (var item in WorksiteDropdown)
                        {
                            <li style="margin-left:5%;">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input worksite-checkbox"
                                           value="@item.Value" id="worksite_@item.Value" />
                                    <label class="form-check-label" for="worksite_@item.Value">@item.Text</label>
                                </div>

                            </li>
                        }
                    </ul>

                </div>
                <input type="hidden" id="Worksite" name="Worksite" />



            </div>

        </div>

        <div class="card-footer text-center">
            <button type="submit" class="btn btn-success" onclick="setAction('Submit', event)">Submit</button>
            <button type="submit" class="btn btn-danger" onclick="setAction('Delete', event)">Delete</button>
        </div>
    </form>
</div>

@section Scripts {
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>

    <script>
        function setAction(action, event) {
            if (action === 'Delete' && !confirm("Are you sure?")) {
                event.preventDefault();
                return;
            }

            document.getElementById('actionType').value = action;

            var selected = [];
            $('.worksite-checkbox:checked').each(function () {
                selected.push($(this).val());
            });

            $('#Worksite').val(selected.join(','));
        }

        $(document).ready(function () {
            $('#showFormButton2').click(function () {
                $('#formContainer').slideDown();
            });
        });
    </script>
}


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

   
     var loggedUserDept = context.AppCoordinatorMasters
         .Where(x => x.Pno == UserId)
         .Select(x => x.DeptName)
         .FirstOrDefault();

   
     ViewBag.PnoList = context.AppCoordinatorMasters
         .Where(e => e.DeptName == loggedUserDept)
         .Select(e => new SelectListItem
         {
             Value = e.Pno,
             Text = e.Pno
         }).ToList();

     var WorksiteList = context.AppLocationMasters
           .Select(x => new SelectListItem
           {
               Value = x.Id.ToString(),
               Text = x.WorkSite
           }).Distinct().OrderBy(x => x.Text).ToList();

     ViewBag.WorksiteDDList = WorksiteList;



     var WorksiteList2 = context.AppEmpPositions
         .Select(x => new SelectListItem
         {
             Value = x.Position.ToString(),
             Text = x.Position.ToString()
         }).ToList();

     ViewBag.PositionDDList = WorksiteList2;




      



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
 
         var empPos = new AppEmpPosition
         {
             Id = Guid.NewGuid(),
             Pno = Pno,
             Position = Position
         };
         context.AppEmpPositions.Add(empPos);

     
         var worksites = Worksite.Split(','); 
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


