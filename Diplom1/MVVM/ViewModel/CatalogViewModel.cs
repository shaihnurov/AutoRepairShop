using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diplom1.MVVM.Model;
using Diplom1.MVVM.Model.Cars;
using Diplom1.Repository;
using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace Diplom1.MVVM.ViewModel
{
    public class CatalogViewModel : ObservableObject
    {
        private Timer timer;
        private GetMessage _getMessage;
        private string _getNameModel;
        private string _getMakeModel;

        private readonly SparesRepository _sparesRepository;
        private readonly WorkShopRepository _workShopRepository;
        private readonly WorkShopSparesRepository _workShopSparesRepository;
        private readonly HistoryPayRepository _historyPayRepository;
        private readonly CarsRepository _carsRepository;

        private ObservableCollection<SparesModel> _spares;
        private ObservableCollection<Car> _cars;
        private ObservableCollection<CarsModel> _carsModel;

        public ObservableCollection<Car> Cars
        {
            get { return _cars; }
            set
            {
                _cars = value;
                OnPropertyChanged(nameof(Cars));
            }
        }
        public ObservableCollection<CarsModel> CarsModel
        {
            get { return _carsModel; }
            set
            {
                _carsModel = value;
                OnPropertyChanged(nameof(CarsModel));
            }
        }
        public ObservableCollection<SparesModel> Spares
        {
            get { return _spares; }
            set
            {
                _spares = value;
                OnPropertyChanged(nameof(Spares));
            }
        }
        public GetMessage GetMessage
        {
            get { return _getMessage; }
            set
            {
                _getMessage = value;
                OnPropertyChanged(nameof(GetMessage));
            }
        }

        public RelayCommand<object> PayCommand { get; set; }
        public RelayCommand<object> EditMakeCommand { get; set; }
        public RelayCommand<object> EditSparesCommand { get; set; }

        public CatalogViewModel() 
        {
            _sparesRepository = new SparesRepository();
            _workShopRepository = new WorkShopRepository();
            _workShopSparesRepository = new WorkShopSparesRepository();
            _historyPayRepository = new HistoryPayRepository();
            _carsRepository = new CarsRepository();

            Cars = _carsRepository.GetCars();

            PayCommand = new RelayCommand<object>(Pay);
            EditMakeCommand = new RelayCommand<object>(FilterModel);
            EditSparesCommand = new RelayCommand<object>(FilterSpares);
        }

        public void FilterModel(object parameter)
        {
            if (parameter is Car selectedMake)
            {
                var make = selectedMake.Name;
                CarsModel = _carsRepository.GetCarsModel(make);
            }
        }
        public void FilterSpares(object parameter)
        {
            if (parameter is CarsModel selected)
            {
                _getNameModel = selected.NameModel;
                _getMakeModel = selected.Make;

                Spares = new ObservableCollection<SparesModel>(_sparesRepository.GetSparesByCategory(_getNameModel, _getMakeModel));
            }
        }
        private void UpdateUserList()
        {
            try
            {
                Spares = new ObservableCollection<SparesModel>(_sparesRepository.GetSparesByCategory(_getNameModel, _getMakeModel));
            }
            catch (Exception ex)
            {
                GetMessage = new GetMessage
                {
                    Message = $"* Список пуст, возникла ошибка: {ex.Message}",
                    TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7596D"))
                };
            }
        }
        private void Pay(object parameter)
        {
            if(parameter is SparesModel selectedSpares)
            {
                try
                {
                    var workShop = _workShopRepository.GetByShopInfo();

                     _sparesRepository.DecreaseSparesAmount(selectedSpares.Id);
                    _workShopRepository.DecreaseBalance(workShop.Id, selectedSpares.Price);
                    _workShopSparesRepository.IncreaseAmount(selectedSpares.Id, workShop.Id, selectedSpares.Articul);

                    HistoryPayModel historyPayModel = new()
                    {
                        Name = selectedSpares.Name,
                        Amount = "1",
                        DateTime = DateTime.Now,
                        WorkShopId = workShop.Id,
                        WorkShopName = workShop.Name,
                        SparesId = selectedSpares.Id,
                        Price = selectedSpares.Price,
                    };
                    _historyPayRepository.AddHistoryPay(historyPayModel);

                    UpdateUserList();

                    GetMessage = new GetMessage
                    {
                        Message = $"* Успешная покупка",
                        TextColor = Brushes.Green
                    };
                }
                catch (InvalidOperationException ex)
                {
                    GetMessage = new GetMessage
                    {
                        Message = $"Не удалось провести оплату: {ex.Message}",
                        TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7596D"))
                    };
                    RefillTimer(parameter);
                    UpdateUserList();
                }
                catch (Exception ex)
                {
                    GetMessage = new GetMessage
                    {
                        Message = $"Не удалось провести оплату: {ex.Message}",
                        TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7596D"))
                    };
                }
            }
        }
        private void RefillTimer(object parameter)
        {
            if (parameter is SparesModel selectedSpares)
            {
                Random rnd = new();
                int randomNumber = rnd.Next(1, 3);
                int millisecondsInMinute = 60000;

                timer = new Timer(randomNumber * millisecondsInMinute)
                {
                    AutoReset = false
                };

                timer.Start();

                timer.Elapsed += (sender, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            _sparesRepository.IncreaseSparesAmount(selectedSpares.Id);
                            UpdateUserList();
                        }
                        catch (Exception ex)
                        {
                            GetMessage = new GetMessage
                            {
                                Message = $"* {ex.Message}",
                                TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7596D"))
                            };
                        }
                    });
                };
            }
        }
    }
}