using SV21T1020105.DomainModels;

namespace SV21T1020105.Web.Models.SearchResults
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}