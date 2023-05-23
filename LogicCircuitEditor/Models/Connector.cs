using Avalonia;
using LogicCircuitEditor.Models.LogicalElements;
using System;

namespace LogicCircuitEditor.Models
{
    public class Connector : Element, IDisposable
    {
        private Point startPoint;
        private Point endPoint;
        private LogicalElement firstElement = null!;
        private LogicalElement secondElement = null!;
        private bool connectToFirstInput;
        private bool connectFromFirstInput;
        private bool reverseConnection;

        public Connector()
        {
            reverseConnection = false;
            connectFromFirstInput = true;
        }

        public Point StartPoint
        {
            get => startPoint;
            set => SetAndRaise(ref startPoint, value);
        }

        public Point EndPoint
        {
            get => endPoint;
            set => SetAndRaise(ref endPoint, value);
        }
        public LogicalElement FirstElement
        {
            get => firstElement;
            set
            {
                firstElement = value;
                if (firstElement != null)
                {
                    firstElement.ChangeStartPoint += OnFirstElementPositionChanged;
                }
            }
        }

        public LogicalElement SecondElement
        {
            get => secondElement;
            set
            {
                secondElement = value;
                if (secondElement != null)
                {
                    secondElement.ChangeStartPoint += OnSecondElementPositionChanged;
                }
            }
        }

        public bool ConnectToFirstInput
        {
            get => connectToFirstInput;
            set => SetAndRaise(ref connectToFirstInput, value);
        }
        public bool ConnectFromFirstInput
        {
            get => connectFromFirstInput;
            set => SetAndRaise(ref connectFromFirstInput, value);
        }

        public bool ReverseConnection
        {
            get => reverseConnection;
            set => SetAndRaise(ref reverseConnection, value);
        }

        public void Dispose()
        {
            if (FirstElement != null)
            {
                FirstElement.ChangeStartPoint -= OnFirstElementPositionChanged;
            }
            if (SecondElement != null)
            {
                if (SecondElement is OutputElement output)
                {
                    output.IsConnected = false;
                    output.SignalIn = false;
                }
                if (SecondElement is NotGate not)
                {
                    not.IsConnected = false;
                    not.Input = false;
                }
                if (secondElement is TwoInputsGate two)
                {
                    if (ConnectToFirstInput)
                    {
                        two.IsConnectedInput1 = false;
                        two.Input1 = false;
                    }
                    else
                    {
                        two.IsConnectedInput2 = false;
                        two.Input2 = false;
                    }
                }
                SecondElement.ChangeStartPoint -= OnSecondElementPositionChanged;
            }
        }

        private void OnFirstElementPositionChanged(object? sender,
                                                   ChangeStartPointEventArgs e)
        {
            if (reverseConnection)
                EndPoint += e.NewStartPoint - e.OldStartPoint;
            else
                StartPoint += e.NewStartPoint - e.OldStartPoint;
        }
        private void OnSecondElementPositionChanged(object? sender,
                                                    ChangeStartPointEventArgs e)
        {
            if (reverseConnection)
                StartPoint += e.NewStartPoint - e.OldStartPoint;
            else
                EndPoint += e.NewStartPoint - e.OldStartPoint;
        }
    }
}
