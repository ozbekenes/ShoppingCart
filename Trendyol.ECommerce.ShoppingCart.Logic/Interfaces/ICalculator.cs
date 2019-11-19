using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trendyol.ECommerce.ShoppingCart.Logic.Models;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Interfaces
{
    /// <summary>
    /// Implementation of calculating campaign and coupon discount by using FACTORY PATTERN.   
    /// </summary>
    public interface ICalculator
    {
        /// <summary>
        /// Calculate the campaign discount by giving discount type, products category and discount rule.
        /// </summary>
        /// <param name="shoppingCartDetails"></param>
        /// <param name="campaign"></param>
        /// <param name="campaignDiscount"></param>
        void CalculateCampaignDiscount(List<ShoppingCartDetail> shoppingCartDetails, Campaign campaign, ref double campaignDiscount);

        /// <summary>
        /// Calculate the coupon discount by total amount of shopping cart and coupon rule.
        /// </summary>
        /// <param name="totalAmount"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        double CalculateCouponDiscount(double totalAmount, Coupon coupon);
    }
}
