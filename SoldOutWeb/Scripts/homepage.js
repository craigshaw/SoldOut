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

    

    function loadPopularProductsTable(productContainer) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var conditionId = container.attr('data-search-conditionId');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: '../Api/Popular/' + conditionId,
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
        // Work out the base URL to pass into the charting function so they can use the general charting API
        pathArray = location.href.split('/');
        protocol = pathArray[0];
        host = pathArray[2];
        url = protocol + '//' + host;

        //loadPopularProductsTable('newProductContainer');
        //loadPopularProductsTable('usedProductContainer');
        //loadMostExpensiveProductsTable();
        loadTopSellersBarChart('topSellersByProduct', url + '/api/TopSellingProducts/');
        loadTopSellersPieChart('topSellersByCategory', url + '/api/TopSellingCategories/');        
    }

    $(function () {
        google.charts.load('current', {
            'packages': ['table','corechart','bar']
        });
        google.charts.setOnLoadCallback(loadProductTables);
    })
})();