I have two checkbox department that is saved in my table - Administration & Event Management and Row,Admin & Compliances   
but when this function execute only this Administration & Event Management department is coming not this Row,Admin & Compliances  

note - Row,Admin & Compliances  its one department with comma
 
 function checkCheckboxesFromDropdownText() {
        const dropdownText = document.getElementById("DeptDropdown").value;
        const selectedNames = dropdownText.split(",").map(s => s.trim()).filter(s => s);

       
        document.querySelectorAll(".Dept-checkbox").forEach(cb => cb.checked = false);





       
        selectedNames.forEach(name => {
            document.querySelectorAll(".Dept-checkbox").forEach(cb => {
                const label = document.querySelector(`label[for="${cb.id}"]`);
                if (label && label.textContent.trim() === name) {
                    cb.checked = true;
                }
            });
        });

        updateHiddenFieldFromCheckboxes(); 
    }
