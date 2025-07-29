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

     
         ViewBag.WorksiteDropdown = context.AppPositionWorksites
             .Select(w => new SelectListItem
             {
                 Value = w.Worksite,
                 Text = w.Worksite
             }).ToList();

      
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




@model GFAS.Models.EmpTagViewModel
@{
    ViewData["Title"] = "Emp Tagging Master";
}
<div class="card rounded-9">
    <div class="row align-items-center form-group">
        <div class="col-md-9">
            <form method="get" action="@Url.Action("EmptaggingMaster")" style="display:flex;">
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
                    <th width="50%">Name</th>
                    <th width="50%">Position</th>
                    <th width="50%">Site</th>
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
    <form asp-action="EmptaggingMaster" asp-controller="Master" method="post">
        @Html.AntiForgeryToken()
        <input type="hidden" name="ActionType" id="actionType" />
        <input type="hidden" name="Coordinators[0].Id" id="Id" value="@Model.Id" />
        <input type="hidden" name="Coordinators[0].CreatedBy" id="CreatedBy" value="@ViewBag.CreatedBy" />
        <input type="hidden" name="Coordinators[0].CreatedOn" id="CreatedOn" value="@Model.CreatedOn" />

        <div class="card mt-3">
            <div class="card-header">Employee Tagging Master Entry</div>
            <div class="card-body">


                <div class="row">

                    <div class="form-group row">







                <div class="col-sm-3">
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

                        </div>

                    <div class="col-sm-2">
                                <input asp-for="Position" class="form-control form-control-sm rangeInput" id="Range" placeholder="" required autocomplete="off" />

                      </div>


                        <div class="col-sm-2">
                            <input asp-for="Range" class="form-control form-control-sm rangeInput" id="Range" placeholder="" required autocomplete="off" />

                        </div>



                        <div class="col-sm-2">
                            <input asp-for="Site" class="form-control form-control-sm rangeInput" id="Range" placeholder="" required autocomplete="off" />

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
                </div>
                </div>

                <div class="text-center">
                    <button type="submit" class="btn btn-success" onclick="setAction('Submit', event)">Submit</button>
                    <button type="submit" class="btn btn-danger" onclick="setAction('Delete', event)">Delete</button>
                </div>
            </div>
        </div>
    </form>
</div>




<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>


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


correct my razor page as per emp tagging master controller
