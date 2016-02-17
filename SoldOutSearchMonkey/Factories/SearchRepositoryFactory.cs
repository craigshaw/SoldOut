using SoldOutBusiness.Repository;

namespace SoldOutSearchMonkey.Factories
{
    internal class SearchRepositoryFactory : ISearchRepositoryFactory
    {
        public ISearchRepository CreateSearchRepository()
        {
            return new SearchRepository();
        }
    }
}
