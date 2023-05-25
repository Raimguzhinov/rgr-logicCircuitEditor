namespace LogicCircuitEditor.Models.SerializebleElements
{
    public class SerializebleTwoInputElements : SerializebleLogicalElement
    {
        public string Type { get; set; }
        public bool Input1 { get; set; }
        public bool Input2 { get; set; }
        public bool IsConnectedInput1 { get; set; }
        public bool IsConnectedInput2 { get; set; }
    }
}
