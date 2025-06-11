                SELECT 
                    CONVERT(date, DateAndTime) AS AttemptDate,
                    CASE 
                        WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                        WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                        WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                        ELSE '10+'
                    END AS AttemptRange,
                    COUNT(DISTINCT Pno) AS NumberOfUsers
                FROM App_FaceVerification_Details
                WHERE CONVERT(date, DateAndTime) BETWEEN '2025-06-09' AND '2025-06-11'
                GROUP BY 
                    CONVERT(date, DateAndTime),
                    CASE 
                        WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                        WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                        WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                        ELSE '10+'
                    END
                ORDER BY AttemptDate


in this query NumberOfUsers are in count i want the average means percentage vise , if record is 700 and 0-2 is 600 then it shows in percentage wise for all


[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult GraphReport(DateTime fromDate, DateTime toDate, string attemptType)
{
    if (toDate == DateTime.MinValue || toDate == default)
    {
        toDate = fromDate; // Default ToDate = FromDate
    }

    // Proceed with SQL query as before
}




function loadChartData() {
    const fromDate = document.getElementById("fromDate").value;
    let toDate = document.getElementById("toDate").value;
    const attemptType = document.getElementById("attemptType").value;
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    if (!fromDate) {
        alert("Please select at least one date.");
        return;
    }

    if (!toDate) {
        toDate = fromDate; // If ToDate not selected, use FromDate
    }

    fetch('/Report/GraphReport', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: `fromDate=${fromDate}&toDate=${toDate}&attemptType=${attemptType}`
    })
    .then(res => {
        if (!res.ok) throw new Error("Failed to fetch data");
        return res.json();
    })
    .then(data => {
        // ... existing chart logic
    })
    .catch(error => {
        console.error("Error fetching data:", error);
        alert("Failed to load data.");
    });
}
<script>
    document.getElementById("fromDate").addEventListener("change", function () {
        const toDate = document.getElementById("toDate");
        if (!toDate.value) {
            toDate.value = this.value;
        }
    });
</script>




<div class="col-sm-2">
    <select class="form-control" id="attemptType">
        <option value="PunchIn">PunchIn</option>
        <option value="PunchOut">PunchOut</option>
    </select>
</div>

<script>
    let chartInstance;

    function loadChartData() {
        const fromDate = document.getElementById("fromDate").value;
        const toDate = document.getElementById("toDate").value;
        const attemptType = document.getElementById("attemptType").value;
        const token = document.getElementById("requestVerificationToken").value;

        if (!fromDate || !toDate) {
            alert("Please select both From and To dates.");
            return;
        }

        fetch('/Report/GraphReport', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: `fromDate=${fromDate}&toDate=${toDate}&attemptType=${attemptType}`
        })
        .then(res => {
            if (!res.ok) throw new Error("Failed to fetch data");
            return res.json();
        })
        .then(data => {
            if (!data || data.length === 0) {
                alert("No data available for the selected date range.");
                return;
            }

            const labels = [...new Set(data.map(d => d.attemptDate))];
            const ranges = ['0-2', '3-5', '6-10', '10+', '11+']; // Support both types
            const colors = {
                '0-2': 'blue',
                '3-5': 'orange',
                '6-10': 'green',
                '10+': 'purple',
                '11+': 'red'
            };

            const datasets = ranges
                .filter(range => data.some(d => d.attemptRange === range))
                .map(range => ({
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
                            text: 'Punch Attempt Distribution by Date'
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
                });
        })
        .catch(error => {
            console.error("Error fetching data:", error);
            alert("Failed to load data.");
        });
    }
</script>


[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult GraphReport(DateTime fromDate, DateTime toDate, string attemptType)
{
    var results = new List<dynamic>();

    using (SqlConnection conn = new SqlConnection(GetRFIDConnectionString()))
    {
        conn.Open();

        string query = "";
        if (attemptType == "PunchIn")
        {
            query = @"
                SELECT CONVERT(date, DateAndTime) AS AttemptDate,
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
                ORDER BY AttemptDate DESC";
        }
        else if (attemptType == "PunchOut")
        {
            query = @"
                SELECT CONVERT(date, DateAndTime) AS AttemptDate,
                    CASE 
                        WHEN PunchOut_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                        WHEN PunchOut_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                        WHEN PunchOut_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                        ELSE '10+'
                    END AS AttemptRange,
                    COUNT(DISTINCT Pno) AS NumberOfUsers
                FROM App_FaceVerification_Details
                WHERE CONVERT(date, DateAndTime) BETWEEN @FromDate AND @ToDate
                GROUP BY 
                    CONVERT(date, DateAndTime),
                    CASE 
                        WHEN PunchOut_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                        WHEN PunchOut_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                        WHEN PunchOut_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                        ELSE '10+'
                    END
                ORDER BY AttemptDate DESC";
        }

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
                        AttemptDate = Convert.ToDateTime(reader["AttemptDate"]).ToString("yyyy-MM-dd"),
                        AttemptRange = reader["AttemptRange"].ToString(),
                        NumberOfUsers = Convert.ToInt32(reader["NumberOfUsers"])
                    });
                }
            }
        }
    }

    return Ok(results);
}




this is my dropdown for punchIn and Punchout
<div class="col-sm-2">
    <select class="form-control">
        <option value="PunchIn">PunchIn</option>
        <option value="PunchOut">PunchOut</option>
    </select>
</div>

this is my query for PunchOut 

   SELECT  CONVERT(date, DateAndTime) AS AttemptDate,
                CASE 
                    WHEN PunchOut_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                    WHEN PunchOut_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                    WHEN PunchOut_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                    ELSE '10+'
                END AS AttemptRange,
                COUNT(DISTINCT Pno) AS NumberOfUsers
            FROM App_FaceVerification_Details
            WHERE CONVERT(date, DateAndTime) BETWEEN '2025-06-01' AND '2025-06-10'
            GROUP BY 
                CONVERT(date, DateAndTime),
                CASE 
                    WHEN PunchOut_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                    WHEN PunchOut_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                    WHEN PunchOut_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                    ELSE '10+'
                END
            ORDER BY AttemptDate desc

i want that if punchIn then first query executes and if punchout then this query executes 
