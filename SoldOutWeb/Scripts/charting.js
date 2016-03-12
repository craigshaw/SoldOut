(function () {
    google.load('visualization', '1', {
        'packages': ['corechart', 'controls']
    });

    // Courtesy of Jack Moore, http://www.jacklmoore.com/notes/rounding-in-javascript/
    function round(value, decimals) {
        return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
    }

    $(function() {
        var searchId = $('#chartContainer').attr('data-search-id');
        var conditionId = $('#chartContainer').attr('data-search-conditionId');
        var chartTitle = $('#chartContainer').attr('data-search-title') + '-' + conditionId;

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: '/PriceHistory/' + searchId + '/' + conditionId,
            success: function (chartsdata) {
                // Create the chart data from the API response
                var data = new google.visualization.DataTable();
                //var addMinMaxPrices = document.getElementById('showMinMax').checked;

                data.addColumn('string', 'Date');
                data.addColumn('number', 'Avg Price');
                data.addColumn('number', 'Min Price');
                data.addColumn('number', 'Max Price');

                for (var i = 0; i < chartsdata.length; i++) {
                        data.addRow([chartsdata[i].PricePeriod,
                                     round(chartsdata[i].AveragePrice, 2),
                                     round(chartsdata[i].MinPrice, 2),
                                     round(chartsdata[i].MaxPrice, 2)
                        ]);
                }

                // Hide the loader
                $('#loader').hide();

                // Create and draw the chart
                var chart = new google.visualization.LineChart(document.getElementById('chartContainer'));

                var options =
                {
                    title: chartTitle,
                    curveType: 'function'
                };

                chart.draw(data, options);
            },
            error: function () {
                $('#chartContainer').hide();
                $('#warningMessage').show();
            }
        });
    })
})();