namespace LogicCircuitEditor.Models.LogicalElements
{
    public class OutputElement : LogicalElement
    {
        private bool _signalIn;
        private bool _isConnected;

        public OutputElement()
        {
            SignalIn = false;
        }
        public bool SignalIn
        {
            get => _signalIn;
            set => SetAndRaise(ref _signalIn, value);
        }
        public bool IsConnected
        {
            get => _isConnected;
            set => SetAndRaise(ref _isConnected, value);
        }
    }
}