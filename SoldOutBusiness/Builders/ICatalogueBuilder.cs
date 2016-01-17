using SoldOutBusiness.Models;
using System.Collections.Generic;

namespace SoldOutBusiness.Builders
{
    public interface ICatalogueBuilder
    {
        ICatalogueBuilder AddFromConfigFile(string filePath);
        ICatalogueBuilder Add(IList<Search> searches);
        Catalogue Build();
    }
}
