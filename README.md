<div class="row">
    <div class="col-sm-12">
        <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;margin-top:20px;">
            <h6 class="text-center overview-heading">Face Recognition - Failed Attempts (Today)</h6>
            <canvas id="barChartFailedAttempts" style="width:800px;height:370px;"></canvas>
        </fieldset>
    </div>
</div>

<script>
    let failedAttemptChart;

    function loadFailedAttemptsChart() {
        fetch('/Dashboard/GetCount') // adjust controller if needed
            .then(response => {
                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
                return response.json();
            })
            .then(data => {
                // Sort ascending by failed attempt count
                data.sort((a, b) => a.FailedAttempt - b.FailedAttempt);

                const labels = data.map(item => `Failed: ${item.FailedAttempt}`);
                const counts = data.map(item => item.Users);
                const colors = [
                    '#6b5b95', '#feb236', '#d64161', '#ff7b25', '#b2ad7f',
                    '#92a8d1', '#88b04b', '#f7cac9', '#955251', '#b565a7',
                    '#009688', '#f7786b', '#5d5d5d', '#ff6f61'
                ];

                const canvas = document.getElementById('barChartFailedAttempts');
                const ctx = canvas.getContext('2d');

                if (failedAttemptChart) {
                    failedAttemptChart.destroy();
                    failedAttemptChart = null;
                    canvas.height = 370;
                    canvas.width = 800;
                }

                failedAttemptChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'User Count',
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
                                    font: { size: 11 },
                                    display: true
                                },
                                grid: { display: false }
                            },
                            y: {
                                grid: { display: false },
                                display: true,
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Number of Users',
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
                        barPercentage: 0.2
                    },
                    plugins: [ChartDataLabels]
                });
            })
            .catch(error => console.error('Error loading failed attempt chart:', error));
    }

    document.addEventListener("DOMContentLoaded", function () {
        updateChart(); // your original division chart
        loadFailedAttemptsChart(); // new failed attempt chart
    });
</script>


no i want like this chartjs for my that query and logic

 [HttpGet]
        public IActionResult GetDivisionCount()
        {
            try
            {
                    string query = @"SELECT DISTINCT DIV.Division, COUNT(INN.Department) AS Count
                         FROM INNOVATIONDB.dbo.App_Innovation INN
                         INNER JOIN TSUSMSDB.dbo.App_DepartmentMaster DEPT
                             ON DEPT.Department COLLATE DATABASE_DEFAULT = INN.Department
                         INNER JOIN TSUSMSDB.dbo.App_DivisionMaster DIV
                             ON DIV.ID = DEPT.DivisionID
                             where INN.CreatedOn>='"+startDate+"'and INN.CreatedOn<='"+endDate+"' and INN.Status = 'Approved'GROUP BY DIV.Division";
                
                using (var connection = new SqlConnection(connectionString))
                {
                    var divisionCounts = connection.Query<DivisionCount>(query).ToList();
                    return Json(divisionCounts);
                }
                
            }
            catch (Exception ex)
            {
               
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


 <div class="row">
            <div class="col-sm-12">
                <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;margin-top:3px;">
                    <h6 class="text-center overview-heading">
                        Division wise participation(Nos)</h6>
                    <h6 class="text-center overview-heading">
                        FY
                     </h6>
                    <canvas id="barChart4" class="" style="width:800px;height:370px;"></canvas>

                    <div id="legendContainer" style="display:flex;flex-wrap:wrap;"></div>
                </fieldset>
            </div>
        </div>



<script>
    let myChart;
    function updateChart(){
        
        const finyear = document.getElementById("FinYear4").value;
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

                const labels = data.map(item => item.division === "Corporate Services" ? "People Function" : item.division);
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
