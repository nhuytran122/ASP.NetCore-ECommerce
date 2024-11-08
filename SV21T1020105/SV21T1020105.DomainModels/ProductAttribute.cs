using System.ComponentModel.DataAnnotations;

namespace SV21T1020105.DomainModels
{
    public class ProductAttribute
    {
        public long AttributeID { get; set; }
        public int ProductID { get; set; }
        public string AttributeName { get; set; } = "";
        public string AttributeValue { get; set; } = "";
        [Required(ErrorMessage = "Vui lòng thứ tự hiển thị của thuộc tính")]
        public int? DisplayOrder { get; set; }
    }
}
