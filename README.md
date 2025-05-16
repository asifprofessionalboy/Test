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
