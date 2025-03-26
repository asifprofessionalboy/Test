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
