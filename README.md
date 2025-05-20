refNoLinks.forEach(link => {
    link.addEventListener("click", function (event) {
        event.preventDefault();
        cOAForm.style.display = "block";

        const pno = this.getAttribute("data-Pno");
        document.getElementById("Pno").value = pno;
        document.getElementById("Cdate").value = this.getAttribute("data-Cdate");
        document.getElementById("OutTime").value = this.getAttribute("data-OutTime");
        document.getElementById("COAId").value = this.getAttribute("data-id");

        // Fetch employee name
        fetch(`/YourController/GetEmployeeName?pno=${pno}`)
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
    });
});

[HttpGet("GetEmployeeName")]
public IActionResult GetEmployeeName(string pno)
{
    using (var connection = new SqlConnection("Your_Connection_String"))
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





 this is my query to fetch Name 
SELECT EName from UserloginDB.dbo.App_EmployeeMaster where Pno = '151514'

and this is my Name textbox
<div class="col-sm-2">
<input class="form-control form-control-sm" id="Name" value="" type="text" readonly>
</div>


and this is my link when click data is fetched 
refNoLinks.forEach(link => {
    link.addEventListener("click", function (event) {
        event.preventDefault();
        cOAForm.style.display = "block";

        document.getElementById("Pno").value = this.getAttribute("data-Pno");
        document.getElementById("Cdate").value = this.getAttribute("data-Cdate");
          document.getElementById("OutTime").value = this.getAttribute("data-OutTime");
        document.getElementById("COAId").value = this.getAttribute("data-id"); // Set ID
