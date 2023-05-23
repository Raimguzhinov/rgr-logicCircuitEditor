using Avalonia;
using LogicCircuitEditor.Models.LogicalElements;
using System;

namespace LogicCircuitEditor.Models
{
    public class Connector : Element, IDisposable
    {
        private Point _startPoint;
        private Point _endPoint;
        private LogicalElement _firstElement = null!;
        private LogicalElement _secondElement = null!;
        private bool _connectToFirstInput;
        private bool _connectFromFirstInput;
        private bool _reverseConnection;

        public Connector()
        {
            _reverseConnection = false;
            _connectFromFirstInput = true;
        }

        public Point StartPoint
        {
            get => _startPoint;
            set => SetAndRaise(ref _startPoint, value);
        }

        public Point EndPoint
        {
            get => _endPoint;
            set => SetAndRaise(ref _endPoint, value);
        }
        public LogicalElement FirstElement
        {
            get => _firstElement;
            set
            {
                _firstElement = value;
                if (_firstElement != null)
                {
                    _firstElement.ChangeStartPoint += OnFirstElementPositionChanged;
                }
            }
        }

        public LogicalElement SecondElement
        {
            get => _secondElement;
            set
            {
                _secondElement = value;
                if (_secondElement != null)
                {
                    _secondElement.ChangeStartPoint += OnSecondElementPositionChanged;
                }
            }
        }

        public bool ConnectToFirstInput
        {
            get => _connectToFirstInput;
            set => SetAndRaise(ref _connectToFirstInput, value);
        }
        public bool ConnectFromFirstInput
        {
            get => _connectFromFirstInput;
            set => SetAndRaise(ref _connectFromFirstInput, value);
        }

        public bool ReverseConnection
        {
            get => _reverseConnection;
            set => SetAndRaise(ref _reverseConnection, value);
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
                if (_secondElement is TwoInputsGate two)
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
            if (_reverseConnection)
                EndPoint += e.NewStartPoint - e.OldStartPoint;
            else
                StartPoint += e.NewStartPoint - e.OldStartPoint;
        }
        private void OnSecondElementPositionChanged(object? sender,
                                                    ChangeStartPointEventArgs e)
        {
            if (_reverseConnection)
                StartPoint += e.NewStartPoint - e.OldStartPoint;
            else
                EndPoint += e.NewStartPoint - e.OldStartPoint;
        }
    }
}
