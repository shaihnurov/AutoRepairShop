using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diplom1.MVVM.Model.Cars;
using Diplom1.MVVM.Model.WorkShop;
using Diplom1.Repository;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Diplom1.MVVM.ViewModel
{
    public class WorkShopViewModel : ObservableObject
    {
        private Visibility _statusNotifications = Visibility.Hidden;
        private Visibility _statusTitleBarSpares = Visibility.Hidden;
        private string _name;
        private decimal _balance;
        private GetMessage _getMessage;

        private readonly WorkShopRepository _workShopRepository;
        private readonly WorkShopSparesRepository _workShopSparesRepository;

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
        public Visibility StatusNotifications
        {
            get { return _statusNotifications; }
            set
            {
                _statusNotifications = value;
                OnPropertyChanged(nameof(StatusNotifications));
            }
        }
        public Visibility StatusTitleBarSpares
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

        public RelayCommand<object> DeleteCommand { get; set; }

        public WorkShopViewModel()
        {
            _workShopRepository = new WorkShopRepository();
            _workShopSparesRepository = new WorkShopSparesRepository();

            GetInfo();

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
                StatusNotifications = Visibility.Visible;
            else
                StatusNotifications = Visibility.Hidden;

            if (WorkShop.Count == 0)
                StatusTitleBarSpares = Visibility.Visible;
            else
                StatusTitleBarSpares = Visibility.Hidden;
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