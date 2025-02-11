this is my chart.js 

<script>
    let myChart;
    function updateChart(){
        
        const finyear = document.getElementById("FinYear4").value;
       //const baseUrl = window.location.origin +'/Log_Innovation';
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

                const labels = data.map(item => item.division);
                const colors = ['#6b5b95', '#b2ad7f', '#feb236', '#b1cbbb', '#86af49', '#b9936c', '#3e4444', '#034f84', '#c94c4c'];
                const counts = data.map(item => item.count);

                
                const canvas = document.getElementById('barChart4');
                const ctx4 = canvas.getContext('2d');

                if(myChart){
                    myChart.destroy();
                    myChart = null;

                    canvas.height = 370;
                    canvas.width = 800;
                }

               myChart= new Chart(ctx4, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Division wise Participation',
                            data: counts,
                            backgroundColor: colors,
                            borderColor: colors.map(color => color.replace('0.2', '1')),
                            borderWidth: 0.5
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            datalabels: {
                                anchor: 'end',
                                align: 'top',
                                color: '#000',
                                font: {
                                    weight: 'bold',
                                    size: 10
                                },
                                formatter: value => value
                            },
                            tooltip: {
                                callbacks: {
                                    label: context => `${context.raw}`
                                }
                            },
                            legend: {
                                display: false
                            }
                        },
                        scales: {
                            x: {

                                ticks: {
                                    callback: function (value, index, ticks) {

                                        const label = this.getLabelForValue(value);


                                        return label.split(' ');

                                    },
                                    autoSkip: false,
                                    maxRotation: 0,
                                    minRotation: 0,
                                    font: {
                                        size: 11

                                    },

                                    display: true
                                },
                                grid: {
                                    display: false
                                },
                            },
                            y: {
                                grid: {
                                    display: false
                                },
                                display: true,
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'No. Of Projects',
                                    font: {
                                        size: 11,
                                        family: 'Arial',
                                        weight: 'bold',
                                        color: '#767676'
                                    }
                                },
                                ticks: {
                                    display: false,
                                    stepSize: 1
                                }
                            }
                        },
                        layout: {
                            padding: {
                                bottom: 20,
                                top: 50
                            }
                        },
                        barPercentage: 0.2,
                    },
                    plugins: [ChartDataLabels]
                });
            })
            .catch(error => console.error('Error:', error));
    }
    document.addEventListener("DOMContentLoaded", updateChart);

</script>

in this js one division name is corporate services , i want that division label should look People Function when the label value is Corporate services 
