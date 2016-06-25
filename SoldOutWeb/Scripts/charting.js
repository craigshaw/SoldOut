var charting = (function ($) {
    // Will store chart data for each chart loaded so we can easily redraw, etc
    var chartCache = {};

    function load() {
        google.charts.load('current', { 'packages': ['table', 'corechart', 'bar', 'scatter'] });
    }

    function setOnLoadCallback(callback) {
        google.charts.setOnLoadCallback(callback);
    }

    // Courtesy of Jack Moore, http://www.jacklmoore.com/notes/rounding-in-javascript/
    function round(value, decimals) {
        return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
    }

    function formatDate(value) {
        var myDate = Date.parse(value);
        return myDate.getDate() + "-" + myDate.getMonth() + "-" + myDate.getFullYear();
    }

    function applicationBaseURL() {
        // Work out the base URL to pass into the charting function so they can use the general charting API
        pathArray = location.href.split('/');
        protocol = pathArray[0];
        host = pathArray[2];
        url = protocol + '//' + host;

        return url;
    }

    function loadSalesByWeekdayAreaChart(productContainer, apiURL, chartName, dataItemAttributeName) {
        var container = $('#' + productContainer);
        var dataItemId = container.attr(dataItemAttributeName)
        var conditionId = container.attr('data-condition-id');

        $.ajax({ 
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: applicationBaseURL() + apiURL + dataItemId + '/' + conditionId,
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

                var chart = new google.visualization.AreaChart(container[0]);

                var options = {
                    height: '100%',
                    width: '100%',
                    chartArea: {
                        height: '75%',
                        width: '85%',
                    },
                    legend: {
                        alignment: 'center',
                        position: 'in'
                    },
                    title: 'No. of Bidders Vs. No. of Sales',
                    colors: ['#ff9900', '#28d72d'],                    
                };

                chart.draw(data, options);

                // Add the loaded chart data to the cache
                chartCache[chartName] = { chart: chart, data: data, options: options };
            },
            error: function () {
            }
        });
    }

    function loadTopSellersPieChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var categoryId = container.attr('data-category-id') || '';

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

                var chart = new google.visualization.PieChart(container[0]);

                var options = {
                    title: 'Best selling categories over the last 30 days',
                    is3D: true,
                    height: '100%',
                    width: '100%',
                    chartArea: {
                        height: '85%',
                        width: '85%',
                    },
                }

                chart.draw(data, options);

                google.visualization.events.addListener(chart, 'select', function () {
                    var selection = chart.getSelection();

                    var cid = productData[selection[0].row * 2].CategoryId;

                    window.location.href = "/Category/" + cid;
                });
            },
            error: function () {
                // ?
            }
        });
    }

    function loadMoversAndLosersBarChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
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

                var chart = new google.visualization.ColumnChart(container[0]);

                var minhValue = productData[productData.length-1].PercentPriceChange;
                var maxhValue = productData[0].PercentPriceChange;

                var options = {
                    chart: {
                        title: 'Movers and losers over the last 30 days',
                        isStacked: true,
                        colors: ['#00b300', '#ff3300']
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

                    var pid = productData[(selection[0].row)].ProductId;                    

                    window.location.href = "/Product/" + pid;
                });
            },
            error: function () {
                // ?
            }
        });
    }

    function loadTopSellersBarChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var categoryId = container.attr('data-category-id') || '';

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

                var chart = new google.visualization.ColumnChart(container[0]);

                var options = {                    
                    title: 'Best selling products over the last 30 days',
                    height: '100%',
                    width: '100%',
                    chartArea: {
                        height: '80%',
                        width: '85%',
                    },
                    legend: {
                        position: 'in',
                        alignment: 'center',
                    },
                    hAxis: {
                        title: 'Product (New & Used)',
                        textStyle: {
                            fontSize: 12,
                        },
                    },
                    vAxis: {
                        textStyle: {
                            fontSize: 12,
                        },
                        title: 'Number of Sales',
                    },
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
                // ??
            }
        });
    }

    function loadProductPricesForMACDBarChart(productContainer, apiURL, chartName) {
        var container = $('#' + productContainer);
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
                data.addColumn('number', 'Histogram');
                data.addColumn('number', 'MACD');
                data.addColumn('number', 'Signal line');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].representativeDate,
                                 parseFloat(productData[i].HistogramData),
                                 parseFloat(productData[i].MACD),
                                 parseFloat(productData[i].SignalLine)
                    ]);
                }

                var chart = new google.visualization.ColumnChart(container[0]);

                var options = {
                    isStacked: true,
                    colors: ['#3333ff', '#ff3300'],
                    seriesType: 'bars',
                    series: {
                        1: { type: 'line', curveType: 'function' },
                        2: { type: 'line', curveType: 'function' }
                    }
                }

                chart.draw(data, options);

                // Add the loaded chart data to the cache
                chartCache[chartName] = { chart: chart, data: data, options: options };
            },
            error: function () {
            }
        });
    }

    function loadProductPricesMACD(productContainer, apiURL, chartName) {
        var container = $('#' + productContainer);
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
                data.addColumn('number', 'Lower Bollinger Band');
                data.addColumn('number', 'Upper Bollinger Band');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].representativeDate,
                                 parseFloat(productData[i].AvgPrice),
                                 parseFloat(productData[i].LowerBand),
                                 parseFloat(productData[i].UpperBand)
                    ]);
                }

                var chart = new google.visualization.LineChart(container[0]);

                var options =
                {
                    title: 'Daily Average Price',
                    legend: {
                        position: 'top',
                        alignment: 'center',
                    },
                    series: {
                        1: { type: 'line', curveType: 'function' },
                        2: { type: 'line', curveType: 'function' },
                    }
                };

                chart.draw(data, options);

                // Add the loaded chart data to the cache
                chartCache[chartName] = { chart: chart, data: data, options: options };
            },
            error: function () {
            }
        });
    }

    function loadProductPricesCandlestickChart(productContainer, apiURL, chartName) {
        var container = $('#' + productContainer);
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
                data.addColumn('number', 'Min. Price');
                data.addColumn('number', 'Open Price');
                data.addColumn('number', 'Max. Price');
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

                var chart = new google.visualization.CandlestickChart(container[0]);

                var options = {
                    title: 'Daily price movements',
                    candlestick: {
                        fallingColor: { strokeWidth: 0, fill: '#a52714' }, // red
                        risingColor: { strokeWidth: 0, fill: '#0f9d58' }   // green
                    }
                }

                chart.draw(data, options);

                // Add the loaded chart data to the cache
                chartCache[chartName] = { chart: chart, data: data, options: options };
            },
            error: function () {
            }
        });
    }

    function loadProductPriceLineChart(containerName, apiURL, chartName) {
        var chartContainer = $('#' + containerName);
        var productId = chartContainer.attr('data-product-id');
        var conditionId = chartContainer.attr('data-product-condition-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: apiURL + productId + '/' + conditionId,
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

                // Create and draw the chart
                var chart = new google.visualization.LineChart(chartContainer[0]);

                var options =
                {
                    title: 'Monthly price summary',
                    curveType: 'function',
                    chartArea: { width: '90%', height: '63%' },
                    legend: {
                        alignment: 'center',
                        position: 'top',
                        },
                    hAxis: {
                        textStyle: {
                            fontSize: 12,
                        },
                    },
                    vAxis: {
                        textStyle: {
                            fontSize: 12,         
                        },
                    },                                   
                };

                chart.draw(data, options);

                // Add the loaded chart data to the cache
                chartCache[chartName] = { chart: chart, data: data, options: options };
            },
            error: function () {
            }
        });
    }

    function loadSellersByCategoryLineChart(chartName, apiURL) {
        var chartContainer = $('#' + chartName);
        var categoryId = chartContainer.attr('data-category-id');
        var interval = chartContainer.attr('data-interval');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: apiURL + categoryId + '/' + interval,
            success: function (chartsdata) {
                // Create the chart data from the API response
                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Date');                
                //data.addColumn('string', 'Name');
                data.addColumn('number', '# Bidders');
                //data.addColumn({ type: 'string', role: 'domain', label: 'Name' });
                //data.addColumn('number', 'Avg. Price');                

                for (var i = 0; i < chartsdata.length; i++) {
                    data.addRow([//chartsdata[i].AsOfDate,                                
                                chartsdata[i].ManufacturerCode,
                                 round(chartsdata[i].NumberOfBidders, 2),
                                 
                                 //round(chartsdata[i].AvgPrice, 2)
                    ]);
                }

                // Create and draw the chart
                var chart = new google.visualization.LineChart(chartContainer[0]);

                var options =
                {
                    title: 'Top 5 sellers by # of bidders for the last ' + interval + 'days',
                    curveType: 'function'
                };

                chart.draw(data, options);
            },
            error: function () {
            }
        });
    }

    function loadDailyProductPriceLineChart(containerName, apiURL, chartName) {
        var chartContainer = $('#' + containerName);
        var searchId = chartContainer.attr('data-product-id');
        var conditionId = chartContainer.attr('data-product-condition-id');
        var shortInterval = chartContainer.attr('data-short-interval');
        var longInterval = chartContainer.attr('data-long-interval');
        var daysToLookBack = chartContainer.attr('data-daysToLookBack');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: apiURL + searchId + '/' + conditionId + '/' + shortInterval + '/' + longInterval + '/' + daysToLookBack,
            success: function (chartsdata) {
                // Create the chart data from the API response
                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Date');
                data.addColumn('number', 'Avg Price');
                data.addColumn('number', 'Lower Bollinger Band');
                data.addColumn('number', 'Upper Bollinger Band');


                // Create a table from the response data                                 
                for (var i = 0; i < chartsdata.length; i++) {
                    data.addRow([chartsdata[i].RepresentativeDate,
                                 round(chartsdata[i].AvgPrice, 2),
                                 parseFloat(chartsdata[i].LowerBand),
                                 parseFloat(chartsdata[i].UpperBand)
                    ]);
                }

                var chart = new google.visualization.ComboChart(chartContainer[0]);

                var options = {
                    title: 'Daily average price for the last ' + daysToLookBack + ' days',
                    titlePosition: 'out',
                    chartArea: { width: '90%', height: '63%' },
                    legend: {
                        alignment: 'center',
                        position: 'top',
                    },
                    hAxis: {
                        slantedText: true,
                        slantedTextAngle: 30,
                        textStyle: {
                            fontSize: 12,
                        },
                    },
                    vAxis: {
                        textStyle: {
                            fontSize: 12,         
                        },
                    },
                    isStacked: true,
                    series: {
                        0: { color: '#0066ff' },
                        1: { type: 'line', curveType: 'function', color: '#009900' },
                        2: { type: 'line', curveType: 'function', color: '#ff3300' },
                    }
                }

                chart.draw(data, options);

                // Add the loaded chart data to the cache
                chartCache[chartName] = { chart: chart, data: data, options: options };
            },
            error: function () {
            }
        });
    }

    function loadProductScatterGraphChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
        var productId = container.attr('data-product-id');

        $.ajax({
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            url: apiURL + productId,
            success: function (productData) {
                // Create the chart data from the API response                                
                var data = new google.visualization.DataTable();

                data.addColumn('string', 'Sale time (to the nearest 15 minute interval)');
                data.addColumn('number', 'New');
                data.addColumn('number', 'Used');                

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].EndTime,
                                parseFloat(productData[i].NewPrice),
                                 parseFloat(productData[i].UsedPrice)
                    ]);
                }

                var chart = new google.charts.Scatter(container[0]);

                var options = {
                    title: 'End time versus price',                    
                    titleTextStyle: {
                        fontName: 'Arial',
                        fontSize: 14,
                        bold: true,
                        color: 'Black',
                    },
                    colors: ['#ff66cc', '#6600ff'],
                    height: '100%',
                    width: '100%',
                    chartArea: {
                        width: '90%',
                        height: '63%'
                    },                    
                    legend: {
                        alignment: 'center',
                        position: 'top',
                    },                    
                    hAxis: {
                        textStyle: {
                            fontSize: 12,
                        },                        
                    },
                    vAxis: {
                        textStyle: {
                            fontSize: 12,
                        },                        
                    },
                }

                chart.draw(data, google.charts.Scatter.convertOptions(options));
            },
            error: function () {
            }
        });
    }

    function loadPopularProductsTable(productContainer, apiURL) {
        var container = $('#' + productContainer);
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

                var table = new google.visualization.Table(container[0]);

                var formatter = new google.visualization.NumberFormat({ prefix: '£' });
                formatter.format(data, 3); // Format price data correctly

                table.draw(data, { showRowNumber: false, allowHtml: true, width: '100%', height: '100%' });
            },
            error: function () {
            }
        });
    }

    function loadProductsTable(productContainer, apiURL) {
        var container = $('#' + productContainer);
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

                var table = new google.visualization.Table(container[0]);

                table.draw(data, { showRowNumber: false, allowHtml: true, width: '100%', height: '100%' });
            },
            error: function () {
            }
        });
    }

    function loadMoversAndLosersBarChart(productContainer, apiURL, chartName) {
        var container = $('#' + productContainer);
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
                data.addColumn('number', '% Price change');

                // Create a table from the response data
                for (var i = 0; i < productData.length; i++) {
                    data.addRow([productData[i].ManufacturerCode.concat(' ', productData[i].Name),
                                    parseInt(productData[i].PlusPriceChange),
                                    parseInt(productData[i].NegativePriceChange)
                    ]);
                }

                var chart = new google.visualization.ColumnChart(container[0]);

                var minhValue = productData[productData.length - 1].PercentPriceChange;
                var maxhValue = productData[0].PercentPriceChange;

                var options = {
                    chartArea: { width: '85%', height: '63%' },
                    title: 'Movers and losers over the last 30 days',
                    //chart: {
                        isStacked: true,
                        legend: { position: 'none' },
                    //},
                    hAxis: {
                        textPosition: 'none',
                        title: 'Price change (hightest to lowest)',
                        textStyle: {
                            fontSize: 8,
                        },
                    },
                    vAxis: {                        
                        title: '% price change',
                        baseline: 0,
                        minValue: minhValue,
                        maxValue: maxhValue,
                        textStyle: {
                          fontSize: 12,
                        },
                    },
                    series: {
                        0: { color: '#009900' },
                        1: { color: '#ff3300' },
                    },
                }

                chart.draw(data, options);

                google.visualization.events.addListener(chart, 'select', function () {
                    var selection = chart.getSelection();

                    var pid = productData[(selection[0].row)].ProductId;

                    window.location.href = "/Product/" + pid;
                });

                // Add the loaded chart data to the cache
                chartCache[chartName] = { chart: chart, data: data, options: options };
            },
            error: function () {
            }
        });
    }


    function loadSalesByWeekdayBarChart(productContainer, apiURL) {
        var container = $('#' + productContainer);
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

                var chart = new google.visualization.AreaChart(container[0]);

                var options = {
                    title: '',
                    legend: { alignment: 'center', position: 'in' },
                }

                chart.draw(data, options);
            },
            error: function () {
            }
        });
    }

    function redrawChart(chartName) {
        // Get the chart data for the given chart, then redraw
        if (!!chartCache[chartName]) {
            var chartData = chartCache[chartName];
            chartData.chart.draw(chartData.data, chartData.options);
        }
    }

    return {
        load: load,
        setOnLoadCallback: setOnLoadCallback,
        loadMoversAndLosersBarChart: loadMoversAndLosersBarChart,
        loadProductsTable: loadProductsTable,
        loadPopularProductsTable: loadPopularProductsTable,
        loadProductScatterGraphChart: loadProductScatterGraphChart,
        loadProductPriceLineChart: loadProductPriceLineChart,
        loadProductPricesCandlestickChart: loadProductPricesCandlestickChart,
        loadProductPricesMACD: loadProductPricesMACD,
        loadProductPricesForMACDBarChart: loadProductPricesForMACDBarChart,
        loadTopSellersBarChart: loadTopSellersBarChart,
        loadTopSellersPieChart: loadTopSellersPieChart,
        loadSalesByWeekdayAreaChart: loadSalesByWeekdayAreaChart,
        loadDailyProductPriceLineChart: loadDailyProductPriceLineChart,
        redrawChart: redrawChart
    };
})(jQuery);
