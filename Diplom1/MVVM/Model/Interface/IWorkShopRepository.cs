using Diplom1.MVVM.Model.WorkShop;

namespace Diplom1.MVVM.Model.Interface
{
    interface IWorkShopRepository
    {
        WorkShopModel GetByShopInfo();
        void DecreaseBalance(string workShopId, decimal priceSpares);

    }
}
