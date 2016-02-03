(function () {
    google.load('visualization', '1', {
        'packages': ['corechart', 'controls']
    });

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
                    data.addRow([chartsdata[i].PricePeriod, chartsdata[i].AveragePrice]);
                }

                // Create and draw the chart
                var chart = new google.visualization.LineChart(document.getElementById('chartContainer'));

                chart.draw(data,
                  {
                      title: "TODO: Chart Title"
                  });
            },
            error: function () {
                // TODO: Make this better - show an error message inline or something
                alert("Couldn't get chart data");
            }
        });
    })
})();