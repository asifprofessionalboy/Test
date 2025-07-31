  WHERE 1=1 and
  {locationFilter}
     and
  {departmentFilter}


   [HttpPost]
 public async Task<IActionResult> EmpTaggingMaster(string Pno, int Position, string Worksite, string ActionType)
 {
     var UserId = HttpContext.Request.Cookies["Session"];
     if (string.IsNullOrEmpty(UserId))
         return RedirectToAction("Login", "User");

     if (ActionType == "Delete")
     {
         var empPosition = await context.AppEmpPositions.FirstOrDefaultAsync(e => e.Pno == Pno);
         if (empPosition != null)
             context.AppEmpPositions.Remove(empPosition);

         var positionWorksite = await context.AppPositionWorksites.FirstOrDefaultAsync(w => w.Position == Position);
         if (positionWorksite != null)
             context.AppPositionWorksites.Remove(positionWorksite);

         await context.SaveChangesAsync();
         TempData["Dltmsg2"] = "Deleted Successfully!";
         return RedirectToAction("EmpTaggingMaster");
     }

     // Save or update position
     var existingEmp = await context.AppEmpPositions.FirstOrDefaultAsync(e => e.Pno == Pno);
     if (existingEmp != null)
     {
         existingEmp.Position = Position;
         context.AppEmpPositions.Update(existingEmp);
     }
     else
     {
         var empPosition = new AppEmpPosition
         {
             Id = Guid.NewGuid(),
             Pno = Pno,
             Position = Position
         };
         await context.AppEmpPositions.AddAsync(empPosition);
     }


     // Save or update worksite
     var existingWorksite = await context.AppPositionWorksites.FirstOrDefaultAsync(w => w.Position == Position);


     if (existingWorksite != null)
     {
         existingWorksite.Worksite = Worksite;
         existingWorksite.CreatedBy = UserId;
         existingWorksite.CreatedOn = DateTime.Now;
         context.AppPositionWorksites.Update(existingWorksite);
     }
     else
     {
         var ws = new AppPositionWorksite
         {
             Id = Guid.NewGuid(),
             Position = Position,
             Worksite = Worksite,
             CreatedBy = UserId,
             CreatedOn = DateTime.Now
         };
         await context.AppPositionWorksites.AddAsync(ws);
     }

     await context.SaveChangesAsync();
     TempData["msg2"] = "Tagged Successfully!";
     return RedirectToAction("EmpTaggingMaster");
 }
