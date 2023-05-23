using DynamicData.Binding;

namespace LogicCircuitEditor.Models
{
    public abstract class Element : AbstractNotifyPropertyChanged
    {
        private static uint _idGenerator = 0;
        protected uint Id;
        protected bool focusOnElement;
        public Element()
        {
            Id = _idGenerator++;
            FocusOnElement = false;
        }

        public uint ID { get => Id; set => SetAndRaise(ref Id, value); }
        public bool FocusOnElement
        {
            get => focusOnElement;
            set => SetAndRaise(ref focusOnElement, value);
        }
    }
}