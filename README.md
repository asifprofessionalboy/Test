<script>
    $(document).ready(function () {
        $(".searchable-dropdown").select2({
            placeholder: "Select a Position",
            allowClear: true
        });
    });
</script>

 <div class="col-sm-3">
    <select asp-for="Position" asp-items="@ViewBag.PositionDDList" class="form-control form-control-sm searchable-dropdown">
        <option value=""></option>
    </select>
</div>

<link href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/jquery@3.6.0/dist/jquery.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>



 
 <div class="col-sm-3">
   
     <select asp-for="Position" asp-items="@ViewBag.PositionDDList" class="form-control form-control-sm custom-select" id="Position">
         <option value=""></option>
     </select>
     
 </div>
