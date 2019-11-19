using System;
using System.Collections.Generic;
using System.Text;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class Coupon : Discount
    {
        public Coupon(double amount, int quantity, Enums.DiscountType discountType)
        {
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
