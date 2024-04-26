using Diplom1.MVVM.Model.Cars;
using System.Collections.ObjectModel;

namespace Diplom1.MVVM.Model.Interface
{
    interface ICarsRepository
    {
        ObservableCollection<Car> GetCars();
        ObservableCollection<CarsModel> GetCarsModel(string selectedMake);
    }
}
