using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class ShoppingCart
    {
        const int lengthOfSpace = 20;
        public List<ShoppingCartDetail> ShoppingCartDetail { get; set; }
        public List<Campaign> CampaignList { get; set; }
        public Coupon Coupon { get; set; }
        private DeliveryCostCalculator DeliveryCostCalculator { get; set; }

        private Dictionary<Category, double> campaignDiscountsByCategory = new Dictionary<Category, double>();

        private Dictionary<Product, double> campaignDiscountsByProduct = new Dictionary<Product, double>();

        private Dictionary<Product, double> couponDiscountsByProduct = new Dictionary<Product, double>();

        private double totalCouponDiscount { get; set; }

        /// <summary>
        /// Get the number of deliveries by calculating the number of distinct categories in the cart.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfDeliveries()
        {
            return ShoppingCartDetail.GroupBy(e => e.Product.Category.Title).Count();
        }

        /// <summary>
        /// Get the number of products by calculating the number of different products in the cart.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfProducts()
        {
            return ShoppingCartDetail.Count;
        }

        public ShoppingCart(DeliveryCostCalculator deliveryCostCalculator)
        {
            DeliveryCostCalculator = deliveryCostCalculator;
            ShoppingCartDetail = new List<ShoppingCartDetail>();
        }

        /// <summary>
        /// Add item to chart by product and quantity. If Item exist in the cart, add to the existing product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        public void AddItem(Product product, int quantity)
        {
            if (product == null || quantity <= 0) return;

            var query = ShoppingCartDetail.FirstOrDefault(p => p.Product == product);
            if (query != null)
            {
                query.Quantity -= -quantity;
            }
            else
            {
                ShoppingCartDetail.Add(new ShoppingCartDetail { Product = product, Quantity = quantity });
            }
        }

        /// <summary>
        /// Applying campaign discounts by using campaign paramater.
        /// Group campaign by category.
        /// Get products by grouped campaign category.
        /// Calculate total campaign discount amount by given rule.
        /// Calculate campaign discount per product and add per product and discount to dictionary.
        /// </summary>
        /// <param name="campaigns"></param>
        public void ApplyDiscounts(params Campaign[] campaigns)
        {
            var campaignsByCategory = campaigns.GroupBy(p => p.Category).ToList();
            foreach (var item in campaignsByCategory)
            {
                var productsByCategory = GetProductsByCategory(item.Key);
                var discountValue = 0.0;
                if (productsByCategory == null) continue;
                var genericCalculator = new GenericCalculator();
                foreach (var campaign in item)
                {
                    if (productsByCategory.Sum(p => p.Quantity) >= campaign.DiscountParameter.Quantity)
                    {
                        genericCalculator = new GenericCalculator();
                        var calculator = genericCalculator.Calculate(campaign.DiscountParameter.DiscountType);
                        calculator.CalculateCampaignDiscount(productsByCategory, campaign, ref discountValue);
                    }
                }
                if (discountValue > 0)
                {
                    var totalAmountByCategory = productsByCategory.Sum(p => p.Product.Price * p.Quantity);
                    foreach (var product in productsByCategory)
                    {
                        var productPrice = product.Product.Price * product.Quantity;
                        campaignDiscountsByProduct.Add(product.Product, Math.Round(productPrice / totalAmountByCategory * discountValue, 2));
                    }
                    //campaignDiscountsByCategory.Add(item.Key, discountValue);
                }
            }
        }

        /// <summary>
        /// Create repeated string with given character and count
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Repeat(string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }

        /// <summary>
        /// Print the shopping cart details and total amount and delivery cost.
        /// </summary>
        /// <returns></returns>
        public string Print()
        {

            StringBuilder sBuilder = new StringBuilder();
            var products = ShoppingCartDetail.GroupBy(p => p.Product.Category.Title).ToList();
            sBuilder.AppendLine($"{"Category Name",lengthOfSpace}  {"Product Name",lengthOfSpace}  {"Quantity",lengthOfSpace}  {"Unit Price",lengthOfSpace}  {"Total Price",lengthOfSpace}  {"Total Discount(coupon,campaign)",lengthOfSpace}");
            sBuilder.AppendLine(Repeat("-", 7 * lengthOfSpace));
            foreach (var item in products)
            {
                foreach (var product in item)
                {
                    sBuilder.AppendLine($"{item.Key,lengthOfSpace} {product.Product.Title,lengthOfSpace} {product.Quantity,lengthOfSpace} " +
                        $"{product.Product.Price,lengthOfSpace} {GetProductPrice(product),lengthOfSpace}  {GetTotalDiscountsByProduct(product.Product),lengthOfSpace}");
                }
            }
            sBuilder.AppendLine(Repeat("-", 7 * lengthOfSpace));
            sBuilder.AppendLine($"{"Total Amount"}  {"Delivery Cost",lengthOfSpace}");
            sBuilder.AppendLine($" {GetTotalAmount()}  {GetDeliveryCost(),lengthOfSpace}");

            return sBuilder.ToString();
        }

        /// <summary>
        /// Get delivery cost of shopping cart with given formula.
        /// </summary>
        /// <returns></returns>
        private double GetDeliveryCost()
        {
            return DeliveryCostCalculator.CalculateFor(this);
        }

        /// <summary>
        /// Calculate total discount of product. 
        /// Calculate campaign discount and coupon discount seperately per product.
        /// Return sum of them.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private double GetTotalDiscountsByProduct(Product product)
        {
            var campaignDiscount = 0.0;
            var couponDiscount = 0.0;
            var campaign = campaignDiscountsByProduct.FirstOrDefault(p => p.Key == product);
            var coupon = couponDiscountsByProduct.FirstOrDefault(p => p.Key == product);

            if (campaign.Key != null) campaignDiscount = campaign.Value;
            if (coupon.Key != null) couponDiscount = coupon.Value;

            return couponDiscount + campaignDiscount;
        }

        /// <summary>
        /// Calculate the product price in shopping cart with product price and quantity.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private double GetProductPrice(ShoppingCartDetail product)
        {
            return product.Product.Price * product.Quantity;
        }

        /// <summary>
        /// Calculate the coupon discount by given rule.
        /// Calculate coupon discount per product and add per product and  coupon discount to dictionary.
        /// </summary>
        /// <param name="coupon"></param>
        public void ApplyCoupon(Coupon coupon)
        {
            if (coupon == null) return;
            var totalAmount = GetTotalAmount();
            if (totalAmount < coupon.DiscountParameter.Amount) return;

            var genericCalculator = new GenericCalculator();
            var calculator = genericCalculator.Calculate(coupon.DiscountParameter.DiscountType);
            totalCouponDiscount = calculator.CalculateCouponDiscount(totalAmount, coupon);

            foreach (var product in ShoppingCartDetail)
            {
                var productPrice = product.Product.Price * product.Quantity;
                couponDiscountsByProduct.Add(product.Product, Math.Round(productPrice / totalAmount * totalCouponDiscount, 2));
            }
        }

        /// <summary>
        /// retun calculated total coupon discount.
        /// </summary>
        /// <returns></returns>
        public double GetCouponDiscount()
        {
            return totalCouponDiscount;
        }

        //public double GetCampaignDiscount()
        //{
        //    return campaignDiscountsByCategory != null ? campaignDiscountsByCategory.Sum(p => p.Value) : 0;
        //}

        /// <summary>
        /// Return calculated campaign discount.
        /// </summary>
        /// <returns></returns>
        public double GetCampaignDiscount()
        {
            return campaignDiscountsByProduct != null ? campaignDiscountsByProduct.Sum(p => p.Value) : 0;
        }

        /// <summary>
        /// Calculate total amount after discounts by subtracting total amount from the total discounts.
        /// </summary>
        /// <returns></returns>
        public double GetTotalAmountAfterDiscounts()
        {
            return GetTotalAmount() - GetCampaignDiscount() - GetCouponDiscount();
            
        }

        public double GetTotalDiscounts()
        {
            return GetCampaignDiscount() + GetCouponDiscount();
        }

        /// <summary>
        /// Calculate the total amount of the given category input.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private double GetTotalAmountByCategory(Category category)
        {
            return ShoppingCartDetail.Where(p => p.Product.Category == category).Sum(e => e.Product.Price * e.Quantity);
        }

        /// <summary>
        /// Get total amount of the shopping cart.
        /// </summary>
        /// <returns></returns>
        public double GetTotalAmount()
        {
            return ShoppingCartDetail.Sum(p => p.Product.Price * p.Quantity);
        }

        /// <summary>
        /// Check if given category is sub category.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        private bool IsSubCategory(Category parent, Category sub)
        {
            Category temp = sub.ParentCategory;
            while (temp != null)
            {
                if (temp == parent)
                {
                    return true;
                }
                temp = temp.ParentCategory;
            }
            return false;
        }
        
        /// <summary>
        /// Get products by given category.
        /// Products which grouped based on category used to calculate campaign discount. 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private List<ShoppingCartDetail> GetProductsByCategory(Category category)
        {
            return ShoppingCartDetail.Where(p => p.Product.Category == category || IsSubCategory(category, p.Product.Category)).ToList();
        }
    }
}
