using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using UnderneathLayerAPI = BusinessLogic.BusinessLogicAbstractAPI;

namespace PresentationModel
{
    internal class ModelImplementation : ModelAbstractApi
    {
        public event EventHandler<ModelBallEventArgs>? BallChanged;

        private bool Disposed = false;
        private readonly IObservable<EventPattern<ModelBallEventArgs>>? eventObservable;
        private readonly UnderneathLayerAPI layerBellow;

        internal ModelImplementation() : this(null)
        { }

        internal ModelImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;
            eventObservable = Observable.FromEventPattern<ModelBallEventArgs>(this, "BallChanged");
        }

        public override int Width => layerBellow.GetDimensions().CanvasWidth;

        public override int Height => layerBellow.GetDimensions().CanvasHeight;

        public override double BallRadius => layerBellow.GetDimensions().BallRadius;

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(PresentationModel));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override IDisposable Subscribe(IObserver<IBall> observer)
        {
            return eventObservable!.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

        public override void Start(int numberOfBalls)
        {
            layerBellow.Start(numberOfBalls, StartHandler);
        }

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
        {
            double currentDiameter = layerBellow.GetDimensions().BallRadius * 2;
            ModelBall newBall = new ModelBall(position.y, position.x, ball) { Diameter = currentDiameter };
            BallChanged?.Invoke(this, new () { Ball = newBall });
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        [Conditional("DEBUG")]
        internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls)
        {
            returnNumberOfBalls(layerBellow);
        }

        [Conditional("DEBUG")]
        internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
        {
            returnBallChangedIsNull(BallChanged == null);
        }
    }

    public class ModelBallEventArgs : EventArgs
    {
        public IBall Ball { get; init; }
    }
}
