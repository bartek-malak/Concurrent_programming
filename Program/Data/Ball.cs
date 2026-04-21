using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    internal class Ball : IBall
    {
        private Vector Position;
        public double Radius { get; }
        public event EventHandler<BallEventArgs>? NewPositionNotification;

        internal Ball(Vector position, double radius)
        {
            Position = position;
            Radius = radius;
        }

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, new BallEventArgs(Position));
        }

        internal void Move(Vector delta, int maxWidth, int maxHeight)
        {
            // Obliczamy nową potencjalną pozycję
            double newX = Position.x + delta.x;
            double newY = Position.y + delta.y;

            // Pilnujemy lewej i prawej krawędzi
            if (newX - Radius < 0) newX = Radius;
            else if (newX + Radius > maxWidth) newX = maxWidth - Radius;

            // Pilnujemy górnej i dolnej krawędzi
            if (newY - Radius < 0) newY = Radius;
            else if (newY + Radius > maxHeight) newY = maxHeight - Radius;

            // Zapisujemy skorygowaną pozycję
            Position = new Vector(newX, newY);
            RaiseNewPositionChangeNotification();
        }
    }
}
