using Diplom1.MVVM.Model.WorkShop;
using System.Collections.ObjectModel;

namespace Diplom1.MVVM.Model.Interface
{
    interface IWorkShopSparesRepository
    {
        void AddSpares(WorkShopSparesModel wssModel);
        ObservableCollection<WorkShopSparesModel> GetWorkShopSpares(string workShopId);
        ObservableCollection<WorkShopSparesModel> GetSparesToAmountZero(string workShopId);
        void IncreaseAmount(string sparesId, string workshopId, string articul);
        void DecreaseSparesAmount(string sparesId, string workShopId);
        void DeleteWorkShopSpares(string sparesId, string workShopId);
    }
}
