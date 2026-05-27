using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Data
{
    internal class Ball : IBall
    {
        public IVector Position { get; private set; }
        public IVector Velocity { get; set; }
        public double Mass { get; }
        public double Radius { get; }

        public event EventHandler<BallEventArgs>? NewPositionNotification;
        private bool _isRunning = false;

        internal Ball(Vector position, double radius, Vector velocity, double mass)
        {
            Position = position;
            Radius = radius;
            Velocity = velocity;
            Mass = mass;
        }

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, new BallEventArgs(Position));
        }

        internal void Move()
        {
            // Obliczamy nową pozycję
            double newX = Position.x + Velocity.x;
            double newY = Position.y + Velocity.y;

            // Zapisujemy pozycję
            Position = new Vector(newX, newY);
            RaiseNewPositionChangeNotification();
        }

        public void StartMovement()
        {
            _isRunning = true;

            Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (_isRunning)
                {
                    // Pobieramy dokładny czas, jaki upłynął od ostatniego obrotu pętli (w sekundach)
                    double deltaTime = stopwatch.Elapsed.TotalSeconds;

                    // Resetujemy stoper żeby mierzył czas do następnej klatki
                    stopwatch.Restart();

                    // Zabezpieczenie (gdyby pierwszy obrót wykonał się w 0 sekund)
                    if (deltaTime == 0)
                    {
                        deltaTime = 0.016; // 60 klatek na sekundę
                    }

                    // Nowa pozycja = aktualna pozycja + (Prędkość * czas, który upłynął)
                    double newX = Position.x + (Velocity.x * deltaTime);
                    double newY = Position.y + (Velocity.y * deltaTime);

                    // Aktualizujemy pozycję
                    Position = new Vector(newX, newY);

                    // Powiadamiamy warstwę wyższą o zmianie pozycji
                    NewPositionNotification?.Invoke(this, new BallEventArgs(Position));

                    // Jeśli procesor się spóźni to i tak stoper nadgoni w kolejnym kroku - zmierzy większy deltaTime.
                    await Task.Delay(16);
                }
            });
        }

        public void Dispose()
        {
            _isRunning = false;
        }
    }
}
