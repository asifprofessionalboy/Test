using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

public class ReportController : Controller
{
    private readonly IAntiforgery _antiforgery;

    public ReportController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    public IActionResult GraphReport()
    {
        // Pass the antiforgery token to the view explicitly if needed
        ViewData["RequestVerificationToken"] = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
                cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

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
        return Json(results);
    }

    private string GetRFIDConnectionString()
    {
        // Return your connection string here
        return "YourConnectionString";
    }
}
@{
    ViewData["Title"] = "Graph Report";
    var token = ViewData["RequestVerificationToken"]?.ToString();
}

<h2>@ViewData["Title"]</h2>

<form id="filterForm" onsubmit="event.preventDefault(); loadChartData();">
    <div class="form-inline row mt-3">
        <div class="col-sm-1">
            <label>From:</label>
        </div>
        <div class="col-sm-3">
            <input type="date" class="form-control" id="fromDate" name="fromDate" value="@DateTime.Today.ToString("yyyy-MM-dd")" />
        </div>
        <div class="col-sm-1">
            <label>To:</label>
        </div>
        <div class="col-sm-3">
            <input type="date" class="form-control" id="toDate" name="toDate" value="@DateTime.Today.ToString("yyyy-MM-dd")" />
        </div>
        <div class="col-sm-3">
            <button type="submit" class="btn btn-primary">Apply Filter</button>
        </div>
    </div>

    <input type="hidden" id="requestVerificationToken" name="__RequestVerificationToken" value="@token" />
</form>

<canvas id="attemptChart" height="100"></canvas>

<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script>
    let chartInstance;

    function loadChartData() {
        const fromDate = document.getElementById("fromDate").value;
        const toDate = document.getElementById("toDate").value;
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
            body: `fromDate=${encodeURIComponent(fromDate)}&toDate=${encodeURIComponent(toDate)}`
        })
        .then(res => {
            if (!res.ok) {
                throw new Error("Failed to fetch data");
            }
            return res.json();
        })
        .then(data => {
            if (!data || data.length === 0) {
                alert("No data available for the selected date range.");
                if (chartInstance) chartInstance.destroy();
                return;
            }

            const labels = [...new Set(data.map(d => d.attemptDate))].sort();
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

    // Optional: load chart on page load
    document.addEventListener('DOMContentLoaded', loadChartData);
</script>





<script>
    let chartInstance;

    function loadChartData() {
        const fromDate = document.getElementById("fromDate").value;
        const toDate = document.getElementById("toDate").value;
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
            body: `fromDate=${fromDate}&toDate=${toDate}`
        })
        .then(res => {
            if (!res.ok) {
                throw new Error("Failed to fetch data");
            }
            return res.json();
        })
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
        }) // ← you missed this closing brace
        .catch(error => {
            console.error("Error fetching data:", error);
            alert("Failed to load data.");
        });
    }
</script>




getting syntax error Uncaught SyntaxError: Unexpected token ')'
<script>
    let chartInstance;

    function loadChartData() {
        const fromDate = document.getElementById("fromDate").value;
        const toDate = document.getElementById("toDate").value;
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
            body: `fromDate=${fromDate}&toDate=${toDate}`
        })
            .then(res => {
                if (!res.ok) {
                    throw new Error("Failed to fetch data");
                }
                return res.json();
            })
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
                    });
            })
            .catch(error => {
                console.error("Error fetching data:", error);
                alert("Failed to load data.");
            });
    }
</script>
