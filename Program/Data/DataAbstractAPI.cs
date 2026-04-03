namespace Concurrent_programming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        public abstract void Start(int numberOfBalls);

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract double BallRadius { get; }
        public abstract IEnumerable<IBall> GetBalls();

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

        public abstract void Dispose();
    }

    public interface IVector
    {
        double x { get; }
        double y { get; }
    }

    public interface IBall
    {
        double X { get; }
        double Y { get; }
        double Radius { get; }

        event EventHandler<BallEventArgs> NewPositionNotification;
    }

    public class BallEventArgs : EventArgs
    {
        public double X { get; }
        public double Y { get; }

        public BallEventArgs(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}