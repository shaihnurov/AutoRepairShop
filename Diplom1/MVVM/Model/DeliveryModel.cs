using System;

namespace Diplom1.MVVM.Model
{
    public class DeliveryModel
    {
        public string Id { get; set; }
        public string IdWorkShop { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateDelivery { get; set; }
        public string SparesId { get; set; }
        public string Amount { get; set; }
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public string Make { get; set; }
        public string Articul { get; set; }
        public decimal Price { get; set; }
    }
}
