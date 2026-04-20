using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{
    internal class Ball : IBall
    {
        public event EventHandler<LogicBallEventArgs>? NewPositionNotification;
        public Ball(Data.IBall ball)
        {
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        private void RaisePositionChangeEvent(object? sender, Data.BallEventArgs e)
        {
            NewPositionNotification?.Invoke(this, new LogicBallEventArgs(new Position(e.Position.x, e.Position.y)));
        }
    }
}
