i have this textbox 
<input type="hidden" asp-for="ApprovedYn" id="ApprovedYn" name="ApprovedYn" />

this is my js 

  refNoLinks.forEach(link => {
      link.addEventListener("click", function (event) {
          event.preventDefault();
          cOAForm.style.display = "block";

          const pno = this.getAttribute("data-Pno");
  document.getElementById("Pno").value = pno;
          document.getElementById("Cdate").value = this.getAttribute("data-Cdate");
          document.getElementById("Reason").value = this.getAttribute("data-Reason");
          document.getElementById("Remarks").value = this.getAttribute("data-Remarks");
          document.getElementById("ApprovedYn").value = this.getAttribute("data-ApprovedYn");
          document.getElementById("ApproverPno").value = this.getAttribute("data-ApproverPno");
          document.getElementById("ApproversRemarks").value = this.getAttribute("data-ApproversRemarks");
          document.getElementById("Approver").value = this.getAttribute("data-Approver");
          document.getElementById("Intime").value = this.getAttribute("data-Intime");
          document.getElementById("OutTime").value = this.getAttribute("data-OutTime");
          document.getElementById("COAId").value = this.getAttribute("data-id"); // Set ID


          fetch(`${window.location.origin}/COA/GetEmployeeName?pno=${encodeURIComponent(pno)}`)
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



in this i want that if value of ApprovedYn is true or false then hide the button if the ApprovedYn is null or blank then show the buttons
