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
),
DateList AS (
    SELECT @FromDate AS TheDate
    UNION ALL
    SELECT DATEADD(DAY, 1, TheDate)
    FROM DateList
    WHERE TheDate < @ToDate
),
AbsentCounts AS (
    SELECT 
        d.TheDate AS AttemptDate,
        'Absent' AS AttemptRange,
        COUNT(*) AS NumberOfUsers
    FROM DateList d
    JOIN App_Empl_Master em ON em.Discharge_Date IS NULL
    WHERE em.pno NOT IN (
        SELECT TRBDGDA_BD_PNO 
        FROM T_TRBDGDAT_EARS 
        WHERE TRBDGDA_BD_DATE = d.TheDate
    )
    GROUP BY d.TheDate
)
SELECT 
    AttemptDate,
    AttemptRange,
    NumberOfUsers,
    NULL AS TotalUsers,
    NULL AS Percentage
FROM AbsentCounts

UNION ALL

SELECT 
    g.AttemptDate,
    g.AttemptRange,
    g.NumberOfUsers,
    t.TotalUsers,
    CAST(g.NumberOfUsers * 100.0 / t.TotalUsers AS DECIMAL(5, 2)) AS Percentage
FROM GroupedCounts g
JOIN TotalPerDay t ON g.AttemptDate = t.AttemptDate

ORDER BY AttemptDate, AttemptRange
OPTION (MAXRECURSION 100);
";
results.Add(new
{
    attemptDate = Convert.ToDateTime(reader["AttemptDate"]).ToString("dd-MM-yyyy"),
    attemptRange = reader["AttemptRange"].ToString(),
    numberOfUsers = Convert.ToInt32(reader["NumberOfUsers"]),
    percentage = reader["Percentage"] == DBNull.Value ? "0" : reader["Percentage"].ToString()
});

 const ranges = ['0-2', '3-5', '6-10', '10+', 'Absent'];
const colors = {
    '0-2': 'blue',
    '3-5': 'orange',
    '6-10': 'green',
    '10+': 'red',
    'Absent': 'gray'
};
tooltip: {
    callbacks: {
        label: function (context) {
            const label = context.dataset.label || '';
            const value = context.raw.y;
            const count = context.raw.numberOfUsers;
            if (label === 'Absent') {
                return `${count} users absent`;
            }
            return `${value}% (${count} users)`;
        }
    }
}
               
                
                
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

   function loadChartData() {
       const fromDate = document.getElementById("fromDate").value;
       const toDate = document.getElementById("toDate").value;
       const attemptType = document.getElementById("attemptType").value;
       const token = document.getElementById("requestVerificationToken").value;

       if (!fromDate && !toDate) {
           alert("Please select at least one date.");
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


i want to say that in this field i want to add new absent field to show on the line graph that is my query to fetch count of absent employees
