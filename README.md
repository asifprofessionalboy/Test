var pieCtx2 = document.getElementById('pieChart2').getContext('2d');
var pieChart2 = new Chart(pieCtx2, {
    type: 'pie',
    data: {
        labels: filteredLabels,
        datasets: [{
            data: filteredData,
            backgroundColor: filteredColors,
            borderColor: filteredColors,
            borderWidth: 1
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false, // Allow chart to resize dynamically
        layout: {
            padding: 20 // Add padding for better spacing
        },
        plugins: {
            legend: {
                display: true,
                position: 'bottom', // Move legend below the chart
                labels: {
                    boxWidth: 12,
                    padding: 9
                }
            },
            datalabels: {
                anchor: 'end',
                align: 'start',
                color: '#000',
                font: {
                    weight: 'bold'
                }
            }
        }
    }
});

 
 
 
 
 var hiddenField2 = 
 document.getElementById('<%= HiddenChartData2.ClientID %>').value;
        var chartData2 = hiddenField2.split(',').map(Number); // Convert string to array of numbers

        var labels = ['1 Pending Day', '2 Pending Day', '3 Pending Day', 'More Than 3 pending Day'];
        var filteredData = [];
        var filteredLabels = [];
        var filteredColors = [];
        var colors = ['#f9b037', '#5a7bf9', '#ed7b8e', '#76c893'];

        for (var i = 0; i < chartData2.length; i++) {
            if (chartData2[i] !== 0) {
                filteredData.push(chartData2[i]);
                filteredLabels.push(labels[i]);
                filteredColors.push(colors[i]);
            }
        }

        var pieCtx2 = document.getElementById('pieChart2').getContext('2d');
        var pieChart2 = new Chart(pieCtx2, {
            type: 'pie',
            data: {
                labels: filteredLabels,
                datasets: [{
                    data: filteredData,
                    backgroundColor: filteredColors,
                    borderColor: filteredColors,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        align: 'start',
                        labels: {
                            boxWidth: 12,
                            padding: 9
                        }
                    },
                    datalabels: {
                        anchor: 'end',
                        align: 'start',
                        color: '#000',
                        font: {
                            weight: 'bold'
                        }
                    }
                }
            }
        });

i have two chartjs , let one has 3 legends and one has 2 legends , because of legend increasing chart size is small for that 3 , i want that it is responsive 
