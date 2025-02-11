const finyear = document.getElementById("FinYear4").value;
const baseUrl = window.location.origin +'/Log_Innovation';
const baseUrl = window.location.origin;
const url = `${baseUrl}/Innovation/GetDivisionCount?FinYear4=${finyear}`;

       
fetch(url)
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        console.log(data);


        make the base url dynamic , because when i locally run the file this base url working const baseUrl = window.location.origin;
        when i upload on IS Server it work for this base url const baseUrl = window.location.origin +'/Log_Innovation'; i want to use dynamic  
