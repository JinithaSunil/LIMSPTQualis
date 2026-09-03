<script>
    function renderChart(months, income, expense) {

    const ctx = document.getElementById('incomeExpenseChart').getContext('2d');

    new Chart(ctx, {
        type: 'line',
    data: {
        labels: months,
    datasets: [
    {
        label: 'Income',
    data: income,
    tension: 0.3
                },
    {
        label: 'Expense',
    data: expense,
    tension: 0.3
                }
    ]
        },
    options: {
        responsive: true,
    plugins: {
        legend: {position: 'top' },
    title: {
        display: true,
    text: 'Income vs Expense (Yearly)'
                }
            }
        }
    });
}
    var invPending = document.getElementById('<%=hfInvPending.ClientID%>').value;
    var invPaid = document.getElementById('<%=hfInvPaid.ClientID%>').value;
    var invOverdue = document.getElementById('<%=hfInvOverdue.ClientID%>').value;

    new Chart(document.getElementById('invoiceChart'), {
        type: 'pie',
    data: {
        labels: ['Pending', 'Paid', 'Overdue'],
    datasets: [{
        data: [invPending, invPaid, invOverdue]
        }]
    }
});

</script>
