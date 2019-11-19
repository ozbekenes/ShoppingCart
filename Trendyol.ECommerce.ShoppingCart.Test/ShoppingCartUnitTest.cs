using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FluentAssertions;
using Trendyol.ECommerce.ShoppingCart.Logic.Models;
using System.Diagnostics;
using System.Linq;
namespace Trendyol.ECommerce.ShoppingCart.Test
{
    [TestClass]
    public class ShoppingCartUnitTest
    {
        Logic.Models.ShoppingCart shoppingCart;
        readonly DeliveryCostCalculator deliveryCostCalculator = Substitute.For<DeliveryCostCalculator>(2.0, 5.0, 2.99);
        public ShoppingCartUnitTest()
        {
            shoppingCart = new Logic.Models.ShoppingCart(deliveryCostCalculator);
        }

        #region tests addding product to shopping cart
        /// <summary>
        /// Method tests adding product to shopping cart with null product.
        /// </summary>
        [TestMethod]
        public void AddProduct_WithNullProduct_ShouldNotBeAdded()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            Product iPhone = null;

            //Act
            shoppingCart.AddItem(iPhone, 1);

            //Assert
            shoppingCart.ShoppingCartDetail.Sum(p => p.Quantity).Should().Be(0);
        }

        /// <summary>
        /// Method tests adding product to shopping cart with null product and less than or equal to zero quantity.
        /// </summary>
        [TestMethod]
        public void AddProduct_WithNullProductAndLessThanOrEqualToZeroQuantity_ShouldNotBeAdded()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            Product iPhone = null;

            //Act
            shoppingCart.AddItem(iPhone, -1);

