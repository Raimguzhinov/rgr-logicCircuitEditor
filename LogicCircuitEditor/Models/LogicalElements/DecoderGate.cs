namespace LogicCircuitEditor.Models.LogicalElements
{
    public class DecoderGate : TwoInputsGate
    {
        public override bool Output()
        {
            return input1 && !input2;
        }

        public bool Output2()
        {
            return input1 && input2;
        }
    }
}