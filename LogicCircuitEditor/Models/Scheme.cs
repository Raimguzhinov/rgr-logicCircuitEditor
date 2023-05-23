using DynamicData.Binding;
using System.Collections.ObjectModel;

namespace LogicCircuitEditor.Models
{
    public class Scheme : AbstractNotifyPropertyChanged
    {
        private string name = "";
        private ObservableCollection<Element> elements = null!;
        public Scheme()
        {
            Name = "Scheme";
            Elements = new ObservableCollection<Element>();
        }
        public string Name
        {
            get => name;
            set => SetAndRaise(ref name, value);
        }
        public ObservableCollection<Element> Elements
        {
            get => elements;
            set => SetAndRaise(ref elements, value);
        }
    }
}