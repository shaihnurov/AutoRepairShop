using Diplom1.MVVM.Model.Cars;
using System.Collections.ObjectModel;

namespace Diplom1.MVVM.Model.Interface
{
    interface ISparesRepository
    {
        ObservableCollection<SparesModel> GetSparesByCategory(string name, string make);
        int DecreaseSparesAmount(string sparesId);
        void IncreaseSparesAmount(string sparesId);
    }
}
