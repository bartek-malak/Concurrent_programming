using System;
using System.Collections.Generic;
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
                while (_isRunning)
                {
                    Move();

                    await Task.Delay(16);
                }
            });
        }
    }
}
