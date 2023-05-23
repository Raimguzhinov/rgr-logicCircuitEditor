namespace LogicCircuitEditor.Models.LogicalElements
{
    public class OrGate : TwoInputsGate
    {
        public override bool Output()
        {
            return input1 || input2;
        }
    }
}