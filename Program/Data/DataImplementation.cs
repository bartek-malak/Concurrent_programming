using System.Diagnostics;
using System;

namespace Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private List<IBall> BallsList = new List<IBall>();

        public override int Width => 370;
        public override int Height => 310;
        public override double BallRadius => 15.0;

        private bool Disposed = false;
        private readonly Timer MoveTimer;
        private Random RandomGenerator = new();


        public DataImplementation()
        {
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
        }

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.NextDouble() * (Width - 2 * BallRadius) + BallRadius, random.NextDouble() * (Height - 2 * BallRadius) + BallRadius);
                Ball newBall = new(startingPosition, BallRadius);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
            }
        }

        private void Move(object? x)
        {
           
            foreach (var item in BallsList.ToList())
            {
                if (item is Ball ball)
                {
                    // 1. Generujemy losowy wektor przesunięcia
                    double deltaX = (RandomGenerator.NextDouble() - 0.5) * 10;
                    double deltaY = (RandomGenerator.NextDouble() - 0.5) * 10;
                    Vector delta = new Vector(deltaX, deltaY);

                    // 2. Wywołujemy ruch kuli, przekazując jej granice świata zdefiniowane w DataImplementation
                    ball.Move(delta, Width, Height);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    BallsList.Clear();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}
