namespace Diplom1.MVVM.Model.WorkShop
{
    public class WorkShopSparesModel
    {
        public string Id { get; set; }
        public string WorkShopId { get; set; }
        public string SparesId { get; set; }
        public string Amount { get; set; }
        public string Name { get; set; }
        public string Articul { get; set; }
        public string Make { get; set; }
        public byte[] Image { get; set; }
        public decimal Price { get; set; }
    }
}
