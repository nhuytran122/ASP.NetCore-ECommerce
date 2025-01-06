using SV21T1020105.DomainModels;

namespace SV21T1020105.Shop.Models
{
    public class ProductDetailModel
    {
        public Product? Product { get; set; }
        public required List<ProductAttribute> Attributes { get; set; }
        public required List<ProductPhoto> Photos { get; set; }
        public required List<Product> SimilarProducts { get; set; }
        public string CategoryName { get; set; } = "";
    }
}
