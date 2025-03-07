var hiddenField1 = document.getElementById('<%= HiddenChartData1.ClientID %>').value;
var chartData1 = hiddenField1.split(',').map(Number); // Convert string to array of numbers
var labels = ['1 Pending Day', '2 Pending Day', '3 Pending Day', 'More Than 3 pending Day'];
var filteredData = [];
var filteredLabels = [];
var filteredColors = [];
var colors = ['#f9b037', '#5a7bf9', '#ed7b8e', '#76c893'];

for (var i = 0; i < chartData1.length; i++) {
    if (chartData1[i] !== 0) {
        filteredData.push(chartData1[i]);
        filteredLabels.push(labels[i]);
        filteredColors.push(colors[i]);
    }
}

var pieCtx1 = document.getElementById('pieChart1').getContext('2d');
var pieChart1 = new Chart(pieCtx1, {
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
        maintainAspectRatio: false,
        layout: {
            padding: 20
        },
        plugins: {
            legend: {
                display: true,
                position: 'bottom',
                labels: {
                    boxWidth: 12,
                    padding: 9
                }
            },
            tooltip: {
                callbacks: {
                    label: function (tooltipItem) {
                        let dataset = tooltipItem.dataset.data;
                        let value = dataset[tooltipItem.dataIndex];
                        return value; // Show number on hover
                    }
                }
            },
            datalabels: {
                formatter: (value, ctx) => {
                    let sum = ctx.dataset.data.reduce((a, b) => a + b, 0);
                    let percentage = ((value / sum) * 100).toFixed(1) + "%";
                    return percentage; // Show percentage inside the pie chart
                },
                color: '#000',
                font: {
                    weight: 'bold'
                }
            }
        }
    },
    plugins: [ChartDataLabels] // Make sure to include ChartDataLabels
});

<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels"></script>



var hiddenField1 = document.getElementById('<%= HiddenChartData1.ClientID %>').value;
        var chartData1 = hiddenField1.split(',').map(Number); // Convert string to array of numbers
        var labels = ['1 Pending Day', '2 Pending Day', '3 Pending Day', 'More Than 3 pending Day'];
        var filteredData = [];
        var filteredLabels = [];
        var filteredColors = [];
        var colors = ['#f9b037', '#5a7bf9', '#ed7b8e', '#76c893'];

        for (var i = 0; i < chartData1.length; i++) {
            if (chartData1[i] !== 0) {
                filteredData.push(chartData1[i]);
                filteredLabels.push(labels[i]);
                filteredColors.push(colors[i]);
            }
        }

        var pieCtx1 = document.getElementById('pieChart1').getContext('2d');
        var pieChart1 = new Chart(pieCtx1, {
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

  

i want percentage in pie chart but when cursor goes on label it shows numbers 
