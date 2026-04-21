using System.ComponentModel;

namespace PresentationModel
{
    public abstract class ModelAbstractApi : IObservable<IBall>, IDisposable
    {
        private static Lazy<ModelAbstractApi> modelInstance = new Lazy<ModelAbstractApi>(() => new ModelImplementation());

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract double BallRadius { get; }
        public static ModelAbstractApi CreateModel()
        {
            return modelInstance.Value;
        }

        public abstract void Start(int numberOfBalls);

        public abstract IDisposable Subscribe(IObserver<IBall> observer);

        public abstract void Dispose();
    }

    public interface IBall : INotifyPropertyChanged
    {
        double Top { get; }
        double Left { get; }
        double Diameter { get; }
    }
}
