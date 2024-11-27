$(document).ready(function() {
    loadDashboardStats();
});

function loadDashboardStats() {
    $.ajax({
        url: '/Home/DashboardStats',
        type: 'GET',
        success: function(data) {
            if (data.success) {
                $('#ciasNumber').text(data.data.ciasCount ?? 0);
                $('#usersNumber').text(data.data.usersCount ?? 0);

                $('#repositoryNumber').text(data.data.repositoryNumber ?? 0);
                $('#repositoryCanUpperNumber').text(data.data.repositoryCanUpperNumber ?? 0);
                $('#repositoryNotEqualsNumber').text(data.data.repositoryNotEqualsNumber ?? 0);

                $('#dmgpolizaNumber').text(data.data.dmgPolizaNumber ?? 0);
                $('#dmgpolizaPrintedNumber').text(data.data.dmgPolizaPrinted ?? 0);
                $('#dmgpolizaNotPrintedNumber').text(data.data.dmgPolizaNotPrinted ?? 0);
            }
        }
    });
}