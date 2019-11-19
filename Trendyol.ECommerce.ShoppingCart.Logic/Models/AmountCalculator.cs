using System.Collections.Generic;
using Trendyol.ECommerce.ShoppingCart.Logic.Interfaces;
using System.Linq;
namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class AmountCalculator : ICalculator
    {
        /// <summary>
        /// Calculate the campaign discount by giving discount type, products category and discount amount.
        /// </summary>
        /// <param name="shoppingCartDetails"></param>
        /// <param name="campaign"></param>
        /// <param name="campaignDiscount"></param>
        public void CalculateCampaignDiscount(List<ShoppingCartDetail> shoppingCartDetails, Campaign campaign, ref double discountValue)
        {
            discountValue = shoppingCartDetails.Sum(p=>p.Quantity) > 1 && campaign.DiscountParameter.Amount >= discountValue ? campaign.DiscountParameter.Amount : discountValue;
        }

        /// <summary>
        /// Calculate the coupon discount by total amount of shopping cart and coupon rule.
        /// </summary>
        /// <param name="totalAmount"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public double CalculateCouponDiscount(double totalAmount, Coupon campaign)
        {
            return campaign.DiscountParameter.Quantity;
        }
    }
}