function checkCheckboxesFromDropdownText() {
    const dropdownText = document.getElementById("DeptDropdown").value;

    // Split the dropdown text into an array of names
    const selectedNames = dropdownText
        .split(",")
        .map(s => s.trim())
        .filter(s => s); // Remove empty strings

    // Uncheck all checkboxes first
    document.querySelectorAll(".Dept-checkbox").forEach(cb => {
        cb.checked = false;
    });

    // Check boxes whose label matches any name from dropdown
    selectedNames.forEach(name => {
        const lowerName = name.toLowerCase();

        document.querySelectorAll(".Dept-checkbox").forEach(cb => {
            const label = document.querySelector(`label[for="${cb.id}"]`);
            if (label) {
                const labelText = label.textContent.trim().toLowerCase();
                if (labelText === lowerName) {
                    cb.checked = true;
                }
            }
        });
    });

    // Call function to update hidden field (assumes this function exists)
    updateHiddenFieldFromCheckboxes();
}
