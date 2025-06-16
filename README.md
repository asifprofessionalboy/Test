else if (attemptType == "Absent")
{
    string query = @"
    ;WITH DateList AS (
        SELECT @FromDate AS TheDate
        UNION ALL
        SELECT DATEADD(DAY, 1, TheDate)
        FROM DateList
        WHERE TheDate < @ToDate
    )
    SELECT 
        d.TheDate AS AttemptDate,
        'Absent' AS AttemptRange,
        (
            SELECT COUNT(*) 
            FROM App_Empl_Master em
            WHERE em.Discharge_Date IS NULL
            AND em.pno NOT IN (
                SELECT TRBDGDA_BD_PNO 
                FROM T_TRBDGDAT_EARS 
                WHERE TRBDGDA_BD_DATE = d.TheDate
            ) 
        ) AS NumberOfUsers,
        0 AS TotalUsers, -- not needed for absent
        0 AS Percentage
    FROM DateList d
    ORDER BY d.TheDate
    OPTION (MAXRECURSION 100);
    ";

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
                    attemptDate = Convert.ToDateTime(reader["AttemptDate"]).ToString("dd-MM-yyyy"),
                    attemptRange = reader["AttemptRange"].ToString(), // always "Absent"
                    numberOfUsers = Convert.ToInt32(reader["NumberOfUsers"]),
                    percentage = 0 // optional, since this graph is just raw count
                });
            }
        }
    }
}
const ranges = attemptType === 'Absent' 
    ? ['Absent'] 
    : ['0-2', '3-5', '6-10', '10+'];

const colors = {
    '0-2': 'blue',
    '3-5': 'orange',
    '6-10': 'green',
    '10+': 'red',
    'Absent': 'gray'
};

label: function (context) {
    const label = context.dataset.label || '';
    const value = context.raw.y;
    const count = context.raw.numberOfUsers;
    return attemptType === 'Absent'
        ? `${count} users absent`
        : `${value}% (${count} users)`;
}


this is my dropdown 

 <div class="col-sm-2">
     <select class="form-control form-control-sm" id="attemptType">
         <option value="PunchIn">PunchIn</option>
         <option value="PunchOut">PunchOut</option>
 <option value="Absent">Absent</option>

     </select>
 </div>


DECLARE @StartDate DATE = '2025-06-01';
DECLARE @EndDate DATE = '2025-06-30';

WITH DateList AS (
    SELECT @StartDate AS TheDate
    UNION ALL
    SELECT DATEADD(DAY, 1, TheDate)
    FROM DateList
    WHERE TheDate < @EndDate
)
SELECT d.TheDate,
    (
        SELECT COUNT(*) 
        FROM App_Empl_Master em
        WHERE em.Discharge_Date IS NULL
        AND em.pno NOT IN (
            SELECT TRBDGDA_BD_PNO 
            FROM T_TRBDGDAT_EARS 
            WHERE TRBDGDA_BD_DATE = d.TheDate
        ) 
    ) AS AbsentCount 
FROM DateList d
ORDER BY d.TheDate
OPTION (MAXRECURSION 31);


