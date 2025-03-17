@model YourNamespace.Models.Person

@{
    ViewData["Title"] = "Face Recognition";
}

<h2>Face Recognition Login</h2>

<form id="photoForm" asp-action="VerifyFace" method="post" enctype="multipart/form-data">
    <div class="form-group">
        <label>Pno</label>
        <input type="text" id="pno" name="Pno" class="form-control" required />
    </div>

    <div class="form-group">
        <video id="video" width="320" height="240" autoplay></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>

    <div class="form-group">
        <button type="button" id="captureBtn" class="btn btn-primary">Capture Photo</button>
    </div>

    <input type="hidden" id="photoInput" name="photoData" />

    <button type="submit" class="btn btn-success">Verify Face</button>
</form>

<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const captureBtn = document.getElementById("captureBtn");
    const photoInput = document.getElementById("photoInput");

    // Access user webcam
    navigator.mediaDevices.getUserMedia({ video: true })
        .then(stream => { video.srcObject = stream; })
        .catch(err => { console.error("Camera Access Denied", err); });

    // Capture image from video
    captureBtn.addEventListener("click", () => {
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        photoInput.value = canvas.toDataURL("image/png"); // Convert to Base64
    });
</script>

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YourNamespace.Data;
using YourNamespace.Models;



[HttpPost("UploadPhoto")]
public IActionResult UploadPhoto([FromBody] PhotoUploadRequest request)
{
    var employee = _context.TblEmployees.FirstOrDefault(e => e.Pno == request.Pno);
    if (employee == null)
        return BadRequest(new { message = "Employee not found" });

    // Convert Base64 Image to Byte Array
    byte[] imageBytes = Convert.FromBase64String(request.Image.Split(',')[1]);

    // Store Image in Database
    employee.Photo = imageBytes;
    _context.SaveChanges();

    return Ok(new { message = "Photo uploaded successfully!" });
}

// Request Model
public class PhotoUploadRequest
{
    public string Pno { get; set; }
    public string Image { get; set; } // Base64 Image
}
@model YourNamespace.Models.Person

@{
    ViewData["Title"] = "Add Person";
}

<h2>Add Person</h2>

<form asp-action="Create" method="post" enctype="multipart/form-data">
    <div class="form-group">
        <label asp-for="Pno">Pno</label>
        <input asp-for="Pno" class="form-control" readonly value="@Guid.NewGuid()" />
    </div>

    <div class="form-group">
        <label asp-for="Name">Name</label>
        <input asp-for="Name" class="form-control" required />
    </div>

    <div class="form-group">
        <label>Upload Photo</label>
        <input type="file" id="photoInput" name="photoFile" class="form-control" accept="image/*" required />
    </div>

    <div class="form-group">
        <img id="previewImage" src="" alt="Image Preview" style="width: 200px; display: none;" />
    </div>

    <button type="submit" class="btn btn-primary">Save Details</button>
</form>

<script>
    document.getElementById("photoInput").addEventListener("change", function (event) {
        let reader = new FileReader();
        reader.onload = function () {
            let img = document.getElementById("previewImage");
            img.src = reader.result;
            img.style.display = "block";
        };
        reader.readAsDataURL(event.target.files[0]);
    });
</script>

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using YourNamespace.Data;
using YourNamespace.Models;

public class PersonController : Controller
{
    private readonly ApplicationDbContext _context;

    public PersonController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Person/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Person/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Person person, IFormFile photoFile)
    {
        if (ModelState.IsValid)
        {
            if (photoFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photoFile.CopyToAsync(memoryStream);
                    person.Photo = memoryStream.ToArray(); // Convert Image to Byte Array
                }
            }

            _context.Add(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(person);
    }

    // GET: Person/Details/{pno}
    public async Task<IActionResult> Details(Guid pno)
    {
        var person = await _context.Persons.FirstOrDefaultAsync(m => m.Pno == pno);
        if (person == null) return NotFound();
        return View(person);
    }

    // GET: Photo from Database
    public async Task<IActionResult> GetPhoto(Guid pno)
    {
        var person = await _context.Persons.FirstOrDefaultAsync(m => m.Pno == pno);
        if (person == null || person.Photo == null)
            return NotFound();

        return File(person.Photo, "image/jpeg"); // Return image file
    }
}


