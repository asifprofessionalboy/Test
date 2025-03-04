Chart.register(ChartDataLabels);
document.addEventListener("DOMContentLoaded", function () {
    var hiddenField1 = document.getElementById('<%= HiddenChartData1.ClientID %>').value;
    var chartData1 = hiddenField1.split(',').map(Number); // Convert string to array of numbers

    var labels = ['1 Pending Day', '2 Pending Day', '3 Pending Day', 'More Than 3 pending Day'];

    // Filter out zero values along with their labels
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
});





this is my chart.js 
    Chart.register(ChartDataLabels);
    document.addEventListener("DOMContentLoaded", function ()

    {
        var hiddenField1 = document.getElementById('<%= HiddenChartData1.ClientID %>').value;
        var chartData1 = hiddenField1.split(',').map(Number); // Convert string to array of numbers

        var pieCtx1 = document.getElementById('pieChart1').getContext('2d');
        var pieChart1 = new Chart(pieCtx1, {
            type: 'pie',
            data: {
                labels: ['1 Pending Day', '2 Pending Day', '3 Pending Day', 'More Than 3 pending Day'],

                datasets: [{
                    data: chartData1,
                    backgroundColor: ['#f9b037', '#5a7bf9', '#ed7b8e', '#76c893'],
                    borderColor: ['#f9b037', '#5a7bf9', '#ed7b8e', '#76c893'],
                    borderWidth: 1
                }]
            },
            options:
            {
                responsive: true,
                plugins: {

                    legend: {
                        display: true,
                        position: 'top',
                        align:'start',
                        labels: {
                            boxWidth: 12,
                            padding: 9,
                            //usePointStyle: true,
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

in this there is three 4 value is coming 0,0,17,0 . i want that value is 0 then dont show on piechart
