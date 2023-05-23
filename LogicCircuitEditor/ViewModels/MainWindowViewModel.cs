using LogicCircuitEditor.Models;
using Newtonsoft.Json;
using ReactiveUI;
using System.IO;

namespace LogicCircuitEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object content = null!;
        public MainWindowViewModel()
        {
        }
        public object Content
        {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }
        public void CreateNewProject()
        {
        }
        public void AddNewProjectPath(string name, string path)
        {
            ProjectFile file = new ProjectFile { Name = name, Path = path };
            ProjectFile find = null!;
            JsonSerializerSettings settings =
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                };
            using (StreamWriter writer =
                   new StreamWriter("../../../Assets/projects.json", false))
            {
                
            }
        }
        public void OpenProject(string path)
        {
            if (File.Exists(path))
            {
                
            }
            else
            {
                
            }
        }
    }
}
