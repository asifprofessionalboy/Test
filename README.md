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
