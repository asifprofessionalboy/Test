Chart.register(ChartDataLabels);

var canvas = document.getElementById('barChart');
canvas.width = 500;
canvas.height = 400;

const ctx = document.getElementById('barChart').getContext('2d');

var barChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: [
            'Project\nentries', 
            'Pending for\nApproval', 
            'Rejected', 
            'Approved &\nPublished'
        ],
        datasets: [{
            data: [@ViewBag.TotalInnovations, @ViewBag.TotalPendingInnovations, @ViewBag.TotalRejectedInnovations, @ViewBag.TotalApprovedInnovations],
            backgroundColor: [
                '#f9b037',
                '#5a7bf9',
                '#ed7b8e',
                '#6aa84f'
            ],
            borderColor: [
                '#f9b037',
                '#5a7bf9',
                '#ed7b8e',
                '#6aa84f'
            ],
            borderWidth: 0.5
        }]
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
                    size: 11
                },
                formatter: (value) => {
                    return value;
                }
            },
            legend: {
                display: false
            }
        },
        scales: {
            y: {
                beginAtZero: true,
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
                    font: {
                        size: 10,
                        family: 'Arial',
                        weight: 'bold',
                        color: 'black'
                    }
                }
            },
            x: {
                grid: {
                    display: false 
                },
                ticks: {
                    font: {
                        size: 11,
                        family: 'Arial',
                        weight: 'bold',
                        color: 'black'
                    }
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


Chart.register(ChartDataLabels);

var canvas = document.getElementById('barChart');
canvas.width = 500;
canvas.height = 400;

const ctx = document.getElementById('barChart').getContext('2d');

var barChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: ['Project entries', 'Pending for Approval', 'Rejected', 'Approved & Published'],
        datasets: [{
            data: [@ViewBag.TotalInnovations, @ViewBag.TotalPendingInnovations, @ViewBag.TotalRejectedInnovations, @ViewBag.TotalApprovedInnovations],
            backgroundColor: [
                '#f9b037',
                '#5a7bf9',
                '#ed7b8e',
                '#6aa84f'
            ],
            borderColor: [
                '#f9b037',
                '#5a7bf9',
                '#ed7b8e',
                '#6aa84f'
            ],
            borderWidth: 0.5
        }]
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
                    size: 11
                },
                formatter: (value) => {
                    return value;
                }
            },
            legend: {
                display: false
            }
        },
        scales: {
            y: {
                beginAtZero: true,
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
                    font: {
                        size: 10,
                        family: 'Arial',
                        weight: 'bold',
                        color: 'black'
                    }
                }
            },
            x: {
                grid: {
                    display: false
                },
                ticks: {
                    font: {
                        size: 11,
                        family: 'Arial',
                        weight: 'bold',
                        color: 'black'
                    },
                    // Add a callback to wrap labels
                    callback: function (value, index, values) {
                        // Split the label by spaces and join with a newline character
                        return value.split(' ').join('\n');
                    }
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
  
  
  
  Chart.register(ChartDataLabels);

  var canvas = document.getElementById('barChart');
  canvas.width = 500;
  canvas.height = 400;

  const ctx = document.getElementById('barChart').getContext('2d');

  var barChart = new Chart(ctx, {
      type: 'bar',
      data: {
          labels: ['Project entries', 'Pending for Approval', 'Rejected', 'Approved & Published'],
          datasets: [{
              data: [@ViewBag.TotalInnovations, @ViewBag.TotalPendingInnovations, @ViewBag.TotalRejectedInnovations, @ViewBag.TotalApprovedInnovations],
              backgroundColor: [
                  '#f9b037',
                  '#5a7bf9',
                  '#ed7b8e',
                  '#6aa84f'
              ],
              borderColor: [
                  '#f9b037',
                  '#5a7bf9',
                  '#ed7b8e',
                  '#6aa84f'
              ],
              borderWidth: 0.5
          }]
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
                      size: 11
                  },
                  formatter: (value) => {
                      return value;
                  }
              },
              legend: {
                  display: false
              }
          },
          scales: {
              y: {
                  beginAtZero: true,
                  
                  
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
                      display:false,
                      
                      font: {
                          size: 10,
                          family: 'Arial',
                          weight: 'bold',
                          color: 'black'
                      }
                  }
              },
              x: {
                  grid: {
                      display: false 
                  },
                  
                  
                  ticks: {
                      font: {
                          size: 11,
                          family: 'Arial',
                          weight: 'bold',
                          color: 'black'
                      }
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
