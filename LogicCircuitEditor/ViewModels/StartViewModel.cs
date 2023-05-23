using LogicCircuitEditor.Models;
using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;

namespace LogicCircuitEditor.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private int index;
        private ObservableCollection<ProjectFile> projects = null!;

        public StartViewModel()
        {
            Projects = new ObservableCollection<ProjectFile>();
            Index = -1;
            Serialize();
        }

        private void Serialize()
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Newtonsoft.Json.Formatting.Indented
                };
                string Serialized;
                using (StreamReader reader =
                       new StreamReader("../../../Assets/projects.json"))
                {
                    Serialized = reader.ReadToEnd();
                }
                Projects =
                    JsonConvert.DeserializeObject<ObservableCollection<ProjectFile>>(
                        Serialized, settings);
            }
            catch
            {
            }
        }

        public ObservableCollection<ProjectFile> Projects
        {
            get => projects;
            set => this.RaiseAndSetIfChanged(ref projects, value);
        }
        public int Index
        {
            get => index;
            set => this.RaiseAndSetIfChanged(ref index, value);
        }
    }
}