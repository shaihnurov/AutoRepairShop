using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diplom1.MVVM.Model;
using Diplom1.MVVM.Model.Cars;
using Diplom1.MVVM.Model.WorkShop;
using Diplom1.Repository;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace Diplom1.MVVM.ViewModel
{
    public class SelectionSparesViewModel : ObservableObject
    {
        private string _selectedCarModel;
        private string _workShopName;
        private string _statusTitleBarSpares = "Hidden";
        private string _statusCountSparesWorkShop = "Hidden";
        private string _statusCountSparesShop = "Hidden";
        private string _getNameModel;
        private string _getMakeModel;
        private Timer timer;
        private GetMessage _getMessage;

        private readonly SparesRepository _sparesRepository;
        private readonly SelectionSparesRepository _selectionSparesRepository;
        private readonly WorkShopRepository _workShopRepository;
        private readonly CarsRepository _carsRepository;
        private readonly WorkShopSparesRepository _workShopSparesRepository;
        private readonly HistoryPayRepository _historyPayRepository;

        private ObservableCollection<Car> _cars;
        private ObservableCollection<CarsModel> _carsModel;
        private ObservableCollection<WorkShopSparesModel> _spares;
        private ObservableCollection<SparesModel> _sparesShop;

        public GetMessage GetMessage
        {
            get { return _getMessage; }
            set
            {
                _getMessage = value;
                OnPropertyChanged(nameof(GetMessage));
            }
        }
        public string StatusCountSparesShop
        {
            get { return _statusCountSparesShop; }
            set
            {
                _statusCountSparesShop = value;
                OnPropertyChanged(nameof(StatusCountSparesShop));
            }
        }
        public string StatusCountSparesWorkShop
        {
            get { return _statusCountSparesWorkShop; }
            set
            {
                _statusCountSparesWorkShop = value;
                OnPropertyChanged(nameof(StatusCountSparesWorkShop));
            }
        }
        public string StatusTitleBarSpares
        {
            get { return _statusTitleBarSpares; }
            set
            {
                _statusTitleBarSpares = value;
                OnPropertyChanged(nameof(StatusTitleBarSpares));
            }
        }
        public string WorkShopName
        {
            get { return _workShopName; }
            set
            {
                _workShopName = value;
                OnPropertyChanged(nameof(WorkShopName));
            }
        }
        public string SelectedCarModel
        {
            get { return _selectedCarModel; }
            set
            {
                _selectedCarModel = value;
                OnPropertyChanged(nameof(SelectedCarModel));
            }
        }
        public ObservableCollection<SparesModel> SparesShop
        {
            get { return _sparesShop; }
            set
            {
                _sparesShop = value;
                OnPropertyChanged(nameof(SparesShop));
            }
        }
        public ObservableCollection<WorkShopSparesModel> Spares
        {
            get { return _spares; }
            set
            {
                _spares = value;
                OnPropertyChanged(nameof(Spares));
            }
        }
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
        
        public RelayCommand<object> EditMakeCommand { get; set; }
        public RelayCommand<object> EditSparesCommand { get; set; }
        public RelayCommand<object> SelectedSparesUse { get; set; }
        public RelayCommand<object> PayCommand { get; set; }

        public SelectionSparesViewModel() 
        {
            _selectionSparesRepository = new SelectionSparesRepository();
            _carsRepository = new CarsRepository();
            _workShopRepository = new WorkShopRepository();
            _workShopSparesRepository = new WorkShopSparesRepository();
            _sparesRepository = new SparesRepository();
            _historyPayRepository = new HistoryPayRepository();

            Cars = _carsRepository.GetCars();

            EditMakeCommand = new RelayCommand<object>(FilterModel);
            EditSparesCommand = new RelayCommand<object>(FilterSpares);
            SelectedSparesUse = new RelayCommand<object>(UseSpares);
            PayCommand = new RelayCommand<object>(Pay);
        }

        private void UpdateUserList()
        {
            try
            {
                Spares = new ObservableCollection<WorkShopSparesModel>(_selectionSparesRepository.GetAvailableSpares(_getNameModel, _getMakeModel));
                if (Spares.Count == 0)
                    StatusCountSparesWorkShop = "Visible";
                else
                    StatusCountSparesWorkShop = "Hidden";

                SparesShop = new ObservableCollection<SparesModel>(_selectionSparesRepository.GetSparesShop(_getNameModel, _getMakeModel));
                if (SparesShop.Count == 0)
                    StatusCountSparesShop = "Visible";
                else
                    StatusCountSparesShop = "Hidden";
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

                SelectedCarModel = $"{_getMakeModel} {_getNameModel}";
                var workShop = _workShopRepository.GetByShopInfo();
                WorkShopName = workShop.Name;
                StatusTitleBarSpares = "Visible";

                UpdateUserList();
                
            }
        }
        public void UseSpares(object parameter)
        {
            if (parameter is WorkShopSparesModel selected)
            {
                var id = selected.Id;
                var workShopId = selected.WorkShopId;

                _workShopSparesRepository.DecreaseSparesAmount(id, workShopId);
                UpdateUserList();
            }
        }
        private void Pay(object parameter)
        {
            if (parameter is SparesModel selectedSpares)
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
