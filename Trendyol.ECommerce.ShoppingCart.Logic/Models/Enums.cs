using System.Xml.Serialization;

namespace Trendyol.ECommerce.ShoppingCart.Logic.Models
{
    public static class Enums
    {
        public enum DiscountType
        {
            [XmlEnum("Rate")]
            Rate,
            [XmlEnum("Amount")]
            Amount
        }
    }
}
