this is my model where data is stored  
public partial class AppUserFormPermissionViewModel
 {
     public Guid UserId { get; set; }

     public List<AppUserFormPermission> FormPermissions { get; set; }
 }

 public partial class AppUserFormPermission
 {
     public Guid Id { get; set; }
     public Guid UserId { get; set; }
     public Guid FormId { get; set; }
     public bool AllowRead { get; set; }
     public bool AllowWrite { get; set; }
     public bool? AllowDelete { get; set; }
     public bool? AllowAll { get; set; }
     public bool? AllowModify { get; set; }
     public bool DownTime { get; set; }
 }

this is my view Method 
  public IActionResult UserPermission()
  {

      string connectionString = GetConnectionString();

      using (var connection = new SqlConnection(connectionString))
      {
          string query = @"
  SELECT 
      e.Pno, 
      e.Ename, 
      l.Id 
  FROM userLoginDB.dbo.App_EmployeeMaster e
  INNER JOIN App_Login l ON e.Pno COLLATE DATABASE_DEFAULT = l.UserId COLLATE DATABASE_DEFAULT";

          var PnoEnameList = connection.Query(query).ToList();
          ViewBag.PnoEnameList = PnoEnameList;
      }


      var formList = context.AppFormDetails.Select(x =>new AppFormDetail
      {
          Id = x.Id,
          Description = x.Description
      }).OrderBy(x=>x.Description).ToList();
      ViewBag.formList = formList;


      return View();
  }

and this is my view cshtml

<div class="form-inline row">
    <div class="col-md-1 mb-1">
        <label class="control-label">Select User:</label>
    </div>
    <div class="col-md-2 mb-1">
        <input type="number" id="Pno" class="form-control form-control-sm" placeholder="Max 6 digits" oninput="javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);" maxlength="6" autocomplete="off">
    </div>
    <div class="col-md-1 mb-1">
        <label class="control-label">User Name:</label>
    </div>
    <div class="col-md-3 mb-1">
        <input type="text" readonly id="Name" class="form-control form-control-sm">
    </div>
</div>

 <form asp-action="UserPermission_Button" asp-controller="User" method="post">
     <fieldset class="mt-2" style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px">
         <legend class="legend"><b>Select Permission for the User</b></legend>
         <div class="form" id="formContainer" style="display:none;">
             <div class="w-100 border" style="overflow:auto;height:250px;">
                 <table class="table-hover table-responsive-sm" cellspacing="0" cellpadding="4" id="MainContent_userPermissions" style="color:#333333;width:100%;border-collapse:collapse;">
                     <tbody>
                         <tr style="color:White;background-color:#49477a;font-size:Smaller;font-weight:bold;">
                             <th align="left" scope="col">Form Name</th>
                             <th scope="col">&nbsp;</th>
                             <th scope="col">&nbsp;</th>
                             <th scope="col">&nbsp;</th>
                             <th scope="col">&nbsp;</th>
                             <th scope="col">&nbsp;</th>
                         </tr>

                         
                         @if (ViewBag.formList != null)
                         {
                             var formList = ViewBag.formList as List<AppFormDetail>;
                             int rowIndex = 0;

                             @foreach (var form in formList)
                             {
                                 string bgColor = (rowIndex % 2 == 1 && rowIndex != 0) ? "#e3dff3" : "transparent";
                                 <tr style="color:#333333; background-color:@bgColor; font-size:Smaller;">
                                     <td style="width:50%;">
                                         <input type="hidden" name="FormPermissions[@rowIndex].FormId" value="@form.Id" />
                                         <span>@form.Description</span>
                                     </td>

                                     <td style="width:100px;">
                                         <input type="checkbox" name="FormPermissions[@rowIndex].AllowRead" value="true">
                                         <label class="control-label">&nbsp;Read</label>
                                     </td>
                                     <td style="width:100px;">
                                         <input type="checkbox" name="FormPermissions[@rowIndex].AllowWrite" value="true">
                                         <label class="control-label">&nbsp;Create</label>
                                     </td>
                                     <td style="width:100px;">
                                         <input type="checkbox" name="FormPermissions[@rowIndex].AllowModify" value="true">
                                         <label class="control-label">&nbsp;Modify</label>
                                     </td>
                                     <td style="width:100px;">
                                         <input type="checkbox" name="FormPermissions[@rowIndex].AllowDelete" value="true">
                                         <label class="control-label">&nbsp;Delete</label>
                                     </td>
                                     <td style="width:100px;">
                                         <input type="checkbox" name="FormPermissions[@rowIndex].AllowAll" value="true">
                                         <label class="control-label">&nbsp;All</label>
                                     </td>
                                 </tr>
                                 rowIndex++;
                             }
                         }
                     </tbody>
                 </table>
             </div>

             <div class="row m-0 justify-content-center mt-2">
                 <input type="hidden" id="UserId" name="UserId" />
                 <input type="submit" value="Save" id="MainContent_btnSave" class="btn btn-primary btn-sm">
             </div>
         </div>
     </fieldset>
 </form>
and this is my js 


<script>
    var pnoEnameList = @Html.Raw(JsonConvert.SerializeObject(ViewBag.PnoEnameList));

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("Pno").addEventListener("input", function () {
        var pno = this.value;
        var user = pnoEnameList.find(u => u.Pno === pno);

        if (user) {
            document.getElementById("Name").value = user.Ename;
            document.getElementById("UserId").value = user.Id; // Set UserId from AppLogin
            
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


</script>


i want that when user input their Pno it catches the checkbox value which is selected or not if there is existing data for that user then show checkbox checked
