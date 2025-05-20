
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
