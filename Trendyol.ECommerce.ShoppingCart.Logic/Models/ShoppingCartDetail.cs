using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public class ShoppingCartDetail
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
