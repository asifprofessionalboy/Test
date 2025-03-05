<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Load iFrame on Button Click</title>
</head>
<body>

    <button id="loadIframe">Load iFrame</button>
    
    <iframe id="myIframe" width="600" height="400" style="display:none; border: 1px solid #000;"></iframe>

    <script>
        document.getElementById("loadIframe").addEventListener("click", function() {
            var iframe = document.getElementById("myIframe");
            iframe.src = "https://www.example.com"; // Replace with your URL
            iframe.style.display = "block"; // Show iframe after loading
        });
    </script>

</body>
</html>




const pathname = window.location.pathname.toLowerCase();

let baseUrl = window.location.origin;
if (pathname.includes('/log_innovation')) {
    baseUrl += '/Log_Innovation';
} else if (pathname.includes('/innovation')) {
    baseUrl += '/Innovation';
}

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
    })
    .catch(error => {
        console.error('Error:', error);
    });




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
