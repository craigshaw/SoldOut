using SoldOutBusiness.Repository;

namespace SoldOutSearchMonkey.Factories
{
    internal class SearchRepositoryFactory : ISearchRepositoryFactory
    {
        public ISoldOutRepository CreateSearchRepository()
        {
            return new SoldOutRepository();
        }
    }
}
