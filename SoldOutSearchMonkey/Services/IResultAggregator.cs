namespace SoldOutSearchMonkey.Services
{
    internal interface IResultAggregator
    {
        void Start();
        void Stop();
        void Add(SearchSummary searchSummary);
    }
}
