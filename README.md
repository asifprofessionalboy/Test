[HttpPost]
public async Task<IActionResult> SubmitTransactions(AppSubjectMaster model)
{
    if (model == null)
    {
        return BadRequest("Invalid data.");
    }

    if (model.Id == Guid.Empty)  // Create new record
    {
        model.Id = Guid.NewGuid();  // Generate new GUID for new record
        model.CreatedOn = DateTime.UtcNow;
        context.AppSubjectMasters.Add(model);
    }
    else  // Update existing record
    {
        var existingRecord = await context.AppSubjectMasters.FindAsync(model.Id);
        if (existingRecord != null)
        {
            existingRecord.Subject = model.Subject;
            existingRecord.CreatedBy = model.CreatedBy;
            existingRecord.CreatedOn = model.CreatedOn; // Update only if needed
            context.AppSubjectMasters.Update(existingRecord);
        }
        else
        {
            return NotFound("Record not found.");
        }
    }

    await context.SaveChangesAsync();
    return RedirectToAction("SubjectMaster");
}

[HttpPost]
public async Task<IActionResult> DeleteTransaction(Guid id)
{
    var record = await context.AppSubjectMasters.FindAsync(id);
    if (record == null)
    {
        return NotFound("Record not found.");
    }

    context.AppSubjectMasters.Remove(record);
    await context.SaveChangesAsync();
    
    return Json(new { success = true, message = "Deleted successfully!" });
}



public async Task<IActionResult> SubjectMaster(Guid? id, int page = 1, string searchString = "")
{
    int pageSize = 5;
    var query = context.AppSubjectMasters.AsQueryable();

    if (!string.IsNullOrEmpty(searchString))
    {
        query = query.Where(a => a.Subject.Contains(searchString));
    }

    var pagedData = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    var totalCount = query.Count();

    ViewBag.ListData2 = pagedData;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    ViewBag.SearchString = searchString;

    // Initialize a new view model
    AppSubjectMaster viewModel = null;

    if (id.HasValue)
    {
        viewModel = await context.AppSubjectMasters.FirstOrDefaultAsync(a => a.Id == id);
    }

    return View(viewModel);
}




i have this model 
 public partial class AppSubjectMaster
 {
     public AppSubjectMaster()
     {

         AppSubjectMasters = new AppSubjectMaster();
        
     }

     public AppSubjectMaster AppSubjectMasters { get; set; }

     public Guid Id { get; set; }
     public string? Subject { get; set; }
     public string? CreatedBy { get; set; }
     public DateTime? CreatedOn { get; set; }
 }

this is my controller method 

 public async Task<IActionResult> SubjectMaster(Guid? id, int page = 1, string searchString = "")
 {
     int pageSize = 5;
     var query = context.AppSubjectMasters.AsQueryable();

     if (!string.IsNullOrEmpty(searchString))
     {
         query = query.Where(a => a.Subject.Contains(searchString));
     }

     var pagedData = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
     var totalCount = query.Count();

     ViewBag.ListData2 = pagedData;
     ViewBag.CurrentPage = page;
     ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
     ViewBag.SearchString = searchString;

     var viewModel = new AppSubjectMaster(); 

     if (id.HasValue)
     {
         var selectedTransaction = await context.AppSubjectMasters.FirstOrDefaultAsync(a => a.Id == id);
         if (selectedTransaction != null)
         {
             viewModel.AppSubjectMasters = selectedTransaction;
         }
     }

     return View(viewModel);
 }

getting error SqlException: Invalid column name 'AppSubjectMastersId'.
