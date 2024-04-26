using Aspose.Words.Replacing;
using Aspose.Words;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diplom1.MVVM.Model;
using Diplom1.MVVM.Model.Cars;
using Diplom1.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.IO;

namespace Diplom1.MVVM.ViewModel
{
    public class HistoryViewModel : ObservableObject
    {
        private string _statusTitleBarSpares = "Hidden";

        private readonly HistoryPayRepository _historyPayRepository;
        private readonly WorkShopRepository _workShopRepository;
        private ObservableCollection<HistoryPayModel> _history;
        private GetMessage _getMessage;

        public GetMessage GetMessage
        {
            get { return _getMessage; }
            set
            {
                _getMessage = value;
                OnPropertyChanged(nameof(GetMessage));
            }
        }
        public ObservableCollection<HistoryPayModel> History
        {
            get { return _history; }
            set
            {
                _history = value;
                OnPropertyChanged(nameof(History));
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

        public RelayCommand<object> GenerateReceiptCommand { get; set; }

        public HistoryViewModel()
        {
            _historyPayRepository = new HistoryPayRepository();
            _workShopRepository = new WorkShopRepository();

            var shopInfo = _workShopRepository.GetByShopInfo();
            History = new ObservableCollection<HistoryPayModel>(_historyPayRepository.GetAllHistory(shopInfo.Id));
            if (History.Count == 0)
                StatusTitleBarSpares = "Visible";
            else
                StatusTitleBarSpares = "Hidden";

            GenerateReceiptCommand = new RelayCommand<object>(GenerateReceipt);
        }

        private void GenerateReceipt(object parameter)
        {
            if (parameter is HistoryPayModel selectedHistory)
            {
                string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string inputFilePath = Path.Combine(directory, "Receipt", "Example.docx");
                string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string outputFilePath = Path.Combine(downloadsPath, "Desktop", $"receipt_{DateTime.Today:yyyy-MM-dd}_{selectedHistory.Id}.pdf");

                var replacements = new Dictionary<string, string>
                {
                    {"<Num>", $"{selectedHistory.WorkShopId}-{selectedHistory.SparesId}.{selectedHistory.Id}" },
                    {"<Cashier>", $"{selectedHistory.WorkShopName}" },
                    {"<InfoService1>", $"{selectedHistory.Name}" }, {"<Quantity1>", $"{selectedHistory.Amount} шт." }, {"<Cost1>", $"{selectedHistory.Price}" },
                    {"<NumFD>", "Неизвестно" },
                    {"<NumFP>", "Неизвестно" },
                    {"<NumCash>", "1" },
                    {"<NumComing>", "Неизвестно" },
                    {"<NDS>", "13" },
                    {"<DateTime>", $"{selectedHistory.DateTime}" },
                    {"<Sum>", $"{selectedHistory.Price}" },
                };
                ReplaceTag(inputFilePath, outputFilePath, replacements);

                GetMessage = new GetMessage
                {
                    Message = $"* Чек успешно загружен на рабочий стол",
                    TextColor = Brushes.Green
                };
            }
        }
        private static bool ReplaceTag(string inputFilePath, string outputFilePath, Dictionary<string, string> replacements)
        {
            try
            {
                Document doc = new(inputFilePath);

                foreach (var replacement in replacements)
                {
                    doc.Range.Replace(replacement.Key, replacement.Value, new FindReplaceOptions());
                }

                doc.Save(outputFilePath, SaveFormat.Pdf);

                return true;
            }
            catch
            {
                throw new InvalidOperationException("Ошибка при формировании чека");
            }
        }
    }
}
