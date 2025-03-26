if (!string.IsNullOrEmpty(searchValue))
{
    if (searchType == "Pno")
    {
        // Ensure the searchValue exists as a Pno before fetching the position
        var position = context.AppEmpPositions
            .Where(e => e.Pno == searchValue)
            .Select(e => e.Position)
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(position))
        {
            query = query.Where(p => p.Position == position);
        }
        else
        {
            // Handle the case where Pno is invalid
            ViewBag.ErrorMessage = "No Position found for this P.No.";
        }
    }
    else if (searchType == "Position")
    {
        // Ensure the searchValue exists as a Position before filtering
        bool positionExists = context.AppPositionWorksites.Any(p => p.Position == searchValue);

        if (positionExists)
        {
            query = query.Where(p => p.Position == searchValue);
        }
        else
        {
            // Handle the case where Position is invalid
            ViewBag.ErrorMessage = "No records found for this Position.";
        }
    }
}

 <form method="get" action="@Url.Action("PositionMaster")" style="display:flex;">
    <div class="form-group row">
        <div class="col-sm-2">
            <label class="control-label">Search </label>
        </div>
        <div class="col-md-4 val">
            <input type="text" name="searchValue" class="form-control" 
                value="@ViewBag.SearchValue" placeholder="Enter search value..." autocomplete="off" />
        </div>
        <div class="col-sm-5 d-flex justify-content-end srch2">
            <select class="form-control custom-select" name="searchType">
                <option value="Pno" @(ViewBag.SearchType == "Pno" ? "selected" : "")>Search by P.No.</option>
                <option value="Position" @(ViewBag.SearchType == "Position" ? "selected" : "")>Search by Position</option>
            </select>
        </div>
        <div class="col-sm-1 srchbtn">
            <button type="submit" class="btn btn-primary">Search</button>
        </div>
    </div>
</form>

@if (!string.IsNullOrEmpty(ViewBag.ErrorMessage))
{
    <div class="alert alert-danger">@ViewBag.ErrorMessage</div>
}

 
 <form method="get" action="@Url.Action("PositionMaster")" style="display:flex;">
     <div class="form-group row">
         <div class="col-sm-2">
             <label class="control-label">Search </label>
         </div>
         <div class="col-md-4 val">
             <input type="text" name="searchValue" class="form-control" value="@ViewBag.SearchValue" placeholder="Enter search value..." autocomplete="off" />
         </div>
         <div class="col-sm-5 d-flex justify-content-end srch2">
             <select class="form-control custom-select" name="searchType">
                 @if (ViewBag.SearchType == "Pno")
                 {
                     <option value="Pno" selected>Search by P.No.</option>
                 }
                 else
                 {
                     <option value="Pno">Search by P.No.</option>
                 }
                 @if (ViewBag.SearchType == "Position")
                 {
                     <option value="Position" selected>Search by Position</option>
                 }
                 else
                 {
                     <option value="Position">Search by Position</option>
                 }
             </select>
         </div>
         <div class="col-sm-1 srchbtn">
             <button type="submit" class="btn btn-primary">Search</button>
         </div>
     </div>


 </form>
