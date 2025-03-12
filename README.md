document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("Pno").addEventListener("input", function () {
        var pno = this.value;
        var user = pnoEnameList.find(u => u.UserId === pno);

        if (user) {
            document.getElementById("Name").value = user.Name;
            document.getElementById("UserId").value = user.Id;
            document.getElementById("formContainer").style.display = "block";

            console.log(userPermissions);

            // Fetch existing permissions for this user
            var userPermissionsList = userPermissions.filter(p => p.UserId === user.Id);
            console.log(userPermissionsList);

            // Reset all checkboxes
            document.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                checkbox.checked = false;
            });

            // Loop through user permissions and check the appropriate checkboxes
            userPermissionsList.forEach(permission => {
                let row = document.querySelector(`input[name="FormPermissions[][FormId]"][value="${permission.FormId}"]`)?.closest("tr");

                if (row) {
                    if (permission.AllowRead) row.querySelector('input[name*="AllowRead"]').checked = true;
                    if (permission.AllowWrite) row.querySelector('input[name*="AllowWrite"]').checked = true;
                    if (permission.AllowModify) row.querySelector('input[name*="AllowModify"]').checked = true;
                    if (permission.AllowDelete) row.querySelector('input[name*="AllowDelete"]').checked = true;
                    if (permission.AllowAll) row.querySelector('input[name*="AllowAll"]').checked = true;
                }
            });

        } else {
            // If user is not found, reset everything
            document.getElementById("Name").value = "";
            document.getElementById("UserId").value = "";
            document.getElementById("formContainer").style.display = "none";

            document.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                checkbox.checked = false;
            });
        }
    });
});




this is for my existing data  
 var userPermissions = context.AppUserFormPermissions.ToList();
 ViewBag.UserPermissions = userPermissions;

and this is my checkbox 
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

  var pnoEnameList = @Html.Raw(JsonConvert.SerializeObject(ViewBag.PnoEnameList));
  var userPermissions = @Html.Raw(JsonConvert.SerializeObject(ViewBag.UserPermissions));

  document.addEventListener("DOMContentLoaded", function () {
      document.getElementById("Pno").addEventListener("input", function () {
          var pno = this.value;
          var user = pnoEnameList.find(u => u.UserId === pno);
         
          if (user) {
              document.getElementById("Name").value = user.Name;
              document.getElementById("UserId").value = user.Id;
              document.getElementById("formContainer").style.display = "block";
                      console.log(userPermissions);
              // Fetch existing permissions for this user
              var userPermissionsList = userPermissions.filter(p => p.UserId === user.Id);
                  console.log(userPermissionsList);
              // Reset checkboxes
              document.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                  checkbox.checked = false;
              });

              // Loop through the permissions and check the corresponding checkboxes
              userPermissionsList.forEach(permission => {
                  document.querySelector(`input[name="FormPermissions[][FormId][value='${permission.FormId}']"]`)
                      ?.closest("tr")?.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                          if (permission.AllowRead && checkbox.name.includes("AllowRead")) checkbox.checked = true;
                          if (permission.AllowWrite && checkbox.name.includes("AllowWrite")) checkbox.checked = true;
                          if (permission.AllowModify && checkbox.name.includes("AllowModify")) checkbox.checked = true;
                          if (permission.AllowDelete && checkbox.name.includes("AllowDelete")) checkbox.checked = true;
                          if (permission.AllowAll && checkbox.name.includes("AllowAll")) checkbox.checked = true;
                      });
              });

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

this type of data is coming in console 

0
: 
{Id: '028bf0ff-16d5-43f4-9199-23a9ac1af236', UserId: '15fae783-d72d-468b-b75c-e388dc0f9b7e', FormId: 'd73cefbf-65a2-4e7b-a052-f950b7962a2d', AllowRead: true, AllowWrite: false, …}
1
: 
{Id: '812078da-4ecb-4144-8004-24257da43778', UserId: '15fae783-d72d-468b-b75c-e388dc0f9b7e', FormId: '5f323068-8bff-49c5-9795-2ceecd0bb138', AllowRead: true, AllowWrite: false, …}
2
: 
{Id: '5c8ac2b5-3385-4062-a653-785daccfd498', UserId: '15fae783-d72d-468b-b75c-e388dc0f9b7e', FormId: 'd48679a0-f8ba-4658-bb9e-c564e95da013', AllowRead: true, AllowWrite: true, …}
3
: 
{Id: 'a0336d8c-5950-426f-b7d8-a63ab6bfce17', UserId: '15fae783-d72d-468b-b75c-e388dc0f9b7e', FormId: 'c40d7ed7-84bc-4f3a-b711-af1a34ef83dc', AllowRead: true, AllowWrite: false, …}
4
: 
{Id: '603d59d5-46ff-4235-9dbe-c47039f5e18b', UserId: '15fae783-d72d-468b-b75c-e388dc0f9b7e', FormId: '2cd119c4-6deb-42a0-b6e4-1570f22d3aa6', AllowRead: true, AllowWrite: false, …}

i want that if i put userId then it matches with UserId and Id of the User and if matches and existing data then and if value is true then checkbox will be checked
