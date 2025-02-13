document.addEventListener("DOMContentLoaded", function () {
    var newButton = document.getElementById("newButton"); // Ensure this button exists
    var subjectMaster = document.getElementById("SubjectMaster");
    var refNoLinks = document.querySelectorAll(".refNoLink");
    var deleteButton = document.getElementById("deleteButton");

    if (newButton) {
        newButton.addEventListener("click", function () {
            subjectMaster.style.display = "block";
            document.getElementById("Subject").value = "";
            document.getElementById("TransactionId").value = ""; // Reset ID
            deleteButton.style.display = "none";
        });
    }

    refNoLinks.forEach(link => {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            subjectMaster.style.display = "block";

            document.getElementById("Subject").value = this.getAttribute("data-subject");
            document.getElementById("TransactionId").value = this.getAttribute("data-id"); // Set ID

            deleteButton.style.display = "inline-block";
        });
    });

    deleteButton.addEventListener("click", function () {
        var transactionId = document.getElementById("TransactionId").value;
        if (transactionId) {
            if (confirm("Are you sure you want to delete this transaction?")) {
                fetch(`/Civil/DeleteTransaction/${transactionId}`, {
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
});


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

    var viewModel = new YourViewModel(); // Ensure ViewModel is initialized

    if (id.HasValue)
    {
        var selectedTransaction = await context.AppSubjectMasters.FirstOrDefaultAsync(a => a.Id == id);
        if (selectedTransaction != null)
        {
            viewModel.AppTransaction = selectedTransaction;
        }
    }

    return View(viewModel);
}




this is my table 
 <tbody style="">
     @if (ViewBag.ListData2 != null)
     {
         @foreach (var item in ViewBag.ListData2)
         {
             <tr>
                 <td>
                     <a asp-action="SubjectMaster" asp-route-id="@item.Id" class="text-primary refNoLink" style="text-decoration:none;background-color:yellow;font-weight:bolder;"
                        data-id="@item.Id" data-subject="@item.Subject">
                         @item.Subject
                     </a>
                 </td>

                 <td>@item.Subject</td>
                 <td>@item.CreatedBy</td>
                 <td>@item.CreatedOn</td>
                
             </tr>
         }
     }
     else
     {
         <tr>
             <td colspan="4">No data available</td>
         </tr>
     }
 </tbody>

when i click on refNoLink it shows the existing data 
<div class="row" id="SubjectMaster" style="display:none;">
    <form asp-action="SubmitTransactions" asp-controller="Civil" method="post">
        <input type="hidden" asp-for="Id" id="TransactionId" name="Id" />

        <div class="row">
            
            <div class="col-md-6 mb-3">
                <label for="CustomerName">Customer Name</label>
                <input asp-for="Subject" class="form-control form-control-sm" id="Subject" type="text" readonly>
            </div>

           </div>

        <div class="mt-3">
            <button class="btn btn-primary" id="submitButton" type="submit">Submit</button>

            <button class="btn btn-danger" id="deleteButton" style="display: none;">Submit</button>

            

        </div>
    </form>


</div>

<script>

    document.addEventListener("DOMContentLoaded", function () {
        var newButton = document.getElementById("newButton");
        var subjectMaster = document.getElementById("SubjectMaster");
        var refNoLinks = document.querySelectorAll(".refNoLink");
        var deleteButton = document.getElementById("deleteButton");


        newButton.addEventListener("click", function () {
            subjectMaster.style.display = "block";


            document.getElementById("Subject").value = "";
           

            deleteButton.style.display = "none";
        });


        refNoLinks.forEach(link => {
            link.addEventListener("click", function (event) {
                event.preventDefault();
                subjectMaster.style.display = "block";

                document.getElementById("Subject").value = this.getAttribute("data-subject");

                deleteButton.style.display = "inline-block";
            });
        });


        deleteButton.addEventListener("click", function () {
            var transactionId = document.getElementById("TransactionId").value;
            if (transactionId) {
                if (confirm("Are you sure you want to delete this transaction?")) {
                    fetch(`/Civil/DeleteTransaction/${transactionId}`, {
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
    });



</script>

this is my controller logic
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

   
     if (id.HasValue)
     {
         var selectedTransaction = await context.AppSubjectMasters.FirstOrDefaultAsync(a => a.Id == id);
         if (selectedTransaction != null)
         {
             viewModel.AppTransaction = selectedTransaction;
         }
     }

     return View(viewModel);
 }

is there any issue in this code 
