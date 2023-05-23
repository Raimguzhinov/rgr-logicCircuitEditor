namespace LogicCircuitEditor.Models.LogicalElements
{
    public class XorGate : TwoInputsGate
    {
        public override bool Output()
        {
            return input1 ^ input2;
        }
    }
}