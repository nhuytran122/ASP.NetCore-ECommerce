using SV21T1020105.DomainModels;

namespace SV21T1020105.Web.Models.SearchResults
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}