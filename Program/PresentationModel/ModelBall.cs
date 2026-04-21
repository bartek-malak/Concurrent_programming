using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BusinessLogic;
using LogicIBall = BusinessLogic.IBall;

namespace PresentationModel
{
    internal class ModelBall : IBall
    {
        private double TopBackingField;
        private double LeftBackingField;
        public double Diameter { get; init; } = 0;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ModelBall(double top, double left, LogicIBall underneathBall)
        {
            TopBackingField = top;
            LeftBackingField = left;
            underneathBall.NewPositionNotification += NewPositionNotification;
        }

        public double Top
        {
            get { return TopBackingField - (Diameter / 2); }
            private set
            {
                if (TopBackingField == value)
                    return;
                TopBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return LeftBackingField - (Diameter / 2); }
            private set
            {
                if (LeftBackingField == value)
                    return;
                LeftBackingField = value;
                RaisePropertyChanged();
            }
        }

        private void NewPositionNotification(object? sender, LogicBallEventArgs e)
        {
            Top = e.Position.y; Left = e.Position.x;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        internal void SetLeft(double x)
        { Left = x; }

        [Conditional("DEBUG")]
        internal void SettTop(double x)
        { Top = x; }
    }
}
