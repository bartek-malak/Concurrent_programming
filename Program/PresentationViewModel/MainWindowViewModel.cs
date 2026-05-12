using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input; // Wymagane dla ICommand
using PresentationModel;
using PresentationViewModel.MVVMLight;
using ModelIBall = PresentationModel.IBall;


namespace PresentationViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;
        private int _ballCount; // Pole przechowujące wpisaną liczbę kul

        // Przechowuje kontekst głównego wątku UI
        private readonly SynchronizationContext _syncContext;

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;

            // Pobieramy kontekst w momencie tworzenia ViewModelu (tworzy się on na wątku UI)
            _syncContext = SynchronizationContext.Current;

            // Zmieniona subskrypcja z użyciem _syncContext
            Observer = ModelLayer.Subscribe<ModelIBall>(x =>
            {
                // Post działa tak samo jak Dispatcher.Invoke - wrzuca zadanie na wątek UI
                // Jeśli nie ma kontekstu synchronizacji (np. w testach jednostkowych),
                // dodajemy bezpośrednio aby kolekcja otrzymała elementy.
                if (_syncContext != null)
                    _syncContext.Post(_ => Balls.Add(x), null);
                else
                    Balls.Add(x);
            });

            // Inicjalizacja komendy Start
            StartCommand = new RelayCommand(ExecuteStart);
        }

        // Właściwość powiązana z TextBox w XAML
        public int BallCount
        {
            get => _ballCount;
            set
            {
                _ballCount = value;
                RaisePropertyChanged(); // Powiadamiamy UI o zmianie
            }
        }

        // Komenda powiązana z przyciskiem START w XAML
        public ICommand StartCommand { get; }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();


        // Metoda wywoływana przez przycisk START
        private async void ExecuteStart()
        {
            Balls.Clear();
            int count = BallCount;

            // Uruchamiamy tworzenie kul w tle - UI pozostaje responsywne.
            await Task.Run(() =>
            {
                Start(count);
            });
        }
        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));

            ModelLayer.Start(numberOfBalls);
          
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer?.Dispose();
                    ModelLayer.Dispose();
                }
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}