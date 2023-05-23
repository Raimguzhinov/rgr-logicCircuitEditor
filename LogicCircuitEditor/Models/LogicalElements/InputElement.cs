namespace LogicCircuitEditor.Models.LogicalElements
{
    public class InputElement : LogicalElement
    {
        private bool signalOut;

        public InputElement()
        {
            SignalOut = false;
        }

        public bool SignalOut
        {
            get => signalOut;
            set => SetAndRaise(ref signalOut, value);
        }
    }
}