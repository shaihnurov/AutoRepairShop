using System;

namespace Diplom1.MVVM.Model
{
    public class HistoryPayModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public DateTime DateTime{ get; set; }
        public string WorkShopId { get; set; }
        public string WorkShopName { get; set; }
        public string SparesId { get; set; }
        public decimal Price { get; set; }
    }
}
