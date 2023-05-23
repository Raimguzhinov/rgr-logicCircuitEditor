using LogicCircuitEditor.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;

namespace LogicCircuitEditor.ViewModels
{
    public class MainUserControlViewModel : ViewModelBase
    {
        private ObservableCollection<Element> _elements = null!;
        private Project _project = null!;
        private int _index;
        private bool isInput = false;
        private bool isOutput = false;
        private bool isNotGate = false;
        private bool isAndGate = false;
        private bool isOrGate = false;
        private bool isXorGate = false;
        private bool isDecoderGate = false;
        private bool changeElements = false;
        public MainUserControlViewModel()
        {
            Elements = new ObservableCollection<Element>();
            Project = new Project();
            IsInput = true;
            Index = 0;
            AddScheme = ReactiveCommand.Create(() =>
            {
                Project.Schemes.Add(new Scheme());
                Index = Project.Schemes.Count - 1;
            });
            DeleteScheme = ReactiveCommand.Create(() =>
            {
                if (Project.Schemes.Count > 1)
                {
                    int index = _index;
                    if (_index != 0)
                    {
                        Index = 0;
                        Project.Schemes.RemoveAt(index);
                    }
                    else
                    {
                        for (int i = 0; i < Project.Schemes.Count - 1; ++i)
                        {
                            Project.Schemes[i].Name = Project.Schemes[i + 1].Name;
                            Project.Schemes[i].Elements = Project.Schemes[i + 1].Elements;
                        }
                        changeElements = true;
                        Index = 0;
                        changeElements = false;
                        Project.Schemes.RemoveAt(Project.Schemes.Count - 1);
                    }
                }
            });
            CreateProject = ReactiveCommand.Create(() =>
            {
                Index = 0;
                changeElements = true;
                Project = new Project();
                changeElements = false;
                Index = 0;
            });
        }
        public Project Project
        {
            get => _project;
            set => this.RaiseAndSetIfChanged(ref _project, value);
        }

        public ObservableCollection<Element> Elements
        {
            get => _elements;
            set
            {
                this.RaiseAndSetIfChanged(ref _elements, value);
            }
        }
        
        public int Index
        {
            get => _index;
            set
            {
                if (!changeElements)
                    Project.Schemes[_index].Elements = Elements;
                if (value != -1)
                    this.RaiseAndSetIfChanged(ref _index, value);
                Elements = Project.Schemes[_index].Elements;
            }
        }
        public bool IsInput
        {
            get => isInput;
            set => this.RaiseAndSetIfChanged(ref isInput, value);
        }
        public bool IsOutput
        {
            get => isOutput;
            set => this.RaiseAndSetIfChanged(ref isOutput, value);
        }
        public bool IsNotGate
        {
            get => isNotGate;
            set => this.RaiseAndSetIfChanged(ref isNotGate, value);
        }
        public bool IsAndGate
        {
            get => isAndGate;
            set => this.RaiseAndSetIfChanged(ref isAndGate, value);
        }
        public bool IsOrGate
        {
            get => isOrGate;
            set => this.RaiseAndSetIfChanged(ref isOrGate, value);
        }
        public bool IsXorGate
        {
            get => isXorGate;
            set => this.RaiseAndSetIfChanged(ref isXorGate, value);
        }
        public bool IsDecoderGate
        {
            get => isDecoderGate;
            set => this.RaiseAndSetIfChanged(ref isDecoderGate, value);
        }

        public bool ChangeElements
        {
            get => changeElements;
            set => changeElements = value;
        }

        public ReactiveCommand<Unit, Unit> AddScheme { get; }
        public ReactiveCommand<Unit, Unit> DeleteScheme { get; }
        public ReactiveCommand<Unit, Unit> CreateProject { get; }
    }
}
