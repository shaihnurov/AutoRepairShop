using System.Collections.ObjectModel;

namespace Diplom1.MVVM.Model.Interface
{
    interface IHistoryPayRepository
    {
        void AddHistoryPay(HistoryPayModel historyModel);
        ObservableCollection<HistoryPayModel> GetAllHistory(string workShopId);
    }
}