            //Assert
            shoppingCart.ShoppingCartDetail.Sum(p => p.Quantity).Should().Be(0);
        }

        /// <summary>
        /// Method tests adding product to shopping cart with less than or equal to zero quantity.
        /// </summary>
        [TestMethod]
        public void AddProduct_WithLessThanOrEqualToZeroQuantity_ShouldBeAdded()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);

            //Act
            shoppingCart.AddItem(iPhone, -1);

            //Assert
            shoppingCart.ShoppingCartDetail.Sum(p => p.Quantity).Should().Be(0);
        }

        /// <summary>
        /// Method tests adding product to shopping cart with valid quantity within 100 miliseconds interval.
        /// </summary>
        [TestMethod]
        public void AddProduct_WithValidQuantity_ShouldBeAdded()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);

            //Act
            shoppingCart.AddItem(iPhone, 3);

            //Assert
            shoppingCart.ShoppingCartDetail.Sum(p => p.Quantity).Should().Be(3);
        }

        /// <summary>
        /// Method tests adding product to shopping cart with valid quantity within 100 miliseconds.
        /// This can be tested by sleep thread in AddItems method.
        /// </summary>
        [TestMethod]
        public void AddProduct_WithValidQuantityAndIn100MilisecondsTimeInterval_ShouldBeAdded()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            sw.Stop();

            //Assert
            sw.ElapsedMilliseconds.Should().BeLessOrEqualTo(100, "No response was received in the requested time." + sw.ElapsedMilliseconds);
            shoppingCart.ShoppingCartDetail.Sum(p => p.Quantity).Should().Be(3);
        }

        #endregion

        #region tests calculating campaign discount
        /// <summary>
        /// Calculate total campaign discounts with null campaign parameters.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithNullCampaignParams_ReturnZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts();

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(0);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and one rate discount type and less than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithRateDiscountTypeAndLessThanMinimumQuantity_ReturnZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 5, Enums.DiscountType.Rate);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(0);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and one rate discount type and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithRateDiscountTypeAndGreaterThanEqualToMinimumQuantity_ReturnSixty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(60);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and multiple rate discount type and both greater than equal to minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithMultipleRateDiscountTypeAndBothGreaterThanEqualToMinimumQuantity_ReturnTwoHundred()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var campaign2 = new Campaign(phone, 40, 4, Enums.DiscountType.Rate);

            //Act
            shoppingCart.AddItem(iPhone, 5);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(200);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and multiple rate discount type and only one greater than equal to minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithMultipleRateDiscountTypeAndOnlyOneGreaterThanEqualToMinimumQuantity_ReturnSixty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var campaign2 = new Campaign(phone, 40, 6, Enums.DiscountType.Rate);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(60);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and one amount discount type and less than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithAmountDiscountTypeAndLessThanMinimumQuantity_ReturnZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);

            //Act
            shoppingCart.AddItem(iPhone, 1);
            shoppingCart.ApplyDiscounts(campaign);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(0);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and one amount discount type and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithAmountDiscountTypeAndGreaterThanEqualToMinimumQuantity_ReturnTwenty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(20);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and multiple amount discount type and both greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithMultipleAmountDiscountTypeAndBothGreaterThanEqualToMinimumQuantity_ReturnForty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);
            var campaign2 = new Campaign(phone, 40, 4, Enums.DiscountType.Amount);

            //Act
            shoppingCart.AddItem(iPhone, 5);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(40);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and multiple amount discount type and only one greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithMultipleAmountDiscountTypeAndOnlyOneGreaterThanEqualToMinimumQuantity_ReturnTwenty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);
            var campaign2 = new Campaign(phone, 40, 6, Enums.DiscountType.Amount);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(20);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and one rate discount type and one amount discount type and rate discount greater than amount discount.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithRateDiscountAndAmountDiscountAndRateDiscountGreaterThanAmountDiscount_ReturnHundred()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var campaign2 = new Campaign(phone, 40, 4, Enums.DiscountType.Amount);

            //Act
            shoppingCart.AddItem(iPhone, 5);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(100);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, one product, and one rate discount type and one amount discount type and amount discount greater than rate discount.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndOneProductAndWithRateDiscountAndAmountDiscountAndAmountGreaterThanEqualToMinimumQuantity_ReturnHundredTwenty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var campaign2 = new Campaign(phone, 120, 4, Enums.DiscountType.Amount);

            //Act
            shoppingCart.AddItem(iPhone, 5);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(120);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, multiple product, and one rate discount type and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndMultipleProductAndWithRateDiscountTypeAndGreaterThanEqualToMinimumQuantity_ReturnOneHundredSixty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);

            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.AddItem(samsung, 4);
            shoppingCart.ApplyDiscounts(campaign);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(160);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, multiple product, and one amount discount type and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndMultipleProductAndWithAmountDiscountTypeAndGreaterThanEqualToMinimumQuantity_ReturnTwenty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.AddItem(samsung, 4);
            shoppingCart.ApplyDiscounts(campaign);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(20);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, multiple product, and multiple rate discount type and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndMultipleProductAndWithMultipleRateDiscountTypeAndGreaterThanEqualToMinimumQuantity_ReturnThreeHundredTwenty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var campaign2 = new Campaign(phone, 40, 4, Enums.DiscountType.Rate);

            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.AddItem(samsung, 4);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(320);
        }

        /// <summary>
        /// Calculate total campaign discounts with one category, multiple product, and multiple amount discount type and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithOneCategoryAndMultipleProductAndWithMultipleAmountDiscountTypeAndGreaterThanEqualToMinimumQuantity_ReturnFifty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);
            var campaign2 = new Campaign(phone, 50, 4, Enums.DiscountType.Amount);

            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.AddItem(samsung, 4);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(50);
        }

        /// <summary>
        /// Calculate total campaign discounts with multiple category, multiple product, multiple rate discount type and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithMultipleCategoryAndMultipleProductAndWithMultipleRateDiscountTypeForOneCategoryAndGreaterThanEqualToMinimumQuantity_ReturnThreeHundredTwenty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var computer = new Category("Computer");
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var thinkPad = new Product("ThinkPad", 200, computer);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var campaign2 = new Campaign(phone, 40, 4, Enums.DiscountType.Rate);

            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.AddItem(samsung, 4);
            shoppingCart.AddItem(thinkPad, 2);
            shoppingCart.ApplyDiscounts(campaign1, campaign2);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(320);
        }

        /// <summary>
        /// Calculate total campaign discounts with multiple category, multiple product, multiple rate discount type both of the category and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithMultipleCategoryAndMultipleProductAndWithMultipleRateDiscountTypeForMultipleCategoryAndGreaterThanEqualToMinimumQuantity_ReturnFourHundred()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var computer = new Category("Computer");
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var thinkPad = new Product("ThinkPad", 200, computer);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var campaign2 = new Campaign(phone, 40, 4, Enums.DiscountType.Rate);
            var campaign3 = new Campaign(computer, 20, 2, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.AddItem(samsung, 4);
            shoppingCart.AddItem(thinkPad, 2);
            shoppingCart.ApplyDiscounts(campaign1, campaign2, campaign3);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(400);
        }

        /// <summary>
        /// Calculate total campaign discounts with multiple category, multiple product, multiple rate discount type both of the category and greater than minimum campaign quantity.
        /// </summary>
        [TestMethod]
        public void CalculateCampaignDiscount_WithMultipleCategoryAndMultipleProductAndWithMixedDiscountTypeForMultipleCategoryAndAmountDiscountGreaterThanRateDiscountType_ReturnTwoHundredEighty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var computer = new Category("Computer");
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var thinkPad = new Product("ThinkPad", 200, computer);
            var campaign1 = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var campaign2 = new Campaign(phone, 200, 4, Enums.DiscountType.Amount);
            var campaign3 = new Campaign(computer, 20, 2, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.AddItem(samsung, 4);
            shoppingCart.AddItem(thinkPad, 2);
            shoppingCart.ApplyDiscounts(campaign1, campaign2, campaign3);

            //Assert
            shoppingCart.GetCampaignDiscount().Should().Be(280);
        }

        #endregion

        #region tests calculating coupon discount.

        /// <summary>
        /// Calculate coupon discount with null coupon.
        /// </summary>
        [TestMethod]
        public void CalculateCouponDiscount_WithNullCoupon_ReturnsZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            Coupon coupon = null;
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.ApplyCoupon(coupon);

            //Assert
            shoppingCart.GetCouponDiscount().Should().Be(0);
        }

        /// <summary>
        /// Calculate coupon discount with total amoun of shopping cart greater than coupon minimum amount with rate discount type.
        /// </summary>
        [TestMethod]
        public void CalculateCouponDiscount_WithTotalPriceGreaterThanMinimumAmountWithRateDiscountType_ReturnsZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var coupon = new Coupon(300, 10, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.ApplyCoupon(coupon);

            //Assert
            shoppingCart.GetCouponDiscount().Should().Be(0);
        }

        /// <summary>
        /// Calculate coupon discount with total amoun of shopping cart greater than coupon minimum amount with amount discount type.
        /// </summary>
        [TestMethod]
        public void CalculateCouponDiscount_WithTotalPriceGreaterThanMinimumAmountWithAmountDiscountType_ReturnsZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var coupon = new Coupon(300, 10, Enums.DiscountType.Amount);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.ApplyCoupon(coupon);

            //Assert
            shoppingCart.GetCouponDiscount().Should().Be(0);
        }

        /// <summary>
        /// Calculate coupon discount with coupon minimum amount greater than total amoun of shopping cart with rate discount type.
        /// </summary>
        [TestMethod]
        public void CalculateCouponDiscount_WithTotalPriceGreaterThanMinimumAmountWithRateDiscountType_ReturnsTwenty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.ApplyCoupon(coupon);

            //Assert
            shoppingCart.GetCouponDiscount().Should().Be(20);
        }


        /// <summary>
        /// Calculate coupon discount with coupon minimum amount greater than total amoun of shopping cart with amount discount type.
        /// </summary>
        [TestMethod]
        public void CalculateCouponDiscount_WithTotalPriceGreaterThanMinimumAmountWithAmountDiscountType_ReturnsTen()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Amount);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.ApplyCoupon(coupon);

            //Assert
            shoppingCart.GetCouponDiscount().Should().Be(10);
        }

        /// <summary>
        /// Calculate coupon discount with coupon minimum amount greater than total amoun of shopping cart with amount discount type and multiple product in the cart.
        /// </summary>
        [TestMethod]
        public void CalculateCouponDiscount_WithTotalPriceGreaterThanMinimumAmountWithAmountDiscountTypeAndMultipleProduct_ReturnsTen()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Amount);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.AddItem(samsung, 2);
            shoppingCart.ApplyCoupon(coupon);

            //Assert
            shoppingCart.GetCouponDiscount().Should().Be(10);
        }

        /// <summary>
        /// Calculate coupon discount with coupon minimum amount greater than total amoun of shopping cart with amount discount type and multiple product in the cart.
        /// </summary>
        [TestMethod]
        public void CalculateCouponDiscount_WithTotalPriceGreaterThanMinimumAmountWithRateDiscountTypeAndMultipleProduct_ReturnsFifty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.AddItem(samsung, 2);
            shoppingCart.ApplyCoupon(coupon);

            //Assert
            shoppingCart.GetCouponDiscount().Should().Be(50);
        }
        #endregion

        #region tests calculating total (campaign&coupon) discount amounts.

        /// <summary>
        /// Calculate total (campaign&coupon) discount amount with no product in the shopping cart.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithNoProductInShoppingCart_ReturnsZero()
        {
            //Arrange

            //Act
            
            //Assert
            shoppingCart.GetTotalAmountAfterDiscounts().Should().Be(0);
        }

        /// <summary>
        /// Calculate total (campaign&coupon) discount amount without any discount for shopping cart.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithoutAnyDiscount_ReturnsZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var samsung = new Product("Samsung Galaxy S10", 150, smartPhone);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.AddItem(samsung, 2);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(0);
        }

        /// <summary>
        /// Calculate total discount with one campaign with rate discount type which less than minimum amount and coupon with rate discount type which less than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignRateDiscountTypeLessThanMinimumAmountAndCouponRateDiscountTypeLessThanMinimumAmount_ReturnZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var coupon = new Coupon(500, 10, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 1);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(0);
        }

        /// <summary>
        /// Calculate total discount with one campaign with rate discount type which greater than minimum amount and coupon with rate discount type which less than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignRateDiscountTypeGreaterThanMinimumAmountAndCouponRateDiscountTypeLessThanMinimumAmount_ReturnSixty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var coupon = new Coupon(500, 10, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(60);
        }

        /// <summary>
        /// Calculate total discount with one campaign with rate discount type which less than minimum amount and coupon with rate discount type which greater than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignRateDiscountTypeLessThanMinimumAmountAndCouponRateDiscountTypeGreaterThanMinimumAmount_ReturnSixty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 3, Enums.DiscountType.Rate);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(20);
        }


        /// <summary>
        /// Calculate total discount with one campaign with rate discount type which greater than minimum amount and coupon with rate discount type which greater than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignRateDiscountTypeGreaterThanMinimumAmountAndCouponRateDiscountTypeGreaterThanMinimumAmount_ReturnNinety()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 3, Enums.DiscountType.Rate);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(90);
        }

        /// <summary>
        /// Calculate total discount with one campaign with amount discount type which less than minimum amount and coupon with rate discount type which less than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignAmountDiscountTypeLessThanMinimumAmountAndCouponAmountDiscountTypeLessThanMinimumAmount_ReturnZero()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);
            var coupon = new Coupon(500, 10, Enums.DiscountType.Amount);
            //Act
            shoppingCart.AddItem(iPhone, 1);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(0);
        }

        /// <summary>
        /// Calculate total discount with one campaign with amount discount type which greater than minimum amount and coupon with rate discount type which less than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignAmountDiscountTypeGreaterThanMinimumAmountAndCouponAmountDiscountTypeLessThanMinimumAmount_ReturnTwenty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);
            var coupon = new Coupon(500, 10, Enums.DiscountType.Amount);
            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(20);
        }

        /// <summary>
        /// Calculate total discount with one campaign with amount discount type which less than minimum amount and coupon with amount discount type which greater than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignAmountDiscountTypeLessThanMinimumAmountAndCouponAmountDiscountTypeGreaterThanMinimumAmount_ReturnTen()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 3, Enums.DiscountType.Amount);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Amount);
            //Act
            shoppingCart.AddItem(iPhone, 2);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(10);
        }


        /// <summary>
        /// Calculate total discount with one campaign with amount discount type which greater than minimum amount and coupon with amount discount type which greater than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignAmountDiscountTypeGreaterThanMinimumAmountAndCouponAmountDiscountTypeGreaterThanMinimumAmount_ReturnThirty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 3, Enums.DiscountType.Amount);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Amount);
            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(30);
        }

        /// <summary>
        /// Calculate total discount with one campaign with rate discount type which greater than minimum amount and coupon with amount discount type which greater than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignAmountRateTypeGreaterThanMinimumAmountAndCouponAmountDiscountTypeGreaterThanMinimumAmount_ReturnSeventy()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Rate);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Amount);
            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(70);
        }

        /// <summary>
        /// Calculate total discount with one campaign with amount discount type which greater than minimum amount and coupon with amount discount type which greater than minimum amount.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDiscount_WithOneCampaignAmountAmountTypeGreaterThanMinimumAmountAndCouponRateDiscountTypeGreaterThanMinimumAmount_ReturnFifty()
        {
            //Arrange
            var phone = new Category("Phone");
            var smartPhone = new Category("SmartPhone", phone);
            var iPhone = new Product("IPhone", 100, smartPhone);
            var campaign = new Campaign(phone, 20, 2, Enums.DiscountType.Amount);
            var coupon = new Coupon(100, 10, Enums.DiscountType.Rate);
            //Act
            shoppingCart.AddItem(iPhone, 3);
            shoppingCart.ApplyDiscounts(campaign);
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            shoppingCart.GetTotalDiscounts().Should().Be(50);
        }

        #endregion

        #region tests calculating total amount after total (campaign&coupon) discounts.

        #endregion

    }
}
