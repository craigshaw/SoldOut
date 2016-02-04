(function () {
    google.load('visualization', '1', {
        'packages': ['corechart', 'controls']
    });

    // Courtesy of Jack Moore, http://www.jacklmoore.com/notes/rounding-in-javascript/
    function round(value, decimals) {
        return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
    }

    $(function () {
        var searchId = $('#chartContainer').attr('data-search-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: '/Home/SearchSummary/' + searchId,
            success: function (chartsdata) {

                // Create the chart data from the API response
                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Date');
                data.addColumn('number', 'Price');

                for (var i = 0; i < chartsdata.length; i++) {
                    data.addRow([chartsdata[i].PricePeriod, round(chartsdata[i].AveragePrice, 2)]);
                }

                // Create and draw the chart
                var chart = new google.visualization.LineChart(document.getElementById('chartContainer'));

                chart.draw(data,
                  {
                      title: "TODO: Chart Title"
                  });
            },
            error: function () {
                $('#chartContainer').hide();
                $('#warningMessage').show();
            }
        });
    })
})();