  $(document).ready(function () {
      let barChart = null;

      function updateBarChart(departments) {
          const selectedDivision = $("#DivisionDropdown").val();
          const selectedFinYear = $("#FinYear5").val(); // Get the selected financial year
          function updateBarChart(departments) {

              if (selectedDivision && departments.length > 0) {
                  $.ajax({
                      url: '@Url.Action("GetDepartmentCounts", "Innovation")',
                      type: 'GET',
                      data: { division: selectedDivision, departments: departments, FinYear5: selectedFinYear },
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
                              options: {
                                  responsive: true,
                                  plugins: {
                                      datalabels: {
                                          anchor: 'end',
                                          align: 'top',
                                          color: '#000',
                                          font: {
                                              weight: 'bold',
                                              size: 10,
                                              color: 'black'
                                          },
                                          formatter: value => value
                                      },
                                      legend: {
                                          display: false,
                                          position: 'top',
                                      }
                                  },
                                  scales: {
                                      y: {
                                          beginAtZero: true,
                                          display: true,
                                          grid: {
                                              display: false
                                          },
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
                                              color: '#000',
                                              font: {
                                                  size: 12,
                                                  weight: 'bold'
                                              }
                                          }
                                      },
                                      x: {
                                          grid: {
                                              display: false
                                          },
                                          ticks: {
                                              callback: function (value, index, ticks) {
                                                  const label = this.getLabelForValue(value);
                                                  return label.split(' ');
                                              },
                                              autoSkip: false,
                                              maxRotation: 0,
                                              minRotation: 0,
                                              font: {
                                                  size: 11,
                                                  color: 'black'

                                              },
                                              padding: 5
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
                      },
                      error: function () {
                          alert("An error occurred while fetching chart data. Please try again.");
                      }
                  });
              } else if (barChart) {
                  barChart.destroy(); // Clear the chart if no data
              }
          }


          function getRandomColor() {
              const letters = "0123456789ABCDEF";
              let color = "#";
              for (let i = 0; i < 6; i++) {
                  color += letters[Math.floor(Math.random() * 16)];
              }
              return color;
          }

          $("#DivisionDropdown").change(function () {
              const selectedDivision = $(this).val();
              $("#DepartmentDropdownButton").val("");
              $("#DepartmentList").empty();

              if (selectedDivision) {
                  $.ajax({
                      url: '@Url.Action("GetDepartments", "Innovation")',
                      type: 'GET',
                      data: { division: selectedDivision },
                      success: function (departments) {
                          console.log("Departments received:", departments);

                          if (departments && departments.length > 0) {
                              const selectAllItem = `<li>
                                  <label class="dropdown-item">
                                      <input type="checkbox" id="selectAll" class="department-checkbox" />
                                      Select All
                                  </label>
                              </li>`;
                              $("#DepartmentList").append(selectAllItem);

                              departments.forEach(function (department) {
                                  const listItem = `<li>
                                      <label class="dropdown-item">
                                          <input type="checkbox" name="Department" value="${department.department}" class="department-checkbox" />
                                          ${department.department}
                                      </label>
                                  </li>`;
                                  $("#DepartmentList").append(listItem);
                              });

                              $(".department-checkbox").change(function () {
                                  if (this.id === "selectAll") {
                                      const isChecked = $(this).is(":checked");
                                      $(".department-checkbox").not("#selectAll").prop("checked", isChecked);

                                      const allDepartments = isChecked
                                          ? $(".department-checkbox")
                                              .not("#selectAll")
                                              .map(function () {
                                                  return $(this).val();
                                              })
                                              .get()
                                          : [];
                                      updateSelectedCount(allDepartments);
                                      updateBarChart(allDepartments);
                                  } else {
                                      const selectedDepartments = $(".department-checkbox:checked")
                                          .not("#selectAll")
                                          .map(function () {
                                              return $(this).val();
                                          })
                                          .get();

                                      $("#selectAll").prop("checked", selectedDepartments.length === $(".department-checkbox").not("#selectAll").length);

                                      updateSelectedCount(selectedDepartments);
                                      updateBarChart(selectedDepartments);
                                  }
                              });
                          } else {
                              $("#DepartmentList").append('<li class="dropdown-item disabled">No departments available</li>');
                          }
                      },
                      error: function () {
                          alert("An error occurred while fetching departments. Please try again.");
                      }
                  });
              } else {
                  $("#DepartmentList").append('<li class="dropdown-item disabled">---- Select Department ----</li>');
              }

              // Clear the bar chart when switching divisions
              if (barChart) {
                  barChart.destroy();
                  barChart = null;
              }
          });

          function updateSelectedCount(selectedDepartments) {
              const selectedCount = selectedDepartments.length;
              $("#DepartmentDropdownButton").val(selectedCount > 0 ? `${selectedCount} selected` : "");
          }
          $("#FinYear5").change(function () {
              const selectedDepartments = $(".department-checkbox:checked")
                  .not("#selectAll").map(function () { return $(this).val(); }).get();

              updateBarChart(selectedDepartments);
          });


      });
