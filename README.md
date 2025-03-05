i have these 3 base url for project , i want to make this dynamic 

const baseUrl = window.location.origin +'/Log_Innovation';
const baseUrl = window.location.origin +'/Innovation';
const baseUrl = window.location.origin;
const url = `${baseUrl}/Innovation/GetDivisionCount?FinYear4=${finyear}`;

       
fetch(url)
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
    })
