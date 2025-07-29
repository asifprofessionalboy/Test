@model GFAS.Models.EmpTagViewModel
@{
    ViewData["Title"] = "Emp Tagging Master";
    var pnoList = ViewBag.PnoList as List<SelectListItem>;
    var worksiteList = ViewBag.WorksiteDropdown as List<SelectListItem>;
    var createdBy = ViewBag.CreatedBy as string;
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
                        <td>@item.Position</td>
                        <td>
                            @{
                                var sites = context.AppPositionWorksites
                                    .Where(x => x.Position == item.Position)
                                    .Select(x => x.Worksite)
                                    .ToList();
                            }
                            @foreach (var site in sites)
                            {
                                <span class="badge bg-info text-dark me-1">@site</span>
                            }
                        </td>
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
                    <a class="page-link" asp-route-page="@(ViewBag.CurrentPage - 1)" asp-route-searchString="@ViewBag.SearchString">Previous</a>
                </li>
                @for (int i = 1; i <= ViewBag.TotalPages; i++)
                {
                    <li class="page-item @(ViewBag.CurrentPage == i ? "active" : "")">
                        <a class="page-link" asp-route-page="@i" asp-route-searchString="@ViewBag.SearchString">@i</a>
                    </li>
                }
                <li class="page-item @(ViewBag.CurrentPage == ViewBag.TotalPages ? "disabled" : "")">
                    <a class="page-link" asp-route-page="@(ViewBag.CurrentPage + 1)" asp-route-searchString="@ViewBag.SearchString">Next</a>
                </li>
            </ul>
        </nav>
    }
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

            <div class="col-md-6 mb-3">
                <label>Worksites</label>
                <div class="border rounded p-2" style="max-height: 150px; overflow-y: auto;">
                    @foreach (var site in worksiteList)
                    {
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input worksite-checkbox" value="@site.Value" id="ws_@site.Value" />
                            <label class="form-check-label" for="ws_@site.Value">@site.Text</label>
                        </div>
                    }
                </div>
            </div>

        </div>

        <div class="card-footer text-center">
            <button type="submit" class="btn btn-success" onclick="setAction('Submit', event)">Submit</button>
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

            // collect selected worksite values
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
