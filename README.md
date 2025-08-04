function checkCheckboxesFromDropdownText() {
    const dropdownText = document.getElementById("DeptDropdown").value;

    // Match strings wrapped in quotes: "Row,Admin & Compliances", etc.
    const selectedNames = Array.from(dropdownText.matchAll(/"([^"]+)"/g), m => m[1].trim());

    // Uncheck all checkboxes
    document.querySelectorAll(".Dept-checkbox").forEach(cb => {
        cb.checked = false;
    });

    // Match each checkbox label
    selectedNames.forEach(name => {
        const lowerName = name.toLowerCase();
        document.querySelectorAll(".Dept-checkbox").forEach(cb => {
            const label = document.querySelector(`label[for="${cb.id}"]`);
            if (label && label.textContent.trim().toLowerCase() === lowerName) {
                cb.checked = true;
            }
        });
    });

    updateHiddenFieldFromCheckboxes(); // your existing function
}
