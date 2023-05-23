using DynamicData.Binding;
using System.Collections.ObjectModel;

namespace LogicCircuitEditor.Models
{
    public class Project : AbstractNotifyPropertyChanged
    {
        private string _name = "";
        private ObservableCollection<Scheme> _schemes = null!;
        public Project()
        {
            Name = "Project";
            Schemes = new ObservableCollection<Scheme>() { new Scheme() };
        }

        public string Name
        {
            get => _name;
            set => SetAndRaise(ref _name, value);
        }

        public ObservableCollection<Scheme> Schemes
        {
            get => _schemes;
            set => SetAndRaise(ref _schemes, value);
        }
    }
}