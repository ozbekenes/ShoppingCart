using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trendyol.ECommerce.ShoppingCart.Logic.Interfaces;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class GenericCalculator
    {
        public ICalculator Calculate(Enums.DiscountType discountType)
        {
            ICalculator calculator = null;

            if (discountType == Enums.DiscountType.Rate) calculator = new RateCalculator();
            if (discountType == Enums.DiscountType.Amount) calculator = new AmountCalculator();

            return calculator;
        }
    }
}
