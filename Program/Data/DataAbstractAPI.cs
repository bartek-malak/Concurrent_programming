namespace Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract double BallRadius { get; }

        public abstract void Dispose();
    }

    public interface IBall
    {
        event EventHandler<BallEventArgs> NewPositionNotification;
    }

    public interface IVector
    {
        double x { get; init; }
        double y { get; init; }
    }

    public class BallEventArgs : EventArgs
    {
        public IVector Position { get; }

        public BallEventArgs(IVector position)
        {
            Position = position;
        }
    }
}