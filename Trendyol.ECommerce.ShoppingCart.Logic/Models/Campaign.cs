using Trendyol.ECommerce.ShoppingCart.Logic.Interfaces;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class Campaign : Discount
    {
        public Category Category { get; private set; }

        public Campaign(Category category, double amount, int quantity, Enums.DiscountType discountType)
        {
            Category = category;
            DiscountParameter = new DiscountParameter();
            SetDiscountParameter(amount, quantity, discountType);
        }

        private void SetDiscountParameter(double amount, int quantity, Enums.DiscountType discountType)
        {
            DiscountParameter.Amount = amount;
            DiscountParameter.Quantity = quantity;
            DiscountParameter.DiscountType = discountType;
        }
    }
}
