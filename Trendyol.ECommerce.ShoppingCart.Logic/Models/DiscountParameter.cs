using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class DiscountParameter
    {
        public double Amount { get; set; }
        public double Quantity { get; set; }
        public Enums.DiscountType DiscountType { get; set; }
    }
}
