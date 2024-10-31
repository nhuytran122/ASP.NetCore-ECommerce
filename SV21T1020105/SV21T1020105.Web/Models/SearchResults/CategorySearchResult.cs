using SV21T1020105.DomainModels;

namespace SV21T1020105.Web.Models.SearchResults
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}