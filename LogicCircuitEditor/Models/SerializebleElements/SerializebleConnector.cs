namespace LogicCircuitEditor.Models.SerializebleElements
{
    public class SerializebleConnector : Element
    {
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public uint FirstElement { get; set; }
        public uint SecondElement { get; set; }
        public bool ConnectToFirstInput { get; set; }
        public bool ConnectFromFirstInput { get; set; }
        public bool ReverseConnection { get; set; }
    }
}