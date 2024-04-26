using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diplom1.MVVM.Model;
using Diplom1.MVVM.Model.Cars;
using Diplom1.Repository;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Diplom1.MVVM.ViewModel
{
    public class DeliveryViewModel : ObservableObject
    {
        private Visibility _statusTitleBarDelivery = Visibility.Hidden;
        private string _adress;

        private GetMessage _getMessage;
        private readonly WorkShopSparesRepository _workShopSparesRepository;
        private readonly WorkShopRepository _workShopRepository;

        private ObservableCollection<DeliveryModel> _deliveryModel;

        public ObservableCollection<DeliveryModel> DeliveryModel
        {
            get { return _deliveryModel; }
            set
            {
                _deliveryModel = value;
                OnPropertyChanged(nameof(DeliveryModel));
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
        public Visibility StatusTitleBarDelivery
        {
            get { return _statusTitleBarDelivery; }
            set
            {
                _statusTitleBarDelivery = value;
                OnPropertyChanged(nameof(StatusTitleBarDelivery));
            }
        }
        public string Adress
        {
            get { return _adress; }
            set
            {
                _adress = value;
                OnPropertyChanged(nameof(Adress));
            }
        }

        public RelayCommand<object> DeleteDeliveryCommand { get; set; }

        public DeliveryViewModel()
        {
            _workShopSparesRepository = new WorkShopSparesRepository();
            _workShopRepository = new WorkShopRepository();

            DeleteDeliveryCommand = new RelayCommand<object>(DeleteDelivery); 

            UpdateUserList();
        }

        private void UpdateUserList()
        {
            try
            {
                var workShop = _workShopRepository.GetByShopInfo();
                DeliveryModel = new ObservableCollection<DeliveryModel>(_workShopSparesRepository.GetDeliveries(workShop.Id));
                Adress = workShop.Adress;
                if (DeliveryModel.Count == 0)
                    StatusTitleBarDelivery = Visibility.Visible;
                else
                    StatusTitleBarDelivery = Visibility.Hidden;
                CheckDeliveriesAsync(workShop.Id);
            }
            catch (Exception ex)
            {
                GetMessage = new GetMessage
                {
                    Message = $"{ex.Message}",
                    TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7596D"))
                };
            }
        }
        private async void CheckDeliveriesAsync(string idWorkShop)
        {
            try
            {
                await Task.Run(() => _workShopSparesRepository.CheckDeliveriesForExactTime(DeliveryModel, idWorkShop));
            }
            catch (Exception ex)
            {
                GetMessage = new GetMessage
                {
                    Message = $"{ex.Message}",
                    TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7596D"))
                };
            }
        }
        private void DeleteDelivery(object parameter)
        {
            if(parameter is DeliveryModel selectedDelivery)
            {
                var succ = MessageBox.Show($"Вы уверенны, что хотите отменить доставку {selectedDelivery.Name}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if(succ == MessageBoxResult.OK){
                    _workShopSparesRepository.DeleteDeliveies(selectedDelivery.Id);
                    _workShopRepository.IncreaseBalance(selectedDelivery.IdWorkShop, selectedDelivery.Price);
                    UpdateUserList();
                }
            }
        } 
    }
}
