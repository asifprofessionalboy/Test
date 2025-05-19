 <div class="col-sm-2">
     <input asp-for="Cdate" class="form-control form-control-sm date-picker" id="Cdate"autocomplete="off"  type="text">
 </div>

this is my js 

$(function () {
    $(".date-picker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd-mm-yy",
        yearRange: "1900:2100"
    });
});

and this is my controller logic 

  [HttpPost]
  public async Task<IActionResult> CorrectionOfAttendance(AppCoa model)
  {
      var user = HttpContext.Request.Cookies["Session"];
      if (model == null)
      {
          return BadRequest("Invalid data.");
      }

      if (model.Id == Guid.Empty)
      {
         
          model.Id = Guid.NewGuid();
          model.CreatedBy = user;
          
          context.AppCoas.Add(model);
      }
    
      await context.SaveChangesAsync();
      return RedirectToAction("CorrectionOfAttendance");
  }

when data is storing Cdate is store as null
