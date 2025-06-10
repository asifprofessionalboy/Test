[HttpGet("GetAttemptsBySql")]
public IActionResult GetAttemptsBySql(DateTime fromDate, DateTime toDate)
{
    var results = new List<dynamic>();

    using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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

<label>From:</label>
<input type="date" id="fromDate">
<label>To:</label>
<input type="date" id="toDate">
<button onclick="loadChartData()">Apply Filter</button>

<canvas id="attemptChart" height="100"></canvas>

<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script>
let chartInstance;

function loadChartData() {
    const fromDate = document.getElementById("fromDate").value;
    const toDate = document.getElementById("toDate").value;

    fetch(`/api/FaceAttempts/GetAttempts?fromDate=${fromDate}&toDate=${toDate}`)
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

            if (chartInstance) chartInstance.destroy(); // destroy old instance

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



SELECT 
    CAST(DateAndTime AS DATE) AS AttemptDate,
    CASE 
        WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
        WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
        WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
        ELSE '11+'
    END AS AttemptRange,
    COUNT(DISTINCT Pno) AS NumberOfUsers
FROM 
    App_FaceVerification_Details
GROUP BY 
    CAST(DateAndTime AS DATE),
    CASE 
        WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
        WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
        WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
        ELSE '11+'
    END
ORDER BY 
    AttemptDate;



 

Pno	DateAndTime	          PunchIn_FailedCount	PunchIn_Success	PunchOut_FailedCount	PunchOut_Success
159695	2025-06-10 08:56:42.527	             1	              1	                 0	             0
841738	2025-06-10 08:51:00.350	             0	              1	                 0	             0


 this is my table 

select * from App_FaceVerification_Details where CAST(DateAndTime AS DATE)='2025-06-10'  


i want a query to use on line graph to find out numbers of attempts over time 


in y axis i want number of user and in x axis i date wise and color of lines decide punchInfailedcount like 0-2, 3-5, 5-10 and 10-above 

i have a sample image to understand
