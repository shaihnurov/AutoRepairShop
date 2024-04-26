using Diplom1.MVVM.Model.Cars;
using Diplom1.MVVM.Model.WorkShop;
using System.Collections.ObjectModel;

namespace Diplom1.MVVM.Model.Interface
{
    interface ISelectionSparesRepository
    {
        ObservableCollection<WorkShopSparesModel> GetAvailableSpares(string nameModel, string make);
        ObservableCollection<SparesModel> GetSparesShop(string nameModel, string make);
    }
}