this is my chartjs for attempt 
<script>
    let chartInstance;

    function loadChartData() {
        const fromDate = document.getElementById("fromDate").value;
        const toDate = document.getElementById("toDate").value;
        const attemptType = document.getElementById("attemptType").value;
        const token = document.getElementById("requestVerificationToken").value;

        if (!fromDate && !toDate) {
            alert("Please select at least one date.");
            return;
        }

        fetch('/TSUISLARS/Report/GraphReport', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: `fromDate=${fromDate}&toDate=${toDate}&attemptType=${attemptType}`
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

                const labels = [...new Set(data.map(d => d.attemptDate))];
                const ranges = ['0-2', '3-5', '6-10', '10+'];
                const colors = {
                    '0-2': 'blue',
                    '3-5': 'orange',
                    '6-10': 'green',
                    '10+': 'red',
                };

                const datasets = ranges.map(range => {
                    return {
                        label: range,
                        borderColor: colors[range],
                        backgroundColor: colors[range],
                        tension: 0.3,
                        fill: false,
                        data: labels.map(date => {
                            const match = data.find(d => d.attemptDate === date && d.attemptRange === range);
                            return {
                                x: date,
                                y: match ? match.percentage : 0,
                                numberOfUsers: match ? match.numberOfUsers : 0
                            };
                        })
                    };
                });

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
                                text: `${attemptType} Attempt Distribution by Date`
                            },
                            legend: {
                                position: 'top'
                            },
                            tooltip: {
                                callbacks: {
                                    label: function (context) {
                                        const label = context.dataset.label || '';
                                        const value = context.raw.y;
                                        const count = context.raw.numberOfUsers;
                                        return `${value}% (${count} users)`;
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                max: 100,
                                title: {
                                    display: true,
                                    text: 'Percentage (%)',
                                    font: {
                                        weight: 'bold',
                                        size: 11
                                    }
                                }
                            },
                            x: {
                                title: {
                                    display: true,
                                    text: 'Date',
                                    font: {
                                        weight: 'bold',
                                        size: 11
                                    }

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

    document.addEventListener('DOMContentLoaded', loadChartData);
</script>


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GraphReport(DateTime fromDate, DateTime toDate,string attemptType)
        {
            if (toDate == DateTime.MinValue || toDate == default)
            {
                toDate = fromDate; 
            }
            else if (fromDate == DateTime.MinValue || fromDate == default)
            {
                fromDate = toDate;
            }

            var results = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(GetRFIDConnectionString()))
            {
                conn.Open();
                string query = "";

                if (attemptType == "PunchIn")
                {

                    query = @"
                 WITH TotalPerDay AS (
    SELECT 
        CONVERT(date, DateAndTime) AS AttemptDate,
        COUNT(DISTINCT Pno) AS TotalUsers
    FROM App_FaceVerification_Details
    WHERE CONVERT(date, DateAndTime) BETWEEN @FromDate AND @ToDate
    GROUP BY CONVERT(date, DateAndTime)
),
GroupedCounts AS (
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
    WHERE CONVERT(date, DateAndTime) BETWEEN @FromDate AND @ToDate
    GROUP BY 
        CONVERT(date, DateAndTime),
        CASE 
            WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
            WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
            WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
            ELSE '10+'
        END
)
SELECT 
    g.AttemptDate,
    g.AttemptRange,
    g.NumberOfUsers,
    t.TotalUsers,
    CAST(g.NumberOfUsers * 100.0 / t.TotalUsers AS DECIMAL(5, 2)) AS Percentage
FROM GroupedCounts g
JOIN TotalPerDay t ON g.AttemptDate = t.AttemptDate
ORDER BY g.AttemptDate, g.AttemptRange;";
                }
                else if (attemptType == "PunchOut")
                {
                    query = @"
                 WITH TotalPerDay AS (
    SELECT 
        CONVERT(date, DateAndTime) AS AttemptDate,
        COUNT(DISTINCT Pno) AS TotalUsers
    FROM App_FaceVerification_Details
    WHERE CONVERT(date, DateAndTime) BETWEEN @FromDate AND @ToDate
    GROUP BY CONVERT(date, DateAndTime)
),
GroupedCounts AS (
    SELECT 
        CONVERT(date, DateAndTime) AS AttemptDate,
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
)
SELECT 
    g.AttemptDate,
    g.AttemptRange,
    g.NumberOfUsers,
    t.TotalUsers,
    CAST(g.NumberOfUsers * 100.0 / t.TotalUsers AS DECIMAL(5, 2)) AS Percentage
FROM GroupedCounts g
JOIN TotalPerDay t ON g.AttemptDate = t.AttemptDate
ORDER BY g.AttemptDate, g.AttemptRange;";
                }
               

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
                                attemptDate = Convert.ToDateTime(reader["AttemptDate"]).ToString("dd-MM-yyyy"),
                                attemptRange = reader["AttemptRange"].ToString(),
                                numberOfUsers = Convert.ToInt32(reader["NumberOfUsers"]),
                                percentage = reader["Percentage"].ToString()
                            });
                        }
                    }
                }
            }
            return Json(results);
        }

i want in this to show count absent user when  user select absent

this is my controller code 
