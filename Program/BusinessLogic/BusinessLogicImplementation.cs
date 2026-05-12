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
                layerBellow.Height,
                layerBellow.Width
                , layerBellow.BallRadius
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

                logicBall.NewPositionNotification += OnBallMoved;

                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        private void OnBallMoved(object? sender, LogicBallEventArgs e)
        {
            if (sender is IBall ball)
            {
                // SEKCJA KRYTYCZNA - blokujemy dostęp dla innych wątków na czas obliczeń
                lock (_collisionLock)
                {
                    CheckBallCollisions(ball);
                    CheckWallCollisions(ball);
                }
            }
        }

        private void CheckWallCollisions(IBall ball)
        {
            int boardWidth = layerBellow.Width;
            int boardHeight = layerBellow.Height;

            double newVelX = ball.Velocity.x;
            double newVelY = ball.Velocity.y;

            // PRAWA i LEWA ściana (zabezpieczone znakiem prędkości)
            if (ball.Position.x + ball.Radius >= boardWidth && newVelX > 0)
            {
                newVelX = -newVelX;
            }
            else if (ball.Position.x - ball.Radius <= 0 && newVelX < 0)
            {
                newVelX = -newVelX;
            }

            // DOLNA i GÓRNA ściana (zabezpieczone znakiem prędkości)
            if (ball.Position.y + ball.Radius >= boardHeight && newVelY > 0)
            {
                newVelY = -newVelY;
            }
            else if (ball.Position.y - ball.Radius <= 0 && newVelY < 0)
            {
                newVelY = -newVelY;
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
                if (currentBall == otherBall) continue;

                // Wektor różnicy pozycji (x1 - x2) i (y1 - y2)
                double dx = currentBall.Position.x - otherBall.Position.x;
                double dy = currentBall.Position.y - otherBall.Position.y;

                // Odległość między środkami do kwadratu
                double distanceSquared = dx * dx + dy * dy;
                double radiusSum = currentBall.Radius + otherBall.Radius;

                // Czy kule na siebie nachodzą?
                if (distanceSquared <= radiusSum * radiusSum) // Używamy kwadratów, by uniknąć wolnego Math.Sqrt
                {
                    // Wektor różnicy prędkości (v1 - v2)
                    double dvx = currentBall.Velocity.x - otherBall.Velocity.x;
                    double dvy = currentBall.Velocity.y - otherBall.Velocity.y;

                    // ILOCZYN SKALARNY (Dot Product) wektora prędkości i pozycji
                    double dotProduct = dvx * dx + dvy * dy;

                    // Jeśli dotProduct > 0, to kule już się od siebie oddalają
                    // Przerywamy obliczenia, żeby nie odwrócić wektorów po raz drugi (zapobiega sklejaniu).
                    if (dotProduct > 0)
                        continue;

                    // KOLIZJE
                    double totalMass = currentBall.Mass + otherBall.Mass;

                    // Współczynnik dla pierwszej kuli: (2 * m2 / (m1 + m2)) * (dotProduct / distanceSquared)
                    double collisionScale1 = (2 * otherBall.Mass / totalMass) * (dotProduct / distanceSquared);

                    // Współczynnik dla drugiej kuli: (2 * m1 / (m1 + m2)) * (dotProduct / distanceSquared)
                    double collisionScale2 = (2 * currentBall.Mass / totalMass) * (dotProduct / distanceSquared);

                    // Obliczenie nowych wektorów (v1' = v1 - scale * dx)
                    double newV1x = currentBall.Velocity.x - collisionScale1 * dx;
                    double newV1y = currentBall.Velocity.y - collisionScale1 * dy;

                    // Dla drugiej kuli dodajemy (zamiast odejmować), bo różnica pozycji x2-x1 to -dx
                    double newV2x = otherBall.Velocity.x + collisionScale2 * dx;
                    double newV2y = otherBall.Velocity.y + collisionScale2 * dy;

                    // Przypisanie nowych prędkości
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
