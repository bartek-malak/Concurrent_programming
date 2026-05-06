using System.Diagnostics;
using System.Numerics;
using Data;
using UnderneathLayerAPI = Data.DataAbstractAPI;

namespace BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private readonly UnderneathLayerAPI layerBellow;
        private bool Disposed = false;
        private readonly object _collisionLock = new object(); // Obiekt do sekcji krytycznej
        private List<IBall> _logicBallsList = new List<IBall>();

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override Dimensions GetDimensions()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));

            return new Dimensions(
                layerBellow.BallRadius,
                layerBellow.Height,
                layerBellow.Width
            );
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            _logicBallsList.Clear();

            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            layerBellow.Start(numberOfBalls, (startingPosition, databall) => {
                var logicBall = new Ball(databall);
                _logicBallsList.Add(logicBall);

                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        private void OnDataBallCreated(IVector position, Data.IBall dataBall)
        {
            var logicBall = new Ball(dataBall);
            _logicBallsList.Add(logicBall);
        }

        private void OnBallMoved(object? sender, BallEventArgs e)
        {
            if (sender is IBall ball)
            {
                // SEKCJA KRYTYCZNA - blokujemy dostęp dla innych wątków na czas obliczeń
                lock (_collisionLock)
                {
                    CheckWallCollisions(ball);
                    CheckBallCollisions(ball);
                }
            }
        }

        private void CheckWallCollisions(IBall ball)
        {
            double newVelX = ball.Velocity.x;
            double newVelY = ball.Velocity.y;

            // Prawa i lewa ściana
            if (ball.Position.x + ball.Radius >= layerBellow.Width || ball.Position.x - ball.Radius <= 0)
            {
                newVelX = -ball.Velocity.x; // Odwracamy kierunek X
            }

            // Dolna i górna ściana
            if (ball.Position.y + ball.Radius >= layerBellow.Height || ball.Position.y - ball.Radius <= 0)
            {
                newVelY = -ball.Velocity.y; // Odwracamy kierunek Y
            }

            // Jeśli wektor się zmienił to przypisujemy nowy
            if (newVelX != ball.Velocity.x || newVelY != ball.Velocity.y)
            {
                ball.Velocity = new Position(newVelX, newVelY);
            }
        }

        private void CheckBallCollisions(IBall currentBall)
        {
            foreach (var otherBall in _logicBallsList)
            {
                if (currentBall == otherBall) continue; // Nie sprawdza kolizji sama z sobą

                double dx = otherBall.Position.x - currentBall.Position.x;
                double dy = otherBall.Position.y - currentBall.Position.y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                // Czy kule się stykają lub nachodzą na siebie?
                if (distance <= currentBall.Radius + otherBall.Radius)
                {

                    // OBLICZANIE NOWYCH PRĘDKOŚCI (Zderzenie sprężyste w 1D uproszczone dla wektorów X i Y)
                    // Korzystamy z prawa zachowania pędu:
                    double m1 = currentBall.Mass;
                    double m2 = otherBall.Mass;

                    double v1x = currentBall.Velocity.x;
                    double v2x = otherBall.Velocity.x;

                    double v1y = currentBall.Velocity.y;
                    double v2y = otherBall.Velocity.y;

                    // Wzory na nowe prędkości:
                    double newV1x = (v1x * (m1 - m2) + 2 * m2 * v2x) / (m1 + m2);
                    double newV1y = (v1y * (m1 - m2) + 2 * m2 * v2y) / (m1 + m2);

                    double newV2x = (v2x * (m2 - m1) + 2 * m1 * v1x) / (m1 + m2);
                    double newV2y = (v2y * (m2 - m1) + 2 * m1 * v1y) / (m1 + m2);

                    // Przypisanie nowych wektorów
                    currentBall.Velocity = new Position(newV1x, newV1y);
                    otherBall.Velocity = new Position(newV2x, newV2y);
                }
            }
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}
