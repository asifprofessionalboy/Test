 i want same logic for this dropdown if the option value of division dropdown is corporate services then the Dropdown value text look people function
 <div class="col-sm-1">
     <label class="control-label">Divisions:</label>
 </div>
 <div class="col-sm-5">
     <select id="DivisionDropdown" class="form-control form-control-sm custom-select">
         <option value="" selected></option>
         @foreach (var division in ViewBag.Divisions)
         {
             <option value="@division.Division">@division.Division</option>
         }
     </select>
 </div>
