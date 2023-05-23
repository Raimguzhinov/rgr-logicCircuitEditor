using DynamicData.Binding;
using System.Collections.ObjectModel;

namespace LogicCircuitEditor.Models
{
    public class Project : AbstractNotifyPropertyChanged
    {
        private string name = "";
        private ObservableCollection<Scheme> schemes = null!;
        public Project()
        {
            Name = "Project";
            Schemes = new ObservableCollection<Scheme>() { new Scheme() };
        }

        public string Name
        {
            get => name;
            set => SetAndRaise(ref name, value);
        }

        public ObservableCollection<Scheme> Schemes
        {
            get => schemes;
            set => SetAndRaise(ref schemes, value);
        }
    }
}