using System;
using System.Collections.ObjectModel;
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

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            // Rejestrujemy obserwatora, który doda każdą nową kulę do kolekcji widocznej w UI
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

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
        private void ExecuteStart()
        {
            // Czyścimy poprzednie kule przed nowym startem
            Balls.Clear();
            // Uruchamiamy logikę z pobraną wartością BallCount
            Start(BallCount);
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