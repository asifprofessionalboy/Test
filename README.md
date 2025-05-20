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





COA_Approver?page=1&searchString=:448 Error: Name not found
