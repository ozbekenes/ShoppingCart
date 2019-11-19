using Trendyol.ECommerce.ShoppingCart.Logic.Interfaces;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class DeliveryCostCalculator
    {
        private double CostPerDelivery { get; set; }
        private double CostPerProduct { get; set; }
        private double FixedCost { get; set; }

        public DeliveryCostCalculator(double costPerDelivery, double costPerProduct, double fixedCost = 2.99)
        {
            CostPerDelivery = costPerDelivery;
            CostPerProduct = costPerProduct;
            FixedCost = fixedCost;
        }

        public double CalculateFor(IShoppingCart cart)
        {
            if (cart == null) return 0;
            int numberOfDeliveries = cart.GetNumberOfDeliveries();
            if (numberOfDeliveries == 0) return 0;
            int numberOfProducts = cart.GetNumberOfProducts();
            return (CostPerDelivery * numberOfDeliveries) + (CostPerProduct * numberOfProducts) + FixedCost;
        }
    }


}
