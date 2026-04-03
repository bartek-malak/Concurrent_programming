namespace Concurrent_programming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private Random _random = new();
        private List<IBall> _balls = new List<IBall>();

        public override int Width => 500;
        public override int Height => 400;
        public override double BallRadius => 15.0;

        public override void Start(int numberOfBalls)
        {
            _balls.Clear();
            for (int i = 0; i < numberOfBalls; i++)
            {
                double x = _random.NextDouble() * (Width - 2 * BallRadius) + BallRadius;
                double y = _random.NextDouble() * (Height - 2 * BallRadius) + BallRadius;
                _balls.Add(new Ball(x, y, BallRadius));
            }
        }

        public override void Dispose()
        {
            _balls.Clear();
        }

        public override IEnumerable<IBall> GetBalls()
        {
            return _balls;
        }
    }
}
