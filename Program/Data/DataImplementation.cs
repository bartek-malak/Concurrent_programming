using System.Diagnostics;

namespace Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private List<IBall> BallsList = new List<IBall>();

        public override int Width => 500;
        public override int Height => 400;
        public override double BallRadius => 15.0;

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            BallsList.Clear();
            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.NextDouble() * (Width - 2 * BallRadius) + BallRadius, random.NextDouble() * (Height - 2 * BallRadius) + BallRadius);
                Ball newBall = new(startingPosition, BallRadius);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
            }
        }

        public override void Dispose()
        {
            BallsList.Clear();
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
    }
}