[Authorize]
[HttpPost]
public async Task<IActionResult> EditDocument(AppTechnicalService technicalService, string action, string RefNo = "")
{
    var userIdString = HttpContext.Session.GetString("ID");
    if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
    {
        return RedirectToAction("AccessDenied");
    }

    string formName = "EditDocument"; 
    var form = await context.AppFormDetails
        .Where(f => f.FormName == formName)
        .Select(f => f.Id)
        .FirstOrDefaultAsync();

    if (form == default)
    {
        return RedirectToAction("AccessDenied");
    }

    bool canModify = await context.AppUserFormPermissions
        .Where(p => p.UserId == userId && p.FormId == form)
        .AnyAsync(p => p.AllowModify == true);

    bool canDelete = await context.AppUserFormPermissions
        .Where(p => p.UserId == userId && p.FormId == form)
        .AnyAsync(p => p.AllowDelete == true);

    if (action == "Delete")
    {
        if (!canDelete)
        {
            return RedirectToAction("AccessDenied");
        }

        var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
        if (existingTechnicalService != null)
        {
            context.AppTechnicalServices.Remove(existingTechnicalService);
            await context.SaveChangesAsync();
            TempData["Message"] = "Document deleted successfully!";
        }
    }
    else if (action == "Save")
    {
        if (!canModify)
        {
            return RedirectToAction("AccessDenied");
        }

        if (ModelState.IsValid)
        {
            var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
            if (existingTechnicalService != null)
            {
                existingTechnicalService.RefNo = RefNo;
                existingTechnicalService.FinYear = technicalService.FinYear;
                existingTechnicalService.Month = technicalService.Month;
                existingTechnicalService.Department = technicalService.Department;
                existingTechnicalService.Subject = technicalService.Subject;

                await context.SaveChangesAsync();
                TempData["Message"] = "Document updated successfully!";
            }
        }
    }

    return RedirectToAction("EditDocument", "Technical");
}

@inject IAuthorizationService AuthorizationService

<div class="text-center">
    <input type="hidden" name="action" id="actionField" />

    @if (ViewBag.CanModify == true)
    {
        <input type="submit" value="Save" class="btn" style="border-radius:7px" onclick="setAction('Save')" />
    }

    @if (ViewBag.CanDelete == true)
    {
        <input type="submit" value="Delete" class="btn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;" onclick="setAction('Delete')" />
    }
</div>
[Authorize]
public async Task<IActionResult> EditDocument(Guid id)
{
    var userIdString = HttpContext.Session.GetString("ID");
    if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
    {
        return RedirectToAction("AccessDenied");
    }

    string formName = "EditDocument"; // Use the actual action name
    var form = await context.AppFormDetails
        .Where(f => f.FormName == formName)
        .Select(f => f.Id)
        .FirstOrDefaultAsync();

    if (form == default)
    {
        return RedirectToAction("AccessDenied");
    }

    bool canModify = await context.AppUserFormPermissions
        .Where(p => p.UserId == userId && p.FormId == form)
        .AnyAsync(p => p.AllowModify == true);

    bool canDelete = await context.AppUserFormPermissions
        .Where(p => p.UserId == userId && p.FormId == form)
        .AnyAsync(p => p.AllowDelete == true);

    ViewBag.CanModify = canModify;
    ViewBag.CanDelete = canDelete;

    var technicalService = await context.AppTechnicalServices.FindAsync(id);
    if (technicalService == null)
    {
        return NotFound();
    }

    return View(technicalService);
}
 
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

<script>
    document.addEventListener("DOMContentLoaded", function () {
        var message = "@TempData["Message"]";
        if (message) {
            Swal.fire({
                title: message,
                icon: "success",
                timer: 3000
            });
        }
    });

    function setAction(actionValue) {
        document.getElementById('actionField').value = actionValue;
    }
</script>
 
 
 
 protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
 {
     var httpContext = _httpContextAccessor.HttpContext;

     if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
     {
         return;
     }

     var userIdString = httpContext.Session.GetString("ID"); 
     if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
     {
         return;
     }

   
     string? formName = httpContext.GetRouteData()?.Values["action"]?.ToString();

     if (string.IsNullOrEmpty(formName))
     {
         return;
     }

   
     var form = await _context.AppFormDetails
         .Where(f => f.FormName == formName)
         .Select(f => new { f.Id })
         .FirstOrDefaultAsync();

     if (form == null)
     {
         return;
     }

     Guid formId = form.Id;

    
     var hasPermission = await _context.AppUserFormPermissions
         .Where(p => p.UserId == userId && p.FormId == formId)
         .AnyAsync(p =>
             (requirement.Permission == "AllowWrite" && p.AllowWrite == true) ||
             (requirement.Permission == "AllowRead" && p.AllowRead == true) ||
             (requirement.Permission == "AllowDelete" && p.AllowDelete == true) ||
             (requirement.Permission == "AllowModify" && p.AllowModify == true)
         );

     if (hasPermission)
     {
         context.Succeed(requirement);
     }
 }
