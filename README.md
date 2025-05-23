[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> PositionMaster(AppPositionWorksite appPosition, string actionType)
{
    if (string.IsNullOrEmpty(actionType))
    {
        return BadRequest("No action specified.");
    }

    var existingParameter = await context.AppPositionWorksites.FindAsync(appPosition.Id);

    var UserId = HttpContext.Request.Cookies["Session"];
    if (actionType == "Submit")
    {
        if (!ModelState.IsValid)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Key:{state.Key},Error:{error.ErrorMessage}");
                }
            }
        }


        if (ModelState.IsValid)
        {


            if (existingParameter != null)
            {
                appPosition.CreatedBy = UserId;
                context.Entry(existingParameter).CurrentValues.SetValues(appPosition);
                await context.SaveChangesAsync();
                TempData["Updatedmsg"] = "Position Updated Successfully!";
                return RedirectToAction("PositionMaster");
            }
            else
            {

                appPosition.CreatedBy = UserId;
                await context.AppPositionWorksites.AddAsync(appPosition);
                await context.SaveChangesAsync();
                TempData["msg"] = "Position Added Successfully!";
                return RedirectToAction("PositionMaster");
            }
        }
    }
    else if (actionType == "Delete")
    {
        if (existingParameter != null)
        {
            context.AppPositionWorksites.Remove(existingParameter);
            await context.SaveChangesAsync();
            TempData["Dltmsg"] = "Position Deleted Successfully!";
        }
    }

    return RedirectToAction("PositionMaster");
}

in this i want to check for duplicate records , if duplicate records then send alert 
