using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diplom1.MVVM.Model.Cars;
using Diplom1.Repository;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;

namespace Diplom1.MVVM.ViewModel
{
    public class InfoWorkShopViewModel : ObservableObject
    {
        private string _idWorkShop;
        private byte[] _imageBytes;
        private string _contentButtonImage;
        private string _name;
        private string _adress;
        private decimal _balance;
        private byte[] _image;
        private SolidColorBrush _backgroundLoadPhoto;
        private GetMessage _getMessage;
        private readonly WorkShopRepository _workShopRepository;

        public GetMessage GetMessage
        {
            get { return _getMessage; }
            set
            {
                _getMessage = value;
                OnPropertyChanged(nameof(GetMessage));
            }
        }
        public string ContentButtonImage
        {
            get { return _contentButtonImage; }
            set
            {
                _contentButtonImage = value;
                OnPropertyChanged(nameof(ContentButtonImage));
            }
        }
        public SolidColorBrush BackgroundLoadPhoto
        {
            get { return _backgroundLoadPhoto; }
            set
            {
                _backgroundLoadPhoto = value;
                OnPropertyChanged(nameof(BackgroundLoadPhoto));
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
        public string Adress
        {
            get { return _adress; }
            set
            {
                _adress = value;
                OnPropertyChanged(nameof(Adress));
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

        public RelayCommand LoadPhotoCommand { get; set; }
        public RelayCommand EditClickCommand { get; set; }
        public RelayCommand EditWorkShopCommand { get; set; }

        public InfoWorkShopViewModel()
        {
            _workShopRepository = new WorkShopRepository();

            EditWorkShopCommand = new RelayCommand(EditWorkShop);
            LoadPhotoCommand = new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string imagePath = openFileDialog.FileName;
                    _imageBytes = File.ReadAllBytes(imagePath);
                    _workShopRepository.UpdateWorkShopImage(_idWorkShop, _imageBytes);
                    ContentButtonImage = "Успешно";
                    BackgroundLoadPhoto = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#25A02A"));
                    GetInfo();
                }
                else
                {
                    GetMessage = new GetMessage
                    {
                        Message = "* Выбор фотографии отменен",
                        TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7596D"))
                    };
                }
            });

            GetInfo();
        }
        private void GetInfo()
        {
            var WorkShopData = _workShopRepository.GetByShopInfo();
            _idWorkShop = WorkShopData.Id;
            Name = WorkShopData.Name;
            Adress = WorkShopData.Adress;
            Balance = WorkShopData.Balance;
            Image = WorkShopData.Image;

            ContentButtonImage = "Загрузить логотип";
            BackgroundLoadPhoto = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30D337"));
        }
        private void EditWorkShop()
        {
            var WorkShopData = _workShopRepository.GetByShopInfo();
            if (WorkShopData.Adress != Adress) 
            {
                _workShopRepository.UpdateWorkShopAdress(WorkShopData.Id, Adress);
            }
            else if (WorkShopData.Name != Name)
            {
                _workShopRepository.UpdateWorkShopName(WorkShopData.Id, Name);
            }
        }
    }
}
