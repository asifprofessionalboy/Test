<form onsubmit="loadChartData(); return false;">

[HttpGet]
public IActionResult GraphReport(string fromDate, string toDate)
{
    var results = new List<dynamic>();

    if (!DateTime.TryParseExact(fromDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime from))
        return BadRequest("Invalid fromDate");
    if (!DateTime.TryParseExact(toDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime to))
        return BadRequest("Invalid toDate");

    using (SqlConnection conn = new SqlConnection(GetRFIDConnectionString()))
    {
        conn.Open();

        string query = @"
            SELECT 
                CONVERT(date, DateAndTime) AS AttemptDate,
                CASE 
                    WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                    WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                    WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                    ELSE '11+'
                END AS AttemptRange,
                COUNT(DISTINCT Pno) AS NumberOfUsers
            FROM App_FaceVerification_Details
            WHERE CONVERT(date, DateAndTime) BETWEEN @FromDate AND @ToDate
            GROUP BY 
                CONVERT(date, DateAndTime),
                CASE 
                    WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                    WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                    WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                    ELSE '11+'
                END
            ORDER BY AttemptDate";

        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@FromDate", from.Date);
            cmd.Parameters.AddWithValue("@ToDate", to.Date);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(new
                    {
                        attemptDate = Convert.ToDateTime(reader["AttemptDate"]).ToString("yyyy-MM-dd"),
                        attemptRange = reader["AttemptRange"].ToString(),
                        numberOfUsers = Convert.ToInt32(reader["NumberOfUsers"])
                    });
                }
            }
        }
    }

    return Ok(results);
}



i have this controller code
   public IActionResult GraphReport()
   {
      

       return View();

   }
   [HttpPost]
   public IActionResult GraphReport(DateTime fromDate, DateTime toDate)
   {
       var results = new List<dynamic>();


       using (SqlConnection conn = new SqlConnection(GetRFIDConnectionString()))
       {
           conn.Open();

           string query = @"
       SELECT 
           CONVERT(date, DateAndTime) AS AttemptDate,
           CASE 
               WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
               WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
               WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
               ELSE '11+'
           END AS AttemptRange,
           COUNT(DISTINCT Pno) AS NumberOfUsers
       FROM App_FaceVerification_Details
       WHERE CONVERT(date, DateAndTime) BETWEEN @FromDate AND @ToDate
       GROUP BY 
           CONVERT(date, DateAndTime),
           CASE 
               WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
               WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
               WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
               ELSE '11+'
           END
       ORDER BY AttemptDate";

           using (SqlCommand cmd = new SqlCommand(query, conn))
           {
               cmd.Parameters.AddWithValue("@FromDate", fromDate);
               cmd.Parameters.AddWithValue("@ToDate", toDate);

               using (SqlDataReader reader = cmd.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       results.Add(new
                       {
                           AttemptDate = reader["AttemptDate"].ToString(),
                           AttemptRange = reader["AttemptRange"].ToString(),
                           NumberOfUsers = Convert.ToInt32(reader["NumberOfUsers"])
                       });
                   }
               }
           }
       }
       return Ok(results);
   }

and this is my view side 
<form asp-action="GraphReport" asp-controller="Report">
    <label>From:</label>
    <input type="date" class="form-control" id="fromDate" name="fromDate" value="@DateTime.Today.ToString("yyyy-MM-dd")">
    <label>To:</label>
    <input type="date" class="form-control" id="toDate" name="toDate" value="@DateTime.Today.ToString("yyyy-MM-dd")">
    <button onclick="loadChartData()" class="btn btn-primary">Apply Filter</button>

    <canvas id="attemptChart" height="100"></canvas>

</form>

<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script>
    let chartInstance;

    function loadChartData() {
        const fromDate = document.getElementById("fromDate").value;
        const toDate = document.getElementById("toDate").value;

        if (!fromDate || !toDate) {
            alert("Please select both From and To dates.");
            return;
        }

        fetch(`/Report/GraphReport?fromDate=${fromDate}&toDate=${toDate}`)
            .then(res => res.json())
            .then(data => {
                if (!data || data.length === 0) {
                    alert("No data available for the selected date range.");
                    return;
                }

                const labels = [...new Set(data.map(d => d.attemptDate))];
                const ranges = ['0-2', '3-5', '6-10', '11+'];
                const colors = {
                    '0-2': 'blue',
                    '3-5': 'orange',
                    '6-10': 'green',
                    '11+': 'red'
                };

                const datasets = ranges.map(range => ({
                    label: range,
                    borderColor: colors[range],
                    backgroundColor: colors[range],
                    tension: 0.3,
                    fill: false,
                    data: labels.map(date => {
                        const match = data.find(d => d.attemptDate === date && d.attemptRange === range);
                        return match ? match.numberOfUsers : 0;
                    })
                }));

                if (chartInstance) chartInstance.destroy();

                chartInstance = new Chart(document.getElementById('attemptChart'), {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: datasets
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            title: {
                                display: true,
                                text: 'Punch-In Attempt Distribution by Date'
                            },
                            legend: {
                                position: 'top'
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Number of Users'
                                }
                            },
                            x: {
                                title: {
                                    display: true,
                                    text: 'Date'
                                }
                            }
                        }
                    }
                });
            })
            .catch(error => {
                console.error("Error fetching data:", error);
                alert("Failed to load data.");
            });
    }
</script>



i want when i click on apply filter based on fromDate and toDate it shows the graph values , it shows me failed to load data
