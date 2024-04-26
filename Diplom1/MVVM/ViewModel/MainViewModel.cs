using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Diplom1.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private CatalogViewModel CatalogVM { get; set; }
        private WorkShopViewModel WorkShopVM { get; set; }
        private HistoryViewModel HistoryVM { get; set; }
        private SelectionSparesViewModel SelectionSparesVM { get; set; }

        public RelayCommand CatalogViewCommand { get; set; }
        public RelayCommand WorkShopViewCommand { get; set; }
        public RelayCommand HistoryViewCommand { get; set; }
        public RelayCommand SelectionSparesCommand { get; set; }

        public MainViewModel()
        {
            CatalogVM = new CatalogViewModel();
            WorkShopVM = new WorkShopViewModel();
            HistoryVM = new HistoryViewModel();
            SelectionSparesVM = new SelectionSparesViewModel();

            CurrentView = WorkShopVM;

            CatalogViewCommand = new RelayCommand(() =>
            {
                CurrentView = CatalogVM;
            });

            WorkShopViewCommand = new RelayCommand(() =>
            {
                CurrentView = WorkShopVM;
            });

            HistoryViewCommand = new RelayCommand(() =>
            {
                CurrentView = HistoryVM;
            });

            SelectionSparesCommand = new RelayCommand(() =>
            {
                CurrentView = SelectionSparesVM;
            });
        }
    }
}
