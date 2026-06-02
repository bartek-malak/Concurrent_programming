using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Text;
using Timer = System.Timers.Timer;

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
        private readonly Logger _logger;
        private System.Timers.Timer? _timer;
        private DateTime _lastTick;

        internal Ball(Vector position, double radius, Vector velocity, double mass, Logger logger)
        {
            Position = position;
            Radius = radius;
            Velocity = velocity;
            Mass = mass;
            _logger = logger;
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

            // Ustawiamy czas ostatniego przebiegu na teraz
            _lastTick = DateTime.UtcNow;

            // Timer wywoływany co ~16 ms (60 FPS)
            _timer = new Timer(16);
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, args) =>
            {
                if (!_isRunning) return;

                var now = DateTime.UtcNow;
                double deltaTime = (now - _lastTick).TotalSeconds;
                _lastTick = now;

                if (deltaTime == 0)
                {
                    deltaTime = 0.016; // fallback
                }

                // Nowa pozycja = aktualna pozycja + (Prędkość * czas, który upłynął)
                double newX = Position.x + (Velocity.x * deltaTime);
                double newY = Position.y + (Velocity.y * deltaTime);

                // Aktualizujemy pozycję
                Position = new Vector(newX, newY);

                _logger.Log(new
                {
                    BallId = this.GetHashCode(),
                    X = Math.Round(Position.x, 2),
                    Y = Math.Round(Position.y, 2),
                    VelX = Math.Round(Velocity.x, 2),
                    VelY = Math.Round(Velocity.y, 2)
                });

                // Powiadamiamy warstwę wyższą o zmianie pozycji
                NewPositionNotification?.Invoke(this, new BallEventArgs(Position));
            };

            _timer.Start();
        }

        public void Dispose()
        {
            _isRunning = false;
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
