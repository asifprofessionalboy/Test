this is my controller logic 

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
         model.CreatedBy = user;
         
         context.AppCoas.Add(model);
     }
     else
     {
         var existingRecord = await context.AppCoas.FindAsync(model.Id);
         if (existingRecord != null)
         {
             existingRecord.CreatedBy = user;
             context.AppCoas.Update(existingRecord);
         }
         else
         {
             return NotFound("Record not found.");
         }
     }

     await context.SaveChangesAsync();
     return RedirectToAction("SubjectMaster");
 }

i have this in cshtml 

 <div class="col-sm-2">

     <div class="row">
         <div class="col-sm-6">
             <input asp-for="Intime" class="form-control form-control-sm" id="IntimeHH" value="" type="text" placeholder="(HH)">

             </div>
             <div class="col-sm-6">

                  <input asp-for="Intime" class="form-control form-control-sm" id="IntimeMM" value="" type="text"  placeholder="(mm)">
             </div>
         
    
         </div>

     
 </div>


i want to store in InTime if the user Input like in HH: 09 mm: 30 then i want to store in column like 09:30
