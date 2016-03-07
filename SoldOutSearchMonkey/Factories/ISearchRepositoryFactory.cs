using SoldOutBusiness.Repository;

namespace SoldOutSearchMonkey.Factories
{
    internal interface ISearchRepositoryFactory
    {
        ISoldOutRepository CreateSearchRepository(); 
    }
}
