using SV21T1020105.DomainModels;

namespace SV21T1020105.Web.Models.SearchResults
{
    public class OrderSearchResult : PaginationSearchResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }
}
