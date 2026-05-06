using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{
    internal class Ball : IBall
    {
        private readonly Data.IBall _dataBall;

        public event EventHandler<LogicBallEventArgs>? NewPositionNotification;
        public Ball(Data.IBall ball)
        {
            _dataBall = ball;
            _dataBall.NewPositionNotification += RaisePositionChangeEvent;
        }

        public IPosition Position => new Position(_dataBall.Position.x, _dataBall.Position.y);

        public IPosition Velocity { 
            get => new Position(_dataBall.Velocity.x, _dataBall.Velocity.y); 
            set => _dataBall.Velocity = new Data.Vector(value.x, value.y); 
        }

        public double Mass => _dataBall.Mass;

        public double Radius => _dataBall.Radius;

        private void RaisePositionChangeEvent(object? sender, Data.BallEventArgs e)
        {
            NewPositionNotification?.Invoke(this, new LogicBallEventArgs(new Position(e.Position.x, e.Position.y)));
        }
    }
}
