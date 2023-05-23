using LogicCircuitEditor.Models;
using LogicCircuitEditor.Models.LogicalElements;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;

namespace LogicCircuitEditor.ViewModels
{
    public class MainUserControlViewModel : ViewModelBase
    {
        private ObservableCollection<Element> _elements = null!; // Коллекция элементов
        private Project _project = null!;  // Проект
        private Stack<Connector> _connectWithInputElement = null!; // Стек входных соединителей
        private int _index;
        private bool _isInput;
        private bool _isOutput;
        private bool _isNotGate;
        private bool _isAndGate;
        private bool _isOrGate;
        private bool _isXorGate;
        private bool _isDecoderGate;
        private bool _changeElements;
        public MainUserControlViewModel()
        {
            Elements = new ObservableCollection<Element>();
            Project = new Project();
            IsInput = true;
            Index = 0;
            
            // Команда удаления выбранного элемента
            DeleteFocusedElement = ReactiveCommand.Create(() =>
            {
                for (int i = 0; i < Elements.Count; ++i)
                {
                    if (Elements[i].FocusOnElement)
                    {
                        if (Elements[i] is Connector connector)
                            connector.Dispose();
                        if (Elements[i] is LogicalElement)
                        {
                            for (int j = i + 1; j < Elements.Count;)
                            {
                                if (Elements[j] is Connector con)
                                {
                                    if (con.FirstElement == Elements[i] ||
                                        con.SecondElement == Elements[i])
                                    {
                                        con.Dispose();
                                        Elements.RemoveAt(j);
                                    }
                                    else
                                        j++;
                                }
                                else
                                    j++;
                            }
                        }
                        Elements.RemoveAt(i);
                        CalcInputConnectors();
                        SignalBypass();
                        break;
                    }
                }
            });
            
            // Команда добавления схемы
            AddScheme = ReactiveCommand.Create(() =>
            {
                Project.Schemes.Add(new Scheme());
                Index = Project.Schemes.Count - 1;
            });
            
            // Команда удаления схемы
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
                        _changeElements = true;
                        Index = 0;
                        _changeElements = false;
                        Project.Schemes.RemoveAt(Project.Schemes.Count - 1);
                    }
                }
            });
            
            // Команда создания проекта
            CreateProject = ReactiveCommand.Create(() =>
            {
                Index = 0;
                _changeElements = true;
                Project = new Project();
                _changeElements = false;
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
                CalcInputConnectors();
            }
        }

        public Stack<Connector> ConnectWithInputElement
        {
            get => _connectWithInputElement;
            set => _connectWithInputElement = value;
        }

        public int Index
        {
            get => _index;
            set
            {
                if (!_changeElements)
                    Project.Schemes[_index].Elements = Elements;
                if (value != -1)
                    this.RaiseAndSetIfChanged(ref _index, value);
                Elements = Project.Schemes[_index].Elements;
            }
        }
        public bool IsInput
        {
            get => _isInput;
            set => this.RaiseAndSetIfChanged(ref _isInput, value);
        }
        public bool IsOutput
        {
            get => _isOutput;
            set => this.RaiseAndSetIfChanged(ref _isOutput, value);
        }
        public bool IsNotGate
        {
            get => _isNotGate;
            set => this.RaiseAndSetIfChanged(ref _isNotGate, value);
        }
        public bool IsAndGate
        {
            get => _isAndGate;
            set => this.RaiseAndSetIfChanged(ref _isAndGate, value);
        }
        public bool IsOrGate
        {
            get => _isOrGate;
            set => this.RaiseAndSetIfChanged(ref _isOrGate, value);
        }
        public bool IsXorGate
        {
            get => _isXorGate;
            set => this.RaiseAndSetIfChanged(ref _isXorGate, value);
        }
        public bool IsDecoderGate
        {
            get => _isDecoderGate;
            set => this.RaiseAndSetIfChanged(ref _isDecoderGate, value);
        }

        public bool ChangeElements
        {
            get => _changeElements;
            set => _changeElements = value;
        }

        // Расчет входных соединителей
        private void CalcInputConnectors()
        {
            ConnectWithInputElement = new Stack<Connector>();
            foreach (Element element in Elements)
            {
                if (element is Connector con)
                {
                    if (con.FirstElement is InputElement)
                        ConnectWithInputElement.Push(con);
                }
            }
        }

        // Пропуск сигнала через элементы схемы от входных элементов к выходным элементам через соединители
        public void SignalBypass()
        {
            // Создаем стек соединителей, начиная с входных соединителей
            Stack<Connector> con = new Stack<Connector>(new Stack<Connector>(ConnectWithInputElement));
            while (con.Count != 0)
            {
                Connector connector = con.Pop();
                if (connector.FirstElement is InputElement inputElement)
                {
                    if (connector.SecondElement is OutputElement outputElement)
                    { 
                        // Устанавливаем сигнал на вход выходного элемента равным сигналу на выходе входного элемента
                        outputElement.SignalIn = inputElement.SignalOut;
                    }
                    if (connector.SecondElement is NotGate notGate)
                    {
                        // Устанавливаем вход NOT-гейта равным сигналу на выходе входного элемента
                        notGate.Input = inputElement.SignalOut;
                        // Добавляем все соединители, соединенные с NOT-гейтом в стек для дальнейшей обработки
                        foreach (Element element in Elements)
                        {
                            if (element is Connector conect)
                            {
                                if (conect.FirstElement == notGate)
                                {
                                    con.Push(conect);
                                }
                            }
                        }
                    }
                    if (connector.SecondElement is TwoInputsGate two)
                    {
                        // Устанавливаем вход 1 или вход 2 двухвходового гейта в зависимости от типа соединения
                        if (connector.ConnectToFirstInput)
                        {
                            two.Input1 = inputElement.SignalOut;
                        }
                        else
                        {
                            two.Input2 = inputElement.SignalOut;
                        }
                        // Добавляем все соединители, соединенные с двухвходовым гейтом в стек для дальнейшей обработки
                        foreach (Element element in Elements)
                        {
                            if (element is Connector conect)
                            {
                                if (conect.FirstElement == two)
                                {
                                    con.Push(conect);
                                }
                            }
                        }
                    }
                } 
                // Если первый элемент соединителя - Decoder-гейт
                if (connector.FirstElement is DecoderGate decoderGate)
                {
                    // Если соединение идет с первого входа Decoder-гейта
                    if (connector.ConnectFromFirstInput)
                    {
                        // Если второй элемент соединителя - выходной элемент
                        if (connector.SecondElement is OutputElement outputElement)
                        {
                            // Устанавливаем сигнал на вход выходного элемента равным значению, возвращаемому Decoder-гейтом
                            outputElement.SignalIn = decoderGate.Output();
                        }
                        // Если второй элемент соединителя - NOT-гейт
                        if (connector.SecondElement is NotGate notGate)
                        {
                            // Устанавливаем вход NOT-гейта равным значению, возвращаемому Decoder-гейтом
                            notGate.Input = decoderGate.Output();
                            // Добавляем все соединители, соединенные с NOT-гейтом в стек для дальнейшей обработки
                            foreach (Element element in Elements)
                            {
                                if (element is Connector conect)
                                {
                                    if (conect.FirstElement == notGate)
                                        con.Push(conect);
                                }
                            }
                        }
                        // Если второй элемент соединителя - двухвходовой гейт
                        if (connector.SecondElement is TwoInputsGate two)
                        {
                            // Устанавливаем вход 1 или вход 2 двухвходового гейта в зависимости от типа соединения
                            if (connector.ConnectToFirstInput)
                                two.Input1 = decoderGate.Output();
                            else
                                two.Input2 = decoderGate.Output();
                            // Добавляем все соединители, соединенные с двухвходовым гейтом в стек для дальнейшей обработки
                            foreach (Element element in Elements)
                            {
                                if (element is Connector conect)
                                {
                                    if (conect.FirstElement == two)
                                        con.Push(conect);
                                }
                            }
                        }
                    }
                    // Если соединение идет со второго входа Decoder-гейта
                    else
                    {
                        // Если второй элемент соединителя - выходной элемент
                        if (connector.SecondElement is OutputElement outputElement)
                        {
                            // Устанавливаем сигнал на вход выходного элемента равным значению, возвращаемому Decoder-гейтом
                            outputElement.SignalIn = decoderGate.Output2();
                        }

                        // Если второй элемент соединителя - NOT-гейт
                        if (connector.SecondElement is NotGate notGate)
                        {
                            // Устанавливаем вход NOT-гейта равным значению, возвращаемому Decoder-гейтом
                            notGate.Input = decoderGate.Output2();
                            // Добавляем все соединители, соединенные с NOT-гейтом в стек для дальнейшей обработки
                            foreach (Element element in Elements)
                            {
                                if (element is Connector conect)
                                {
                                    if (conect.FirstElement == notGate)
                                        con.Push(conect);
                                }
                            }
                        }
                        // Если второй элемент соединителя - двухвходовой гейт
                        if (connector.SecondElement is TwoInputsGate two)
                        {
                            // Устанавливаем вход 1 или вход 2 двухвходового гейта в зависимости от типа соединения
                            if (connector.ConnectToFirstInput)
                                two.Input1 = decoderGate.Output2();
                            else
                                two.Input2 = decoderGate.Output2();

                            // Добавляем все соединители, соединенные с двухвходовым гейтом в стек для дальнейшей обработки
                            foreach (Element element in Elements)
                            {
                                if (element is Connector conect)
                                {
                                    if (conect.FirstElement == two)
                                        con.Push(conect);
                                }
                            }
                        }
                    }
                }
                else if (connector.FirstElement is Gate gate)
                {
                    if (connector.SecondElement is OutputElement outputElement)
                        outputElement.SignalIn = gate.Output();
                    if (connector.SecondElement is NotGate notGate)
                    {
                        notGate.Input = gate.Output();
                        foreach (Element element in Elements)
                        {
                            if (element is Connector conect)
                                if (conect.FirstElement == notGate)
                                    con.Push(conect);
                        }
                    }
                    if (connector.SecondElement is TwoInputsGate two)
                    {
                        if (connector.ConnectToFirstInput)
                            two.Input1 = gate.Output();
                        else
                            two.Input2 = gate.Output();
                        foreach (Element element in Elements)
                        {
                            if (element is Connector conect)
                                if (conect.FirstElement == two)
                                    con.Push(conect);
                        }
                    }
                }
            }
        }

        public ReactiveCommand<Unit, Unit> DeleteFocusedElement { get; }
        public ReactiveCommand<Unit, Unit> AddScheme { get; }
        public ReactiveCommand<Unit, Unit> DeleteScheme { get; }
        public ReactiveCommand<Unit, Unit> CreateProject { get; }
    }
}
