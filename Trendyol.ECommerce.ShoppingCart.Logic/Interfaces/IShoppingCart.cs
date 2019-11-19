using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trendyol.ECommerce.ShoppingCart.Logic.Models;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Interfaces
{
    /// <summary>
    /// This interface created for NSubstitute mocking framework restrictions.
    /// </summary>
    public interface IShoppingCart
    {
        int GetNumberOfDeliveries();
        int GetNumberOfProducts();
        void AddItem(Product product, int amount);
        void ApplyDiscounts(params Campaign[] campaigns);
        void ApplyCoupon(Coupon coupon);
        string Print();
    }
}
