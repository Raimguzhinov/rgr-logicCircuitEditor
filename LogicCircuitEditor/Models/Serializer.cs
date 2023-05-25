using Avalonia;
using LogicCircuitEditor.Models.LogicalElements;
using LogicCircuitEditor.Models.SerializebleElements;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using InputElement = LogicCircuitEditor.Models.LogicalElements.InputElement;

namespace LogicCircuitEditor.Models
{
    public static class Serializer
    {
        // Инициализация сериализатора с использованием Builder
        private static readonly ISerializer serializer = new SerializerBuilder()
            .WithTagMapping("!scheme", typeof(Scheme))
            .WithTagMapping("!connector", typeof(SerializebleConnector))
            .WithTagMapping("!input", typeof(SerializebleInput))
            .WithTagMapping("!output", typeof(SerializebleOutput))
            .WithTagMapping("!twoinputs", typeof(SerializebleTwoInputElements))
            .WithTagMapping("!not", typeof(SerializebleNot))
            .Build();

        // Инициализация десериализатора с использованием Builder
        private static readonly IDeserializer deserializer = new DeserializerBuilder()
            .WithTagMapping("!scheme", typeof(Scheme))
            .WithTagMapping("!connector", typeof(SerializebleConnector))
            .WithTagMapping("!input", typeof(SerializebleInput))
            .WithTagMapping("!output", typeof(SerializebleOutput))
            .WithTagMapping("!twoinputs", typeof(SerializebleTwoInputElements))
            .WithTagMapping("!not", typeof(SerializebleNot))
            .Build();

        // Метод для сохранения проекта в файл YAML
        public static void Save(string path, Project data)
        {
            // Преобразование проекта в сериализуемую форму
            Project project = ToSerializeble(data);
            // Сериализация проекта в YAML
            var yaml = serializer.Serialize(project);
            // Запись YAML в файл
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine(yaml);
            }
        }

        // Метод для загрузки проекта из файла YAML
        public static Project Load(string path)
        {
            string input;
            // Чтение YAML из файла
            using (StreamReader reader = new StreamReader(path))
            {
                input = reader.ReadToEnd();
            }
            // Десериализация YAML в объект проекта
            var ser = deserializer.Deserialize<Project>(input);
            // Преобразование сериализуемого объекта обратно в исходный формат
            return FromSerializeble(ser);
        }

        // Метод для преобразования проекта в сериализуемую форму
        public static Project ToSerializeble(Project data)
        {
            Project newProject = new Project { Name = data.Name };
            var schemes = data.Schemes;
            ObservableCollection<Scheme> newSchemes =
                new ObservableCollection<Scheme>();
            foreach (Scheme scheme in schemes)
            {
                ObservableCollection<Element> newElements =
                    new ObservableCollection<Element>();
                var elements = scheme.Elements;
                foreach (Element element in elements)
                {
                    switch (element)
                    {
                        case Connector connector:
                            // Преобразование Connector в SerializebleConnector
                            newElements.Add(new SerializebleConnector
                            {
                                ID = connector.ID,
                                FocusOnElement = connector.FocusOnElement,
                                ConnectFromFirstInput = connector.ConnectFromFirstInput,
                                ConnectToFirstInput = connector.ConnectToFirstInput,
                                ReverseConnection = connector.ReverseConnection,
                                StartPoint = connector.StartPoint.ToString(),
                                EndPoint = connector.EndPoint.ToString(),
                                FirstElement = connector.FirstElement.ID,
                                SecondElement = connector.SecondElement.ID
                            });
                            break;

                        case InputElement input:
                            // Преобразование InputElement в SerializebleInput
                            newElements.Add(new SerializebleInput
                            {
                                ID = input.ID,
                                FocusOnElement = input.FocusOnElement,
                                SignalOut = input.SignalOut,
                                StartPoint = input.StartPoint.ToString()
                            });
                            break;

                        case OutputElement output:
                            // Преобразование OutputElement в SerializebleOutput
                            newElements.Add(new SerializebleOutput
                            {
                                ID = output.ID,
                                FocusOnElement = output.FocusOnElement,
                                IsConnected = output.IsConnected,
                                SignalIn = output.SignalIn,
                                StartPoint = output.StartPoint.ToString()
                            });
                            break;

                        case TwoInputsGate two:
                            var elem = new SerializebleTwoInputElements
                            {
                                ID = two.ID,
                                FocusOnElement = two.FocusOnElement,
                                Input1 = two.Input1,
                                Input2 = two.Input2,
                                IsConnectedInput1 = two.IsConnectedInput1,
                                IsConnectedInput2 = two.IsConnectedInput2,
                                StartPoint = two.StartPoint.ToString()
                            };

                            switch (two)
                            {
                                case AndGate:
                                    elem.Type = "AndGate";
                                    break;
                                case OrGate:
                                    elem.Type = "OrGate";
                                    break;
                                case XorGate:
                                    elem.Type = "XorGate";
                                    break;
                                case DecoderGate:
                                    elem.Type = "DecoderGate";
                                    break;
                            }

                            newElements.Add(elem);
                            break;

                        case NotGate not:
                            // Преобразование NotGate в SerializebleNot
                            newElements.Add(new SerializebleNot
                            {
                                ID = not.ID,
                                FocusOnElement = not.FocusOnElement,
                                Input = not.Input,
                                IsConnected = not.IsConnected,
                                StartPoint = not.StartPoint.ToString()
                            });
                            break;
                    }
                }
                newSchemes.Add(
                    new Scheme { Name = scheme.Name, Elements = newElements });
            }
            newProject.Schemes = newSchemes;
            return newProject;
        }

