<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

<script>
    document.addEventListener("DOMContentLoaded", function () {
        fetch('/Dashboard/GetCount') // Replace 'Dashboard' with your actual controller name if different
            .then(response => response.json())
            .then(data => {
                // Sort the data by FailedAttempt (optional for better visual order)
                data.sort((a, b) => a.FailedAttempt - b.FailedAttempt);

                const labels = data.map(item => `Failed: ${item.FailedAttempt}`);
                const users = data.map(item => item.Users);

                const ctx = document.getElementById('barChart4').getContext('2d');
                const barChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Users',
                            data: users,
                            backgroundColor: 'rgba(75, 192, 192, 0.7)',
                            borderColor: 'rgba(75, 192, 192, 1)',
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
                                text: 'Punch In Failed Attempts - Today'
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
                                    text: 'Failed Attempt Count'
                                }
                            }
                        }
                    }
                });
            })
            .catch(error => {
                console.error('Error fetching chart data:', error);
            });
    });
</script>




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
