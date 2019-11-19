namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class Category
    {
        public string Title { get; set; }
        public Category ParentCategory { get; set; }

        public Category(string title, Category parentCategory = null)
        {
            Title = title;
            ParentCategory = parentCategory;
        }
    }
}
