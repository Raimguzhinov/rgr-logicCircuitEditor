using LogicCircuitEditor.Models;
using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;

namespace LogicCircuitEditor.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private int _index;
        private ObservableCollection<ProjectFile> _projects = null!;

        public StartViewModel()
        {
            Projects = new ObservableCollection<ProjectFile>();
            Index = -1;
            LoadProjects();
        }

        private void LoadProjects()
        {
            try
            {
                // Чтение сериализованных данных из файла
                string serialized;
                using (StreamReader reader = new StreamReader("../../../Assets/projects.json"))
                {
                    serialized = reader.ReadToEnd();
                }

                // Десериализация списка проектов
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Newtonsoft.Json.Formatting.Indented
                };
                Projects = JsonConvert.DeserializeObject<ObservableCollection<ProjectFile>>(serialized, settings);
            }
            catch
            {
                // Обработка ошибки чтения или десериализации
            }
        }

        public ObservableCollection<ProjectFile> Projects
        {
            get => _projects;
            set => this.RaiseAndSetIfChanged(ref _projects, value);
        }

        public int Index
        {
            get => _index;
            set => this.RaiseAndSetIfChanged(ref _index, value);
        }
    }
}