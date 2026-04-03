using System;
using System.Collections.Generic;
using System.Text;

namespace Concurrent_programming.Data
{
    internal class Ball : IBall
    {
        private double _x;
        private double _y;
        public double Radius { get; }
        public event EventHandler<BallEventArgs> NewPositionNotification;

        internal Ball(double x, double y, double radius)
        {
            _x = x;
            _y = y;
            Radius = radius;
        }

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                RaiseNewPositionChangeNotification();
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                RaiseNewPositionChangeNotification();
            }
        }

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, new BallEventArgs(_x, _y));
        }
    }
}
