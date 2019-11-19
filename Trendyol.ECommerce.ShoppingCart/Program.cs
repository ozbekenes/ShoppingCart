using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trendyol.ECommerce.ShoppingCart.Logic;
using Trendyol.ECommerce.ShoppingCart.Logic.Interfaces;
using Trendyol.ECommerce.ShoppingCart.Logic.Models;
using static Trendyol.ECommerce.ShoppingCart.Logic.Models.Enums;

namespace Trendyol.ECommerce.ShoppingCart
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Category definition
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var computer = new Category("Computer");
            #endregion

            #region product definition
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var lg = new Product("LG-5", 200, smartPhone);
            var thinkPad = new Product("ThinkPad", 200, computer);
            #endregion

            #region shopping cart & delivery cost implementation
            var costPerDelivery = 2.0;
            var costPerProduct = 5.0;
            var deliveryCostCalculator = new DeliveryCostCalculator(costPerDelivery, costPerProduct);
            IShoppingCart cart = new Logic.Models.ShoppingCart(deliveryCostCalculator);
            cart.AddItem(iPhone, 1);
            cart.AddItem(iPhone, 1);
            cart.AddItem(samsung, 4);
            cart.AddItem(lg, 1);
            cart.AddItem(thinkPad, 2);
            #endregion

            #region campaign definition & implementation
            Campaign campaign1 = new Campaign(smartPhone, 20, 3, DiscountType.Rate);
            Campaign campaign2 = new Campaign(smartPhone, 40, 5, DiscountType.Rate);
            Campaign campaign3 = new Campaign(smartPhone, 50, 2, DiscountType.Amount);
            Campaign campaign4 = new Campaign(computer, 20, 2, DiscountType.Rate);
            cart.ApplyDiscounts(campaign1, campaign2, campaign3, campaign4);
            #endregion

            #region coupon definition & implementation
            Coupon coupon = new Coupon(1000, 150, DiscountType.Amount);
            cart.ApplyCoupon(coupon);
            #endregion

            #region print the result
            System.Console.WriteLine(cart.Print());
            #endregion
        }
    }
}
