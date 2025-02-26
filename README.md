var pnoEnameList = @Html.Raw(JsonConvert.SerializeObject(ViewBag.PnoEnameList));

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("Pno").addEventListener("input", function () {
        var pno = this.value;
        var user = pnoEnameList.find(u => u.Pno === pno);

        if (user) {
            document.getElementById("Name").value = user.Ename;
            document.getElementById("formContainer").style.display = "block";
        } else {
            document.getElementById("Name").value = "";
            document.getElementById("formContainer").style.display = "none";

            // Uncheck all checkboxes
            document.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                checkbox.checked = false;
            });
        }
    });
});

 
 
 
 
 @if (ViewBag.formList != null)
 {
     var formList = ViewBag.formList as List<AppFormDetail>;
     int rowIndex = 0;

     @foreach (var form in formList)
     {
         string bgColor = (rowIndex % 2 == 1 && rowIndex != 0) ? "#e3dff3" : "transparent";
         <tr style="color:#333333; background-color:@bgColor; font-size:Smaller;">
             <td>
                 <span>@form.Description</span>
             </td>
             <td style="width:100px;">
                 <input type="checkbox" id="Read"><label class="control-label">&nbsp;Read</label>
             </td>
             <td style="width:100px;">
                 <input type="checkbox" id="Create"><label class="control-label">&nbsp;Create</label>
             </td>
             <td style="width:100px;">
                 <input type="checkbox" id="Modify"><label class="control-label">&nbsp;Modify</label>
             </td>
             <td style="width:100px;">
                 <input type="checkbox" id="Delete"><label class="control-label" >&nbsp;Delete</label>
             </td>
             <td style="width:100px;">
                 <input type="checkbox" id="All"><label class="control-label">&nbsp;All</label>
             </td>
         </tr>
         rowIndex++;
     }
 }
 
this is my js 
  var pnoEnameList = @Html.Raw(JsonConvert.SerializeObject(ViewBag.PnoEnameList));
  document.addEventListener("DOMContentLoaded", function () {
      document.getElementById("Pno").addEventListener("input", function () {
          var pno = this.value;


          var user = pnoEnameList.find(u => u.Pno === pno);

          if (user) {
              document.getElementById("Name").value = user.Ename;
              document.getElementById("formContainer").style.display = "block";

          } else {
              document.getElementById("Name").value = "";
              document.getElementById("Read").checked = false;
              document.getElementById("Create").checked = false;
              document.getElementById("Modify").checked = false;
              document.getElementById("Delete").checked = false
              document.getElementById("All").checked = false;
              document.getElementById("formContainer").style.display = "none";

          }


      });
  });

in this i want to uncheck the checkboxes when goes on else part 
