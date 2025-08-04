
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
