<!-- jQuery (required) -->
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

<!-- Bootstrap JS (optional but usually used) -->
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>

<!-- Bootstrap Datepicker JS -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>

<!-- Bootstrap Datepicker CSS -->
<link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" rel="stylesheet" />




<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>
<script>
    $('.datepicker').datepicker({
        format: 'dd-mm-yyyy',
        autoclose: true,
        todayHighlight: true
    });
</script>




!-- jQuery UI CSS & JS -->
<link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" />
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

<div class="form-group">
    <label for="DateOfBirth">Date of Birth</label>
    <input asp-for="DateOfBirth" class="form-control date-picker" autocomplete="off" />
    <span asp-validation-for="DateOfBirth" class="text-danger"></span>
</div>

<script>
    $(function () {
        $(".date-picker").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd-mm-yy",
            yearRange: "1900:2100"
        });
    });
</script>


refNoLinks.forEach(link => {
    link.addEventListener("click", function (event) {
        event.preventDefault();
        cOAForm.style.display = "block";

        document.getElementById("Pno").value = this.getAttribute("data-Pno");
        document.getElementById("Cdate").value = this.getAttribute("data-Cdate");
        document.getElementById("Reason").value = this.getAttribute("data-Reason");
        document.getElementById("Remarks").value = this.getAttribute("data-Remarks");
        document.getElementById("ApproverPno").value = this.getAttribute("data-ApproverPno");
        document.getElementById("Approver").value = this.getAttribute("data-Approver");
        document.getElementById("Intime").value = this.getAttribute("data-Intime");
        document.getElementById("OutTime").value = this.getAttribute("data-OutTime");
        document.getElementById("COAId").value = this.getAttribute("data-id");

        // ✅ Split and populate InTime HH & MM
        const intime = this.getAttribute("data-Intime");
        if (intime && intime.includes(":")) {
            const [hh, mm] = intime.split(":");
            document.getElementById("IntimeHH").value = hh;
            document.getElementById("IntimeMM").value = mm;
        }

        // ✅ Split and populate OutTime HH & MM
        const outtime = this.getAttribute("data-OutTime");
        if (outtime && outtime.includes(":")) {
            const [hh, mm] = outtime.split(":");
            document.getElementById("OutTimeHH").value = hh;
            document.getElementById("OutTimeMM").value = mm;
        }

        deleteButton.style.display = "inline-block";
    });
});





i have this js for fetching 

 refNoLinks.forEach(link => {
     link.addEventListener("click", function (event) {
         event.preventDefault();
         cOAForm.style.display = "block";

         document.getElementById("Pno").value = this.getAttribute("data-Pno");
         document.getElementById("Cdate").value = this.getAttribute("data-Cdate");
         document.getElementById("Reason").value = this.getAttribute("data-Reason");
         document.getElementById("Remarks").value = this.getAttribute("data-Remarks");
         document.getElementById("ApproverPno").value = this.getAttribute("data-ApproverPno");
         document.getElementById("Approver").value = this.getAttribute("data-Approver");
         document.getElementById("Intime").value = this.getAttribute("data-Intime");
         document.getElementById("OutTime").value = this.getAttribute("data-OutTime");
         document.getElementById("COAId").value = this.getAttribute("data-id"); // Set ID

         deleteButton.style.display = "inline-block";
     });
 });


in InTime and OutTime it shows the value in hidden now in this i want to split that only
