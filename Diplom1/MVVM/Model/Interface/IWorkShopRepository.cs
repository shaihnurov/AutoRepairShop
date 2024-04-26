using Diplom1.MVVM.Model.WorkShop;

namespace Diplom1.MVVM.Model.Interface
{
    interface IWorkShopRepository
    {
        WorkShopModel GetByShopInfo();
        void UpdateWorkShopImage(string workShopId, byte[] imageBytes);
        void UpdateWorkShopAdress(string workShopId, string adress);
        void UpdateWorkShopName(string workShopId, string name);
        void DecreaseBalance(string workShopId, decimal priceSpares);
        void IncreaseBalance(string workShopId, decimal priceSpares);
    }
}
