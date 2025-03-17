
<div class="text-center">
    <input type="hidden" name="action" id="actionField" />

    @if (User.HasClaim(c => c.Type == "Permission" && c.Value == "CanModify"))
    {
        <input type="submit" value="Save" class="btn" style="border-radius:7px" onclick="setAction('Save')" />
    }

    @if (User.HasClaim(c => c.Type == "Permission" && c.Value == "CanDelete"))
    {
        <input type="submit" value="Delete" class="btn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;" onclick="setAction('Delete')" />
    }
</div>
<script>
    document.addEventListener("DOMContentLoaded", function () {
        var message = '@TempData["Message"]';
        var actionType = '@TempData["ActionType"]';

        if (message) {
            if (actionType === "Delete") {
                Swal.fire({
                    title: "Deleted!",
                    text: message,
                    icon: "success",
                    timer: 3000
                });
            } else {
                Swal.fire({
                    title: "Success!",
                    text: message,
                    icon: "success",
                    timer: 3000
                });
            }
        }
    });
</script>
[Authorize(Policy = "CanModify")]
[HttpPost]
public async Task<IActionResult> EditDocument(AppTechnicalService technicalService, string action, string RefNo = "")
{
    if (action == "Delete")
    {
        var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
        if (existingTechnicalService != null)
        {
            context.AppTechnicalServices.Remove(existingTechnicalService);
            await context.SaveChangesAsync();
            TempData["Message"] = "Document deleted successfully!";
            TempData["ActionType"] = "Delete"; // Store action type
        }
    }
    else if (action == "Save")
    {
        if (ModelState.IsValid)
        {
            var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
            if (existingTechnicalService != null)
            {
                // Handle file uploads
                if (technicalService.Attach != null && technicalService.Attach.Any())
                {
                    var uploadPath = configuration["FileUpload:Path"];
                    var newAttachments = new List<string>();

                    foreach (var file in technicalService.Attach)
                    {
                        if (file.Length > 0)
                        {
                            var uniqueId = Guid.NewGuid().ToString();
                            var currentDateTime = DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss");
                            var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var fileExtension = Path.GetExtension(file.FileName);
                            var formattedFileName = $"{uniqueId}_{currentDateTime}_{originalFileName}{fileExtension}";
                            var fullPath = Path.Combine(uploadPath, formattedFileName);

                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            newAttachments.Add(formattedFileName);
                        }
                    }

                    existingTechnicalService.Attachment = string.Join(",", newAttachments);
                }

                if (technicalService.Attach == null || !technicalService.Attach.Any())
                {
                    technicalService.Attachment = existingTechnicalService.Attachment;
                }

                var CreatedBy = HttpContext.Session.GetString("Session");
                existingTechnicalService.CreatedBy = CreatedBy;
                existingTechnicalService.CreatedOn = DateTime.Now;

                existingTechnicalService.RefNo = RefNo;
                existingTechnicalService.FinYear = technicalService.FinYear;
                existingTechnicalService.Month = technicalService.Month;
                existingTechnicalService.Department = technicalService.Department;
                existingTechnicalService.Subject = technicalService.Subject;

                await context.SaveChangesAsync();
                TempData["Message"] = "Document updated successfully!";
                TempData["ActionType"] = "Save"; // Store action type
            }
        }
    }

    return RedirectToAction("EditDocument", "Technical");
}

