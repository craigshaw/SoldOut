(function () {
    var conditions = { 2: "New", 7: "Used" };

    // Courtesy of Jack Moore, http://www.jacklmoore.com/notes/rounding-in-javascript/
    function round(value, decimals) {
        return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
    }

    function loadPopularProducts(productContainer) {
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
                // Create a table from the response data
                var html = buildProductTableHTMLFrom(productData, conditionId);

                // Hide the loader
                loader.hide();

                // smash it in the DOM
                container.append(html);
            },
            error: function () {
                loader.hide();
                errorMessage.show();
            }
        });
    }

    function buildProductTableHTMLFrom(productData, conditionId)
    {
        var HTML = "<h3>Most Popular " + conditions[conditionId] + " Sets This Week</h3>";
        HTML += "<table class='table table-condensed'><thead><tr><th></th><th>Product</th><th># Sold</th><th>Av Price</th></tr></thead>";
        HTML += "<tbody>";

        for (var i = 0; i < productData.length; i++) {
            HTML += "<tr><td>" + (i + 1) + ".</td><td>";
            HTML += "<a href='/Product/" + productData[i].ProductId + "'>" + productData[i].ManufacturerCode + " " + productData[i].Name + "</a></td>";
            HTML += "<td>" + productData[i].ItemCount + "</td>";
            HTML += "<td>" + round(productData[i].AveragePrice, 2) + "</td></tr>";
            // TODO: Format the average price as the MVC version would (i.e. £24.10)
        }

        HTML += "</tbody></table>";

        return HTML;
    }

    $(function () {
        loadPopularProducts('newProductContainer');
        loadPopularProducts('usedProductContainer');
    })
})();