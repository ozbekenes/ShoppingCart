using System.Collections.Generic;
using System.Linq;
using Trendyol.ECommerce.ShoppingCart.Logic.Interfaces;
using System;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class RateCalculator : ICalculator
    {
        /// <summary>
        /// Calculate the campaign discount by giving discount type, products category and discount rate.
        /// </summary>
        /// <param name="shoppingCartDetails"></param>
        /// <param name="campaign"></param>
        /// <param name="campaignDiscount"></param>
        public void CalculateCampaignDiscount(List<ShoppingCartDetail> shoppingCartDetails, Campaign campaign, ref double discountValue)
        {
            var query = shoppingCartDetails.Sum(p => p.Product.Price * p.Quantity) * (campaign.DiscountParameter.Amount / 100);
            discountValue = query > discountValue ? query : discountValue;
        }

        /// <summary>
        /// Calculate the coupon discount by total amount of shopping cart and coupon rule.
        /// </summary>
        /// <param name="totalAmount"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public double CalculateCouponDiscount(double totalAmount, Coupon coupon)
        {
            return totalAmount * (coupon.DiscountParameter.Quantity / 100);
        }
    }
}
