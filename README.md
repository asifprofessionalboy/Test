function updateBarChart(departments) {
    const selectedDivision = $("#DivisionDropdown").val();
    const selectedFinYear = $("#FinYear5").val(); // Get the selected financial year

    if (selectedDivision && departments.length > 0) {
        $.ajax({
            url: '@Url.Action("GetDepartmentCounts", "Innovation")',
            type: 'GET',
            data: { 
                division: selectedDivision, 
                departments: departments,
                FinYear5: selectedFinYear // Pass FinYear5 to the controller
            },
            traditional: true,
            success: function (data) {
                console.log("Chart Data:", data);

                const labels = data.map(item => item.department);
                const counts = data.map(item => item.count);
                const colors = labels.map(() => getRandomColor());

                if (barChart) {
                    barChart.destroy();
                }

                const ctx = document.getElementById("barChart6").getContext("2d");

                barChart = new Chart(ctx, {
                    type: "bar",
                    data: {
                        labels: labels,
                        datasets: [
                            {
                                label: "Department Count",
                                data: counts,
                                backgroundColor: colors,
                                borderColor: colors,
                                borderWidth: 1
                            }
                        ]
                    },
                    options: { /* Same as your previous settings */ },
                    plugins: [ChartDataLabels]
                });
            },
            error: function () {
                alert("An error occurred while fetching chart data. Please try again.");
            }
        });
    } else if (barChart) {
        barChart.destroy(); // Clear the chart if no data
    }
}






$("#FinYear5").change(function () {
    const selectedDepartments = $(".department-checkbox:checked")
        .not("#selectAll")
        .map(function () {
            return $(this).val();
        })
        .get();

    updateBarChart(selectedDepartments);
});


[HttpGet]
public IActionResult GetDepartmentCounts(string division, [FromQuery] List<string> departments, string FinYear5 = "24-25")
{
    string connectionString = "Server=10.0.168.50;Database=INNOVATIONDB;User Id=fs;Password=p@ssW0Rd321";

    DateTime startDate = DateTime.MinValue;
    DateTime endDate = DateTime.MaxValue;

    if (FinYear5 == "24-25")
    {
        startDate = new DateTime(2024, 4, 1);
        endDate = new DateTime(2025, 3, 31);
    }
    else if (FinYear5 == "25-26")
    {
        startDate = new DateTime(2025, 4, 1);
        endDate = new DateTime(2026, 3, 31);
    }

    string query = @"
    SELECT DISTINCT 
        DIV.Division,
        INN.Department,
        COUNT(INN.Department) AS Count
    FROM App_Innovation INN
    INNER JOIN TSUSMSDB.dbo.App_DepartmentMaster DEPT 
        ON DEPT.Department COLLATE DATABASE_DEFAULT = INN.Department
    INNER JOIN TSUSMSDB.dbo.App_DivisionMaster DIV 
        ON DIV.ID = DEPT.DivisionID
    WHERE DIV.Division = @Division
    AND INN.Department IN @Departments
    AND INN.Status = 'Approved'
    AND INN.CreatedOn >= @StartDate AND INN.CreatedOn <= @EndDate
    GROUP BY DIV.Division, INN.Department";

    using (var connection = new SqlConnection(connectionString))
    {
        var parameters = new
        {
            Division = division,
            Departments = departments,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = connection.Query<Departmentdd>(query, parameters).ToList();
        return Json(result);
    }
}




