using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diplom1.Repository;
using System;
using System.Windows;

namespace Diplom1.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private object _currentView;
        private string _titlePage;
        private decimal _balance;
        private byte[] _image;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        public string TitlePage
        {
            get { return _titlePage; }
            set
            {
                _titlePage = value;
                OnPropertyChanged(nameof(TitlePage));
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
        public byte[] Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        private readonly WorkShopRepository _workShopRepository;
        private CatalogViewModel CatalogVM { get; set; }
        private WorkShopViewModel WorkShopVM { get; set; }
        private HistoryViewModel HistoryVM { get; set; }
        private SelectionSparesViewModel SelectionSparesVM { get; set; }
        private DeliveryViewModel DeliveryVM { get; set; }
        private InfoWorkShopViewModel InfoWorkShopVM { get; set; }

        public RelayCommand CatalogViewCommand { get; set; }
        public RelayCommand WorkShopViewCommand { get; set; }
        public RelayCommand HistoryViewCommand { get; set; }
        public RelayCommand SelectionSparesCommand { get; set; }
        public RelayCommand DeliveryCommand { get; set; }
        public RelayCommand InfoWorkShopCommand { get; set; }

        public MainViewModel()
        {
            try
            {
                _workShopRepository = new WorkShopRepository();

                CatalogVM = new CatalogViewModel();
                WorkShopVM = new WorkShopViewModel();
                HistoryVM = new HistoryViewModel();
                SelectionSparesVM = new SelectionSparesViewModel();
                DeliveryVM = new DeliveryViewModel();
                InfoWorkShopVM = new InfoWorkShopViewModel();
                LoadInfo();

                TitlePage = "Автомастерская";
                CurrentView = WorkShopVM;

                CatalogViewCommand = new RelayCommand(() =>
                {
                    CurrentView = CatalogVM;
                    TitlePage = "Каталог";
                    LoadInfo();
                });

                WorkShopViewCommand = new RelayCommand(() =>
                {
                    CurrentView = WorkShopVM;
                    TitlePage = "Автомастерская";
                    LoadInfo();
                });

                HistoryViewCommand = new RelayCommand(() =>
                {
                    CurrentView = HistoryVM;
                    TitlePage = "История заказов";
                    LoadInfo();
                });

                SelectionSparesCommand = new RelayCommand(() =>
                {
                    CurrentView = SelectionSparesVM;
                    TitlePage = "Подбор запчастей";
                    LoadInfo();
                });

                DeliveryCommand = new RelayCommand(() =>
                {
                    CurrentView = DeliveryVM;
                    TitlePage = "Доставка";
                    LoadInfo();
                });

                InfoWorkShopCommand = new RelayCommand(() =>
                {
                    CurrentView = InfoWorkShopVM;
                    TitlePage = "Личный кабинет";
                    LoadInfo();
                });
            }
            catch (System.Data.SqlClient.SqlException)
            {
                MessageBox.Show("Не удалось установить соединение с базой. Пожалуйста, проверьте корректность работы SQL Server Management Studio 20. \n\nДополнительно, убедитесь, что установленная локальная БД имеет название AutoRepairShop. \n\nБолее подробную информацию можно узнать изучив исходники на GitHub: \t github.com/shaihnurov/AutoRepairShop");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadInfo()
        {
            var workShop = _workShopRepository.GetByShopInfo();

            Balance = workShop.Balance;
            Image = workShop.Image;
        }
    }
}
