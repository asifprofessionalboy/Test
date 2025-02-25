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
