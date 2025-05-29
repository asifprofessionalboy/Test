string query = @"
SELECT 
    PunchIn_FailedCount AS FailedAttempt, 
    COUNT(PunchIn_FailedCount) AS Users 
FROM 
    App_FaceVerification_Details 
WHERE 
    PunchIn_Success = 1
    AND CAST(DateAndTime AS DATE) = @Date
GROUP BY 
    PunchIn_FailedCount";

var divisionCounts = connection.Query<AppDetails>(query, new { Date = DateTime.Today }).ToList();

<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

<script>
    document.addEventListener("DOMContentLoaded", function () {
        fetch('/Dashboard/GetCount') // Make sure this matches your controller
            .then(response => response.json())
            .then(data => {
                // Sort by FailedAttempt so 0, 1, 2, ... appear in order
                data.sort((a, b) => a.FailedAttempt - b.FailedAttempt);

                const labels = data.map(item => item.FailedAttempt);
                const users = data.map(item => item.Users);

                const ctx = document.getElementById('barChart4').getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels, // FailedAttempt on X-axis
                        datasets: [{
                            label: 'Users',
                            data: users, // Users on Y-axis
                            backgroundColor: 'rgba(54, 162, 235, 0.7)',
                            borderColor: 'rgba(54, 162, 235, 1)',
                            borderWidth: 1
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: {
                                display: false
                            },
                            title: {
                                display: true,
                                text: 'Punch In Failed Attempts (Today)'
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'User Count'
                                }
                            },
                            x: {
                                title: {
                                    display: true,
                                    text: 'Failed Attempt Count'
                                }
                            }
                        }
                    }
                });
            })
            .catch(error => {
                console.error('Error loading chart data:', error);
            });
    });
</script>



i have this controller code 
      [HttpGet]
      public IActionResult GetCount()
      {
          try
          {
              string connectionString = GetRFIDConnectionString();


              DateTime startDate = DateTime.Today;
            
              string query = @"select PunchIn_FailedCount as FailedAttempt, count(PunchIn_FailedCount) as Users from App_FaceVerification_Details where PunchIn_Success =1
and CAST(DateAndTime as Date) = '2025-05-29'
group by PunchIn_FailedCount";

              using (var connection = new SqlConnection(connectionString))
              {
                  var divisionCounts = connection.Query<AppDetails>(query).ToList();
                  return Json(divisionCounts);
              }

          }
          catch (Exception ex)
          {

              Console.WriteLine(ex.Message);
              return StatusCode(500, "An error occurred while processing your request.");
          }
      }
check the dapper query is correct or not 

FailedAttempt	Users
0	394
1	56
2	35
3	8
4	9
5	3
6	7
7	5
8	2
11	3
16	1
17	1

using the query using Group by i want to show failed Attempts on x axis and Users count on Y axis . let failed attempt 0 on x axis and 394 on bar like this i want my chartjs

