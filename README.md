[HttpPost]
public async Task<IActionResult> DeleteSubject(Guid id)
{
    var record = await context.AppSubjectMasters.FindAsync(id);
    if (record == null)
    {
        return Json(new { success = false, message = "Subject not found!" });
    }

    context.AppSubjectMasters.Remove(record);
    await context.SaveChangesAsync();

    return Json(new { success = true, message = "Subject deleted successfully!" });
}

document.getElementById("deleteButton").addEventListener("click", function () {
    var subjectId = document.getElementById("SubjectId").value;

    if (subjectId) {
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/Master/DeleteSubject/${subjectId}`, {
                    method: "POST"
                }).then(response => {
                    if (response.ok) {
                        Swal.fire({
                            title: "Deleted!",
                            text: "Subject deleted successfully!",
                            icon: "success",
                            timer: 3000
                        }).then(() => {
                            location.reload();
                        });
                    } else {
                        Swal.fire({
                            title: "Error",
                            text: "Error deleting subject.",
                            icon: "error"
                        });
                    }
                }).catch(error => {
                    Swal.fire({
                        title: "Error",
                        text: "Something went wrong!",
                        icon: "error"
                    });
                });
            }
        });
    }
});



deleteButton.addEventListener("click", function () {
    var subjectId = document.getElementById("SubjectId").value;
    if (subjectId) {
        if (confirm("Are you sure you want to delete this Subject?")) {
            fetch(`/Master/DeleteSubject/${subjectId}`, {
                method: "POST"
            }).then(response => {
                if (response.ok) {
                    alert("Transaction deleted successfully!");
                    location.reload();
                } else {
                    alert("Error deleting transaction.");
                }
            });
        }
    }
});


 [HttpPost]
 public async Task<IActionResult> DeleteSubject(Guid id)
 {
     var record = await context.AppSubjectMasters.FindAsync(id);
     if (record == null)
     {
         return RedirectToAction("SubjectMaster");
     }

     context.AppSubjectMasters.Remove(record);
     await context.SaveChangesAsync();

     return View();
 }

in this i want the same logic 
