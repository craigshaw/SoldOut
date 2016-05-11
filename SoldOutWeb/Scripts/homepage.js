(function () {
    var conditions = { 2: "New", 7: "Used" };

    function loadMostExpensiveProductsTable() {
        var container = $('#mostExpensiveProductContainer');
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: 'Api/Expensive/',
            success: function (productData) {
                // Create the chart data from the API response
                var data = new google.visualization.DataTable();

                data.addColumn('number', '#');
                data.addColumn('string', 'Product');
                data.addColumn('number', 'Price');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([(i + 1),
                                 "<a href='/Product/" + productData[i].ProductId + "'>" + productData[i].ManufacturerCode + " " + productData[i].Name + "</a>",
                                 productData[i].AveragePrice
                    ]);
                }

                // Hide the loader
                loader.hide();

                var table = new google.visualization.Table(container[0]);

                var formatter = new google.visualization.NumberFormat({ prefix: '£' });
                formatter.format(data, 2); // Format price data correctly

                table.draw(data, { showRowNumber: false, allowHtml: true, width: '100%', height: '100%' });
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadTopSellersChart(productContainer) {
        var container = $('#topSellersContainer');
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: 'Api/TopSellers/',
            success: function (productData) {
                // Create the chart data from the API response                
                //var data = google.visualization.arrayToDataTable([
                //      ['Set Name', 'Condition', '# Bidders']
                //]);

                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Name');
                data.addColumn('number', 'New');
                data.addColumn('number', 'Used');
                //data.addColumn('string', 'Number Sold');                

                // Create a table from the response data
                for (var i = 0; i < productData.length; i+=2) {
                    data.addRow([ productData[i].ManufacturerCode.concat( ' ', productData[i].Name),
                                 parseInt(productData[i].NumberSold),
                                 parseInt(productData[i+1].NumberSold)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.charts.Bar(container[0]);

                //var formatter = new google.visualization.NumberFormat({ prefix: '£' });
                //formatter.format(data, 3); // Format price data correctly

                var options = {
                    isStacked: 'percent'                    
                }

                chart.draw(data);

                google.visualization.events.addListener(chart, 'select', function () {
                    var selection = chart.getSelection();
                    //var message = '';

                    //for (var i = 0; i < selection.length; i++) {
                    //    var item = selection[i];
                    //    if (item.row != null && item.column != null) {
                    //        message += '{row:' + item.row + ',column:' + item.column + '}';
                    //    } else if (item.row != null) {
                    //        message += '{row:' + item.row + '}';
                    //    } else if (item.column != null) {
                    //        message += '{column:' + item.column + '}';
                    //    }
                    //}
                    //if (message == '') {
                    //    message = 'nothing';
                    //}
                    //alert('You selected ' + message);

                    var pid = productData[(selection[0].row * 2) + (selection[0].column - 1)].ProductId;

                    window.location.href = "/Product/" + pid;
                });
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadPopularProductsTable(productContainer) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var conditionId = container.attr('data-search-conditionId');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: 'Api/Popular/' + conditionId,
            success: function (productData) {
                // Create the chart data from the API response
                var data = new google.visualization.DataTable();

                data.addColumn('number', '#');
                data.addColumn('string', 'Product');
                data.addColumn('number', 'Sold');
                data.addColumn('number', 'Av Price');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([(i+1),
                                 "<a href='/Product/" + productData[i].ProductId + "'>" + productData[i].ManufacturerCode + " " + productData[i].Name + "</a>",
                                 productData[i].ItemCount,
                                 productData[i].AveragePrice
                    ]);
                }

                // Hide the loader
                loader.hide();

                var table = new google.visualization.Table(container[0]);

                var formatter = new google.visualization.NumberFormat({ prefix: '£' });
                formatter.format(data, 3); // Format price data correctly

                table.draw(data, { showRowNumber: false, allowHtml: true, width: '100%', height: '100%' });
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadProductTables()
    {
        loadPopularProductsTable('newProductContainer');
        loadPopularProductsTable('usedProductContainer');
        loadMostExpensiveProductsTable();
        loadTopSellersChart();
    }

    $(function () {
        google.charts.load('current', {
            'packages': ['table','corechart','bar']
        });
        google.charts.setOnLoadCallback(loadProductTables);
    })
})();