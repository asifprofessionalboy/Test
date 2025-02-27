using (var connection = context1.Database.GetDbConnection()) // Use context1 for DB connection
{
    string query = @"
        SELECT 
            e.Pno, 
            e.Id, 
            e.Ename, 
            l.UserId 
        FROM AppEmployeeMasters e
        LEFT JOIN AppLogin l ON e.Id = l.Id";

    var PnoEnameList = connection.Query(query).ToList();
    ViewBag.PnoEnameList = PnoEnameList;
}




var pnoEnameList = @Html.Raw(JsonConvert.SerializeObject(ViewBag.PnoEnameList));

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("Pno").addEventListener("input", function () {
        var pno = this.value;
        var user = pnoEnameList.find(u => u.Pno === pno);

        if (user) {
            document.getElementById("Name").value = user.Ename;
            document.getElementById("UserId").value = user.UserId; // Set UserId from AppLogin
            
            document.getElementById("formContainer").style.display = "block";
        } else {
            document.getElementById("Name").value = "";
            document.getElementById("UserId").value = "";
            document.getElementById("formContainer").style.display = "none";

            document.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                checkbox.checked = false;
            });
        }
    });
});



var PnoEnameList = (from emp in context1.AppEmployeeMasters
                    join login in context1.AppLogins on emp.Id equals login.Id
                    select new
                    {
                        Pno = emp.Pno,
                        Id = emp.Id,
                        Ename = emp.Ename,
                        UserId = login.UserId // Fetching UserId from AppLogin
                    }).ToList();

ViewBag.PnoEnameList = PnoEnameList;




i have this js 

  var pnoEnameList = @Html.Raw(JsonConvert.SerializeObject(ViewBag.PnoEnameList));

  document.addEventListener("DOMContentLoaded", function () {
      document.getElementById("Pno").addEventListener("input", function () {
          var pno = this.value;
          var user = pnoEnameList.find(u => u.Pno === pno);

          if (user) {
              document.getElementById("Name").value = user.Ename;
              
              document.getElementById("formContainer").style.display = "block";
              document.getElementById("UserId").value = user.Id;
          } else {
              document.getElementById("Name").value = "";
              document.getElementById("UserId").value = "";
              document.getElementById("formContainer").style.display = "none";

              
              document.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                  checkbox.checked = false;
              });
          }
      });
  });

this is my controller logic 

 var PnoEnameList = context1.AppEmployeeMasters
        .Select(x => new
        {
            Pno = x.Pno,
            Id = x.Id,
            Ename = x.Ename,
        })
        .ToList();
 ViewBag.PnoEnameList = PnoEnameList;


and this is my form  <div class="col-md-2 mb-1">
     <input type="number" id="Pno" class="form-control form-control-sm" placeholder="Max 6 digits" oninput="javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);" maxlength="6" autocomplete="off">
 </div>
 <div class="col-md-1 mb-1">
     <label class="control-label">User Name:</label>
 </div>
 <div class="col-md-3 mb-1">
     <input type="text" readonly id="Name" class="form-control form-control-sm">
 </div>  



in this i want a logic that when user input the pno in id pno it fetches the loginId from App_login place in hidden field

this is my login model and my hidden field

 public partial class AppLogin
 {
     public Guid Id { get; set; }
     public string UserId { get; set; } = null!;
     public string Password { get; set; } = null!;
}
 <input type="hidden" id="UserId" name="UserId" />
