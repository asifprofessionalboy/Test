document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("btnSearch").addEventListener("click", function () {
        var hiddenField3 = document.getElementById('<%= HiddenField3.ClientID %>').value; // Use correct ASP.NET syntax
        alert(hiddenField3);

        if (!hiddenField3) {
            console.log("No Data");
            return;
        }

        var dataPoints;
        try {
            dataPoints = JSON.parse(hiddenField3); // Ensure JSON parsing
        } catch (error) {
            console.error("Invalid JSON Data:", error);
            return;
        }

        const labels = dataPoints.map(dp => `${dp.month}/${dp.year}`);
        const l1Data = dataPoints.map(dp => dp.l1);
        const l2Data = dataPoints.map(dp => dp.l2);

        const ctx = document.getElementById('DualLineChart').getContext('2d');
        
        Chart.register(ChartDataLabels); // Register the plugin

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'L1 Data',
                        data: l1Data,
                        borderColor: 'blue',
                        backgroundColor: 'blue',
                        fill: false,
                        tension: 0.3
                    },
                    {
                        label: 'L2 Data',
                        data: l2Data,
                        borderColor: 'green',
                        backgroundColor: 'green',
                        fill: false,
                        tension: 0.3
                    },
                    {
                        label: 'Average (3)',
                        data: Array(labels.length).fill(3),
                        borderColor: 'red',
                        borderWidth: 1,
                        borderDash: [5, 5],
                        pointRadius: 0,
                        fill: false
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    datalabels: {
                        color: 'black',
                        anchor: 'end',
                        align: 'bottom',
                        font: {
                            weight: 'bold',
                            size: 10
                        },
                        formatter: (value) => value,
                        display: (context) => context.dataset.label !== 'Average (3)'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1 }
                    }
                }
            }
        });
    });
});

 
 
 document.addEventListener("DOMContentLoaded", (event) =>
        {
            document.getElementById("btnSearch").addEventListener("click", function ()
            {
            
                var hiddenField3 = document.getElementById('<%= HiddenField3.ClientID %>').value;
                alert(hiddenField3);
                if (!hiddenField3) {
                    console.log("No Data");
                    return;
                }
                /* console.log("Parsed Data:", dataPoints);*/
                var dataPoints = (hiddenField3);

                //for (var i = 0; i < chartData3.length; i++) {
                //    if (chartData3[i] !== 0) {
                //        filteredData.push(chartData3[i]);
                //        filteredLabels.push(labels[i]);
                //        filteredColors.push(colors[i]);
                //    }
                //}

                const labels = dataPoints.map(dp => `${dp.month}/${dp.year}`);
                const l1Data = dataPoints.map(dp => dp.l1);
                const l2Data = dataPoints.map(dp => dp.l2);

                const ctx = document.getElementById('DualLineChart').getContext('2d');
                new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [
                        {
                            label: 'L1 Data',
                            data: l1Data,
                            borderColor: 'blue',
                            backgroundColor: 'blue',
                            fill: false,
                            tension: 0.3
                        },
                        {
                            label: 'L2 Data',
                            data: l2Data,
                            borderColor: 'green',
                            backgroundColor: 'green',
                            fill: false,
                            tension: 0.3
                        },
                        {
                            label: 'Average (3)',
                            data: Array(labels.length).fill(3),
                            borderColor: 'red',
                            borderWidth: 1,
                            borderDash: [5, 5],
                            pointRadius: 0,
                            fill: false
                        }
                    ]
                },
                options:
                {
                    responsive: true,

                    plugins: {
                        datalabels: {
                            color: 'black',
                            anchor: 'end',
                            align: 'bottom',
                            font: {
                                weight: 'bold',
                                size: 10
                            },
                            formatter: (value, context) => {
                                return value; // Show data labels for L1 and L2 lines
                            },
                            display: (context) => {
                                // Only show data labels for L1 and L2 datasets, not for the average line
                                return context.dataset.label !== 'Average (3)';
                            }
                        }
                    },
                    scales:
                    {
                        y:
                        {
                            beginAtZero: true,
                            ticks: { stepSize: 1 }
                        }
                    }
                   },
                    plugins: [ChartDataLabels]
            });
                });


        });
