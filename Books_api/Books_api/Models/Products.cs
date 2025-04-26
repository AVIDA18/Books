using System.ComponentModel.DataAnnotations;

namespace Books_api.Models
{
    public class ProductParameters
    {
        public int? ProductId { get; set; }

        public int? ParentProductId { get; set; }

        [Required]
        [MaxLength(150)]
        public string ProductName { get; set; }

        public IFormFile? ImageFile { get; set; }

        public byte[]? Image { get; set; }
        [MaxLength(256)]
        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public bool OnDiscount { get; set; } = false;

        public decimal? DiscountPrice { get; set; }

        public bool IsParentProduct { get; set; } = false;


        public bool Status { get; set; } = true;
    }


    public class Products
    {
        public int ProductId { get; set; }
        public int? ParentProductId { get; set; }
        public string? ParentProductName {  get; set; }
        public string ProductName { get; set; }
        public byte[]? Image { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public bool OnDiscount { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool IsParentProduct { get; set; }
        public bool Status { get; set; }
    }

    public class ProductCategories
    {
        public int ParentProductId { get; set; }
        public bool Status { get; set; }
        public string ParentProductName { get; set; }
    }

    public class ListProductCategoriesParameters
    {
        [Required]
        public bool Status { get; set; }
    }

    public class ListProductParameters
    {
        public int? ParentProductId { get; set; }
        public bool? Status { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    
    public class SearchAndSortProductParameters
    {
        [MaxLength(150)]
        public string? SearchTerm { get; set; }
        public int? ParentProductId { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        [MaxLength(50)]
        public string? SortBy { get; set; }
        [MaxLength(4)]
        public string? SortOrder { get; set; }
        public bool? Status { get; set; }
    }
}