        // Метод для преобразования сериализуемой формы в исходный формат проекта
        public static Project FromSerializeble(Project data)
        {
            Project newProject = new Project { Name = data.Name };
            var schemes = data.Schemes;
            ObservableCollection<Scheme> newSchemes =
                new ObservableCollection<Scheme>();
            foreach (Scheme scheme in schemes)
            {
                ObservableCollection<Element> newElements =
                    new ObservableCollection<Element>();
                var elements = scheme.Elements;
                foreach (Element element in elements)
                {
                    switch (element)
                    {
                        case SerializebleConnector connector:
                            // Преобразование SerializebleConnector в Connector
                            Connector newConnector = new Connector
                            {
                                ID = connector.ID,
                                FocusOnElement = connector.FocusOnElement,
                                ConnectFromFirstInput = connector.ConnectFromFirstInput,
                                ConnectToFirstInput = connector.ConnectToFirstInput,
                                ReverseConnection = connector.ReverseConnection,
                                StartPoint = Point.Parse(connector.StartPoint),
                                EndPoint = Point.Parse(connector.EndPoint)
                            };

                            foreach (Element el in newElements.OfType<LogicalElement>())
                            {
                                if (el.ID == connector.FirstElement)
                                {
                                    newConnector.FirstElement = (LogicalElement)el;
                                    break;
                                }
                            }

                            foreach (Element el in newElements.OfType<LogicalElement>())
                            {
                                if (el.ID == connector.SecondElement)
                                {
                                    newConnector.SecondElement = (LogicalElement)el;
                                    break;
                                }
                            }

                            newElements.Add(newConnector);
                            break;

                        case SerializebleInput input:
                            // Преобразование SerializebleInput в InputElement
                            newElements.Add(
                                new InputElement
                                {
                                    ID = input.ID,
                                    FocusOnElement = input.FocusOnElement,
                                    SignalOut = input.SignalOut,
                                    StartPoint = Point.Parse(input.StartPoint)
                                });
                            break;

                        case SerializebleOutput output:
                            // Преобразование SerializebleOutput в OutputElement
                            newElements.Add(new OutputElement
                            {
                                ID = output.ID,
                                FocusOnElement = output.FocusOnElement,
                                IsConnected = output.IsConnected,
                                SignalIn = output.SignalIn,
                                StartPoint = Point.Parse(output.StartPoint)
                            });
                            break;

                        case SerializebleNot not:
                            // Преобразование SerializebleNot в NotGate
                            newElements.Add(
                                new NotGate
                                {
                                    ID = not.ID,
                                    FocusOnElement = not.FocusOnElement,
                                    Input = not.Input,
                                    StartPoint = Point.Parse(not.StartPoint),
                                    IsConnected = not.IsConnected
                                });
                            break;

                        case SerializebleTwoInputElements two:
                            TwoInputsGate twoInputsGate;
                            switch (two.Type)
                            {
                                case "AndGate":
                                    twoInputsGate = new AndGate();
                                    break;
                                case "OrGate":
                                    twoInputsGate = new OrGate();
                                    break;
                                case "XorGate":
                                    twoInputsGate = new XorGate();
                                    break;
                                case "DecoderGate":
                                    twoInputsGate = new DecoderGate();
                                    break;
                                default:
                                    continue;
                            }

                            twoInputsGate.ID = two.ID;
                            twoInputsGate.FocusOnElement = two.FocusOnElement;
                            twoInputsGate.Input1 = two.Input1;
                            twoInputsGate.Input2 = two.Input2;
                            twoInputsGate.IsConnectedInput1 = two.IsConnectedInput1;
                            twoInputsGate.IsConnectedInput2 = two.IsConnectedInput2;
                            twoInputsGate.StartPoint = Point.Parse(two.StartPoint);

                            newElements.Add(twoInputsGate);
                            break;
                    }
                }
                newSchemes.Add(
                    new Scheme { Name = scheme.Name, Elements = newElements });
            }
            newProject.Schemes = newSchemes;
            return newProject;
        }
    }
}
