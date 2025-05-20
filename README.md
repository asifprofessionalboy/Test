 <a asp-action="CorrectionOfAttendance" style="text-decoration:none;background-color:#ffffff;font-weight:bolder;color:darkblue;" asp-route-id="@item.Id" class="control-label refNoLink"
    data-id="@item.Id" data-Pno="@item.Pno" data-Cdate="@item.Cdate" data-Intime="@item.Intime" data-OutTime="@item.OutTime"  data-Remarks="@item.Remarks" data-Reason="@item.Reason"
 data-ApproverPno="@item.ApproverPno" data-ApproversRemarks="@item.ApproversRemarks" data-Approver="@item.Approver" data-Approver="@item.Hodremarks">
     @item.Pno
 </a>


 refNoLinks.forEach(link => {
     link.addEventListener("click", function (event) {
         event.preventDefault();
         cOAForm.style.display = "block";

         const pno = this.getAttribute("data-Pno");
 document.getElementById("Pno").value = pno;
         document.getElementById("Cdate").value = this.getAttribute("data-Cdate");
         document.getElementById("Reason").value = this.getAttribute("data-Reason");
         document.getElementById("Remarks").value = this.getAttribute("data-Remarks");
         document.getElementById("ApproverPno").value = this.getAttribute("data-ApproverPno");
         document.getElementById("Approver").value = this.getAttribute("data-Approver");
         document.getElementById("Intime").value = this.getAttribute("data-Intime");
         document.getElementById("OutTime").value = this.getAttribute("data-OutTime");
         document.getElementById("COAId").value = this.getAttribute("data-id"); // Set ID


         fetch(`/COA/GetEmployeeName?pno=${pno}`)
             .then(response => {
                 if (!response.ok) throw new Error("Name not found");
                 return response.json();
             })
             .then(data => {
                 document.getElementById("Name").value = data.name;
             })
             .catch(error => {
                 console.error(error);
                 document.getElementById("Name").value = "Not Found";
             });
     



        
 const intime = this.getAttribute("data-Intime");
 if (intime && intime.includes(":")) {
     const [hh, mm] = intime.split(":");
     document.getElementById("IntimeHH").value = hh;
     document.getElementById("IntimeMM").value = mm;
 }


 const outtime = this.getAttribute("data-OutTime");
 if (outtime && outtime.includes(":")) {
     const [hh, mm] = outtime.split(":");
     document.getElementById("OutTimeHH").value = hh;
     document.getElementById("OutTimeMM").value = mm;
 }

     });
 });

  [HttpGet("GetEmployeeName")]
  public IActionResult GetEmployeeName(string pno)
  {
      using (var connection = new SqlConnection("Server=10.0.168.50;Database=UserLoginDB;User Id=fs;Password=p@ssW0Rd321;TrustServerCertificate=ye"))
      {
          string query = "SELECT EName FROM UserloginDB.dbo.App_EmployeeMaster WHERE Pno = @Pno";
          var command = new SqlCommand(query, connection);
          command.Parameters.AddWithValue("@Pno", pno);

          connection.Open();
          var result = command.ExecuteScalar();
          connection.Close();

          if (result != null)
              return Ok(new { name = result.ToString() });
          else
              return NotFound();
      }
  }

it shows NotFound
