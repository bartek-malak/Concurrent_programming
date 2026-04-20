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
    }
}
