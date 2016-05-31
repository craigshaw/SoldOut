//(function () {
//    var conditions = { 2: "New", 7: "Used" };

    // Will store chart data for each chart loaded so we can easily redraw, etc
    var chartCache = {};

    // Courtesy of Jack Moore, http://www.jacklmoore.com/notes/rounding-in-javascript/
    function round(value, decimals) {
        return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
    }

    function formatDate(value)
    {
        var myDate = Date.parse(value);
        return myDate.getDate() + "-" + myDate.getMonth() + "-" + myDate.getFullYear();
    }

    function applicationBaseURL()
    {
        // Work out the base URL to pass into the charting function so they can use the general charting API
        pathArray = location.href.split('/');
        protocol = pathArray[0];
        host = pathArray[2];
        url = protocol + '//' + host;

        return url;
    }

    function redrawChart(chartName) {
        // Get the chart data for the given chart, then redraw
        if (!!chartCache[chartName]) {
            var chartData = chartCache[chartName];
            chartData.chart.draw(chartData.data, chartData.options);
        }
    }

    function loadSalesByWeekdayBarChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var categoryId = container.attr('data-category-id')

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: applicationBaseURL() + apiURL + categoryId,
            success: function (productData) {

                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Day');
                data.addColumn('number', 'Number of Bidders');
                data.addColumn('number', 'Number of sales');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].DayName,
                                 parseInt(productData[i].NumberOfBidders),
                                 parseInt(productData[i].NumberOfItemsSold)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.visualization.AreaChart(container[0]);

                var options = {
                    title: ''
                }

                chart.draw(data, options);
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadTopSellersPieChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var categoryId = container.attr('data-category-id')

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: applicationBaseURL() + apiURL + categoryId,
            success: function (productData) {
                // Create the chart data from the API response                

                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Name');
                data.addColumn('number', '# Sold');             

                // Create a table from the response data
                for (var i = 0; i < productData.length; i += 2) {
                    data.addRow([productData[i].Name,
                                 parseInt(productData[i].NumberSold) + parseInt(productData[i + 1].NumberSold)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.visualization.PieChart(container[0]);

                var options = {
                    title: 'Best selling categories over the last 30 days',
                    is3D: true,
                }

                chart.draw(data, options);

                google.visualization.events.addListener(chart, 'select', function () {
                    var selection = chart.getSelection();

                    var cid = productData[selection[0].row * 2].CategoryId;

                    window.location.href = "/Category/" + cid;
                });
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadMoversAndLosersBarChart(productContainer, apiURL, chartName) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var categoryId = container.attr('data-category-id')
        var conditionId = container.attr('data-condition-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: applicationBaseURL() + apiURL + categoryId + '/' + conditionId,
            success: function (productData) {

                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Product Name');
                data.addColumn('number', '% Price change');
                //data.addColumn('number', 'Used');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].ManufacturerCode.concat(' ', productData[i].Name),
                                 parseInt(productData[i].PercentPriceChange)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.visualization.ColumnChart(container[0]);

                var minhValue = productData[productData.length-1].PercentPriceChange;
                var maxhValue = productData[0].PercentPriceChange;

                var options = {
                    chart: {
                        title: 'Movers and losers over the last 30 days',
                        isStacked: true
                    },
                    hAxis: {
                        maxTextLines: 3
                    },
                    vAxis: {
                        title: '% price change', 
                        viewWindowMode: 'pretty',
                        baseline: 0,
                        minValue: minhValue,
                        maxValue: maxhValue
                    }                    
                }

                chart.draw(data, options);

                google.visualization.events.addListener(chart, 'select', function () {
                    var selection = chart.getSelection();

                    var pid = productData[(selection[0].row * 2)].ProductId;
                    var categoryId = productData[(selection[0].row * 2)].CategoryId;

                    var conditionId = productData[(selection[0].row * 2) + (selection[0].column) - 1].ConditionId;

                    window.location.href = "/Product/" + pid + "/" + conditionId;
                });

                // Add the loaded chart data to the cache
                chartCache[chartName] = { chart: chart, data: data, options: options };
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }


    function loadTopSellersBarChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var categoryId = container.attr('data-category-id')

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: applicationBaseURL() + apiURL + categoryId,
            success: function (productData) {

                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Product Name');
                data.addColumn('number', 'New');
                data.addColumn('number', 'Used');            

                // Create a table from the response data
                for (var i = 0; i < productData.length; i += 2) {
                    data.addRow([productData[i].ManufacturerCode.concat(' ', productData[i].Name),
                                 parseInt(productData[i].NumberSold),
                                 parseInt(productData[i + 1].NumberSold)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.charts.Bar(container[0]);

                var options = {
                    chart: {
                        title: 'Most popular products being bought over the last 30 days'
                    },
                    series: {
                        0: { axis: 'product' },
                        1: { axis: 'sales' }
                    },
                    axes: {
                        x: {
                            product: { label: 'Product Name' }
                        },
                        y: {
                            sales: { label: '# Sales' }
                        }
                    }
                }

                chart.draw(data, options);

                google.visualization.events.addListener(chart, 'select', function () {
                    var selection = chart.getSelection();

                    var pid = productData[(selection[0].row * 2)].ProductId;
                    var categoryId = productData[(selection[0].row * 2)].CategoryId;

                    var conditionId = productData[(selection[0].row * 2) + (selection[0].column) - 1].ConditionId;

                    window.location.href = "/Product/" + pid + "/" + conditionId;
                });
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadProductPricesForMACDBarChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var productId = container.attr('data-product-id');
        var conditionId = container.attr('data-product-condition-id');
        var shortInterval = container.attr('data-short-interval');
        var longInterval = container.attr('data-long-interval');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: applicationBaseURL() + apiURL + productId + '/' + conditionId + '/' + shortInterval + '/' + longInterval,
            success: function (productData) {
                // Create the chart data from the API response                

                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Date');
                data.addColumn('number', 'MACD');
                //data.addColumn('number', 'Signal Line');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].representativeDate,
                                 parseFloat(productData[i].MACD)//,
                                // parseFloat(productData[i].SignalLine)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.visualization.ColumnChart(container[0]);

                var options = {
                    isStacked: true
                }

                chart.draw(data, options);
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadProductPricesMACD(productContainer, apiURL)
    {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var productId = container.attr('data-product-id');
        var conditionId = container.attr('data-product-condition-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: apiURL + productId + '/' + conditionId,
            success: function (productData) {
                // Create the chart data from the API response                

                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Date');
                data.addColumn('number', 'Average Price');
                data.addColumn('number', 'Short EMA');
                data.addColumn('number', 'Long EMA');
                //data.addColumn('number', 'MACD');
                //data.addColumn('number', 'Signal Line');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].representativeDate,
                                 parseFloat(productData[i].AvgPrice),
                                 parseFloat(productData[i].ShortEMA),
                                 parseFloat(productData[i].LongEMA)
                                 //parseFloat(productData[i].MACD),
                                 //parseFloat(productData[i].SignalLine)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.visualization.LineChart(container[0]);

                var options =
                {
                    title: 'Moving averages'                    
                };

                chart.draw(data, options);
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadProductPricesCandlestickChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var productId = container.attr('data-product-id');
        var conditionId = container.attr('data-product-condition-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: apiURL + productId + '/' + conditionId,
            success: function (productData) {
                // Create the chart data from the API response                                
                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Date');
                data.addColumn('number', 'Open Price');
                data.addColumn('number', 'Min Price');
                data.addColumn('number', 'Max Price');
                data.addColumn('number', 'Close Price');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].representativeDate,
                                 parseFloat(productData[i].MinPrice),
                                 parseFloat(productData[i].OpenPrice),
                                 parseFloat(productData[i].ClosePrice),
                                 parseFloat(productData[i].MaxPrice)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.visualization.CandlestickChart(container[0]);

                var options = {
                    title: 'Daily price movements',
                    candlestick: {
                        fallingColor: { strokeWidth: 0, fill: '#a52714' }, // red
                        risingColor: { strokeWidth: 0, fill: '#0f9d58' }   // green
                    }
                }

                chart.draw(data,options);
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadProductPriceLineChart(chartName, apiURL) {
        var chartContainer = $('#' + chartName);
        var loader = chartContainer.find('#loader');
        var errorMessage = chartContainer.find('#errorMessage');
        var searchId = chartContainer.attr('data-product-id');
        var conditionId = chartContainer.attr('data-product-condition-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: apiURL + searchId + '/' + conditionId,
            success: function (chartsdata) {
                // Create the chart data from the API response
                var data = new google.visualization.DataTable();

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
                loader.hide();

                // Create and draw the chart
                var chart = new google.visualization.LineChart(chartContainer[0]);

                var options =
                {
                    title: 'Prices for the last 30 days',
                    curveType: 'function'
                };

                chart.draw(data, options);
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadProductScatterGraphChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var productId = container.attr('data-product-id');        

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: apiURL + productId,
            success: function (productData) {
                // Create the chart data from the API response                                
                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Time');
                //data.addColumn('string', 'Condition');
                data.addColumn('number', 'Price');                

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].EndTime,
                                //productData[i].Condition,
                                 parseFloat(productData[i].Price)
                    ]);
                }

                // Hide the loader
                loader.hide();

                var chart = new google.charts.Scatter(container[0]);
                

                var options = {
                    title: 'End time versus price'
                }

                chart.draw(data, google.charts.Scatter.convertOptions(options));
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function loadPopularProductsTable(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var categoryId = container.attr('data-category-id');
        var conditionId = container.attr('data-condition-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: applicationBaseURL() + apiURL + categoryId + '/' + conditionId,
            success: function (productData) {
                // Create the chart data from the API response
                var data = new google.visualization.DataTable();

                data.addColumn('number', '#');
                data.addColumn('string', 'Product');
                data.addColumn('number', 'Sold');
                data.addColumn('number', 'Av Price');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([(i + 1),
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

    function loadProductsTable(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var loader = container.find('#loader');
        var errorMessage = container.find('#errorMessage');
        var categoryId = container.attr('data-category-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: applicationBaseURL() + apiURL + categoryId,
            success: function (productData) {
                // Create the chart data from the API response
                var data = new google.visualization.DataTable();

                data.addColumn('string', '');
                data.addColumn('string', 'Name');
                data.addColumn('string', 'Year of release');                

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow(["<a href='/Product/" + productData[i].ProductId + "'>" + productData[i].ManufacturerCode + "</a>",
                                    "<a href='/Product/" + productData[i].ProductId + "'>" + productData[i].Name + "</a>",                                 
                                 productData[i].YearOfRelease
                    ]);
                }

                // Hide the loader
                loader.hide();

                var table = new google.visualization.Table(container[0]);

                table.draw(data, { showRowNumber: false, allowHtml: true, width: '100%', height: '100%' });
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    $(function () {
        google.charts.load('current', { 'packages': ['table', 'corechart', 'bar', 'scatter'] });


    })
