using DynamicData.Binding;
using System.Collections.ObjectModel;

namespace LogicCircuitEditor.Models
{
    public class Scheme : AbstractNotifyPropertyChanged
    {
        private string _name = "";
        private ObservableCollection<Element> _elements = null!;
        public Scheme()
        {
            Name = "Scheme";
            Elements = new ObservableCollection<Element>();
        }
        public string Name
        {
            get => _name;
            set => SetAndRaise(ref _name, value);
        }
        public ObservableCollection<Element> Elements
        {
            get => _elements;
            set => SetAndRaise(ref _elements, value);
        }
    }
}