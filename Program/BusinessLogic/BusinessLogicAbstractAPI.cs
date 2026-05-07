using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Data;

namespace BusinessLogic
{
    public abstract class BusinessLogicAbstractAPI : IDisposable
    {
        private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());

        public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
        {
            return modelInstance.Value;
        }
        public abstract Dimensions GetDimensions();

        public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);

        public abstract void Dispose();

    }
    public record Dimensions(int CanvasHeight, int CanvasWidth);

    public interface IPosition
    {
        double x { get; init; }
        double y { get; init; }
    }

    public interface IBall
    {
        IPosition Position { get; }
        IPosition Velocity { get; set; }
        double Mass { get; }
        double Radius { get; }

        event EventHandler<LogicBallEventArgs> NewPositionNotification;
    }


    public class LogicBallEventArgs : EventArgs
    {
        public IPosition Position { get; }

        public LogicBallEventArgs(IPosition position)
        {
            Position = position;
        }
    }
}
