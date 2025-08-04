function checkCheckboxesFromDropdownText() {
    const dropdownText = document.getElementById("DeptDropdown").value;

    // Split using semicolon instead of comma
    const selectedNames = dropdownText
        .split(";")
        .map(s => s.trim())
        .filter(s => s);

    // Uncheck all checkboxes
    document.querySelectorAll(".Dept-checkbox").forEach(cb => cb.checked = false);

    // Check matching checkboxes
    selectedNames.forEach(name => {
        const lowerName = name.toLowerCase();

        document.querySelectorAll(".Dept-checkbox").forEach(cb => {
            const label = document.querySelector(`label[for="${cb.id}"]`);
            if (label && label.textContent.trim().toLowerCase() === lowerName) {
                cb.checked = true;
            }
        });
    });

    updateHiddenFieldFromCheckboxes();
}
