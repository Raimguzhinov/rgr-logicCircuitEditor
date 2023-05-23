using DynamicData.Binding;

namespace LogicCircuitEditor.Models
{
    public abstract class Element : AbstractNotifyPropertyChanged
    {
        private static uint id_generator = 0;
        protected uint id;
        protected bool focusOnElement;
        public Element()
        {
            id = id_generator++;
            FocusOnElement = false;
        }

        public uint ID { get => id; set => SetAndRaise(ref id, value); }
        public bool FocusOnElement
        {
            get => focusOnElement;
            set => SetAndRaise(ref focusOnElement, value);
        }
    }
}