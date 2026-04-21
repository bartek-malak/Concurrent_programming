using System;
using System.Collections.ObjectModel;
using System.Windows.Input; // Wymagane dla ICommand
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region private fields
        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;
        private int _ballCount; // Pole przechowujące wpisaną liczbę kul
        #endregion private fields

        #region ctor
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
        #endregion ctor

        #region properties
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
        #endregion properties

        #region private methods
        // Metoda wywoływana przez przycisk START
        private void ExecuteStart()
        {
            // Czyścimy poprzednie kule przed nowym startem
            Balls.Clear();
            // Uruchamiamy logikę z pobraną wartością BallCount
            Start(BallCount);
        }
        #endregion private methods

        #region public API
        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));

            ModelLayer.Start(numberOfBalls);
            // Ważne: w oryginalnym kodzie było Observer.Dispose() - 
            // usunąłem to stąd, aby UI nadal dostawało powiadomienia o ruchu kul!
        }
        #endregion public API

        #region IDisposable
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
        #endregion IDisposable
    }
}