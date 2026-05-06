using System.Diagnostics;
using System;

namespace Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private List<IBall> BallsList = new List<IBall>();

        public override int Width => 370;
        public override int Height => 310;

        private bool Disposed = false;
        private Random RandomGenerator = new();


        public DataImplementation()
        {
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
                double BallRadius = 10.0;
                double mass = 10.0;
                Vector startingPosition = new(random.NextDouble() * (Width - 2 * BallRadius) + BallRadius, random.NextDouble() * (Height - 2 * BallRadius) + BallRadius);
                Vector startingVelocity = new((random.NextDouble() * 10) - 5, (random.NextDouble() * 10) - 5);
                
                Ball newBall = new(startingPosition, BallRadius, startingVelocity, mass);
                BallsList.Add(newBall);
                upperLayerHandler(startingPosition, newBall);
                newBall.StartMovement();
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

        public override IEnumerable<IBall> GetBalls()
        {
            return BallsList;
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
