this is my js 

<script>
    let chartInstance;

    function loadChartData() {
        const fromDate = document.getElementById("fromDate").value;
        const toDate = document.getElementById("toDate").value;

        fetch(`/Report/GraphReport?fromDate=${fromDate}&toDate=${toDate}`)
            .then(res => res.json())
            .then(data => {
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
                                text: 'Punch-In Attempt Distribution'
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true
                            }
                        }
                    }
                });
            });
    }
</script>

and this is my controller logic 

    public IActionResult GraphReport(DateTime fromDate, DateTime toDate)
    {
        var results = new List<dynamic>();


        fromDate = DateTime.Now;
        toDate = DateTime.Now;

        using (SqlConnection conn = new SqlConnection())
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
                cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

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
i am not getting data in chart i am getting in json data please show in line js and also configure my connection string into this 
 private string GetRFIDConnectionString()
{
    return this.configuration.GetConnectionString("RFID");
}
