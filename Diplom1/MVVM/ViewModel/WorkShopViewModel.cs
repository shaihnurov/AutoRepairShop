using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diplom1.MVVM.Model.Cars;
using Diplom1.MVVM.Model.WorkShop;
using Diplom1.MVVM.Model;
using Diplom1.Repository;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace Diplom1.MVVM.ViewModel
{
    public class WorkShopViewModel : ObservableObject
    {
        private Timer timer;
        private string _statusNotifications = "Hidden";
        private string _statusTitleBarSpares = "Hidden";
        private string _name;
        private decimal _balance;
        private GetMessage _getMessage;

        private readonly SparesRepository _sparesRepository;
        private readonly WorkShopRepository _workShopRepository;
        private readonly WorkShopSparesRepository _workShopSparesRepository;
        private readonly HistoryPayRepository _historyPayRepository;

        private ObservableCollection<WorkShopSparesModel> _workShop;
        private ObservableCollection<WorkShopSparesModel> _workShopAmountZero;

        public GetMessage GetMessage
        {
            get { return _getMessage; }
            set
            {
                _getMessage = value;
                OnPropertyChanged(nameof(GetMessage));
            }
        }
        public ObservableCollection<WorkShopSparesModel> WorkShop
        {
            get { return _workShop; }
            set
            {
                _workShop = value;
                OnPropertyChanged(nameof(WorkShop));
            }
        }
        public ObservableCollection<WorkShopSparesModel> WorkShopAmountZero
        {
            get { return _workShopAmountZero; }
            set
            {
                _workShopAmountZero = value;
                OnPropertyChanged(nameof(WorkShopAmountZero));
            }
        }
        public string StatusNotifications
        {
            get { return _statusNotifications; }
            set
            {
                _statusNotifications = value;
                OnPropertyChanged(nameof(StatusNotifications));
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
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public decimal Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                OnPropertyChanged(nameof(Balance));
            }
        }

        public RelayCommand<object> PayCommand { get; set; }
        public RelayCommand<object> DeleteCommand { get; set; }

        public WorkShopViewModel()
        {
            _sparesRepository = new SparesRepository();
            _workShopRepository = new WorkShopRepository();
            _workShopSparesRepository = new WorkShopSparesRepository();
            _historyPayRepository = new HistoryPayRepository();

            GetInfo();

            PayCommand = new RelayCommand<object>(Pay);
            DeleteCommand = new RelayCommand<object>(DeleteSpares);
        }

        private void GetInfo()
        {
            var workShop = _workShopRepository.GetByShopInfo();

            Name = workShop.Name;
            Balance = workShop.Balance;

            WorkShop = new ObservableCollection<WorkShopSparesModel>(_workShopSparesRepository.GetWorkShopSpares(workShop.Id));
            WorkShopAmountZero = new ObservableCollection<WorkShopSparesModel>(_workShopSparesRepository.GetSparesToAmountZero(workShop.Id));

            if (WorkShopAmountZero.Any())
                StatusNotifications = "Visible";
            else
                StatusNotifications = "Hidden";

            if (WorkShop.Count == 0)
                StatusTitleBarSpares = "Visible";
            else
                StatusTitleBarSpares = "Hidden";
        }
        public void Pay(object parameter)
        {
            if (parameter is WorkShopSparesModel selectedSpares)
            {
                try
                {
                    var workShop = _workShopRepository.GetByShopInfo();

                    _workShopRepository.DecreaseBalance(workShop.Id, selectedSpares.Price);
                    _sparesRepository.DecreaseSparesAmount(selectedSpares.SparesId);
                    _workShopSparesRepository.IncreaseAmount(selectedSpares.SparesId, workShop.Id, selectedSpares.Articul);

                    HistoryPayModel historyPayModel = new()
                    {
                        Name = selectedSpares.Name,
                        Amount = "1",
                        DateTime = DateTime.Now,
                        WorkShopId = workShop.Id,
                        WorkShopName = workShop.Name,
                        SparesId = selectedSpares.SparesId,
                        Price = selectedSpares.Price,
                    };
                    _historyPayRepository.AddHistoryPay(historyPayModel);

                    GetInfo();

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
                    GetInfo();
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
                            GetInfo();
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
        public void DeleteSpares(object parameter)
        {
            if (parameter is WorkShopSparesModel selected)
            {
                var id = selected.SparesId;
                var workShopId = selected.WorkShopId;

                _workShopSparesRepository.DeleteWorkShopSpares(id, workShopId);
                GetInfo();
            }
        }
    }
}