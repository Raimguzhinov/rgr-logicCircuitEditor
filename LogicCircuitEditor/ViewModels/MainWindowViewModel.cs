using LogicCircuitEditor.Models;
using Newtonsoft.Json;
using ReactiveUI;
using System.IO;

namespace LogicCircuitEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object _content = null!;
        private MainUserControlViewModel _mainModel;
        private StartViewModel _startModel;

        public MainWindowViewModel()
        {
            _mainModel = new MainUserControlViewModel();
            _startModel = new StartViewModel();
            Content = _startModel;
        }

        public object Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public void CreateNewProject()
        {
            _mainModel.Project = new Project();
            Content = _mainModel;
        }

        public void AddNewProjectPath(string name, string path)
        {
            ProjectFile file = new ProjectFile { Name = name, Path = path };
            ProjectFile find = null!;
            
            // Поиск проекта по пути
            foreach (ProjectFile item in _startModel.Projects)
            {
                if (item.Path == path)
                {
                    find = item;
                    break;
                }
            }
            
            // Если проект найден, удаляем его из списка и добавляем в начало
            if (find != null)
            {
                _startModel.Projects.Remove(find);
                _startModel.Projects.Insert(0, file);
            }
            else
            {
                _startModel.Projects.Insert(0, file);
            }
            
            // Сериализация списка проектов в JSON
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
            
            string serialized = JsonConvert.SerializeObject(_startModel.Projects, settings);
            
            // Запись сериализованных данных в файл
            using (StreamWriter writer = new StreamWriter("../../../Assets/projects.json", false))
            {
                writer.WriteLine(serialized);
            }
        }

        public void OpenProject(string path)
        {
            if (File.Exists(path))
            {
                // Открытие проекта
                Content = _mainModel;
                _mainModel.Index = 0;
                _mainModel.ChangeElements = true;
                _mainModel.Project = LogicCircuitEditor.Models.Serializer.Load(path);
                _mainModel.ChangeElements = false;
                _mainModel.Index = 0;
            }
            else
            {
                // Если файл не существует, удаляем проект из списка
                _startModel.Projects.RemoveAt(_startModel.Index);
            }
        }
    }
}
