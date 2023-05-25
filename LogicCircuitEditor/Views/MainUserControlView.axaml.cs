using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using LogicCircuitEditor.Models.LogicalElements;
using LogicCircuitEditor.Models;
using LogicCircuitEditor.ViewModels;
using System.Collections.Generic;
using System.Linq;
using InputElement = LogicCircuitEditor.Models.LogicalElements.InputElement;

namespace LogicCircuitEditor.Views
{
    public partial class MainUserControlView : UserControl
    {
        private Point _pointPointerPressed;
        private Point _pointerPositionIntoShape;
        public MainUserControlView() { InitializeComponent(); }

        // Обработчик события нажатия кнопки "Открыть файл YAML" в меню
        private async void OpenFileDialogMenuYamlClick(object sender, RoutedEventArgs routedEventArgs)
        {
            // Создание диалогового окна для выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            List<string> formats = new List<string> { "yaml" };
            openFileDialog.Filters.Add(new FileDialogFilter { Extensions = formats, Name = "Yaml files" });
            openFileDialog.AllowMultiple = false;

            // Отображение диалогового окна и получение результата (выбранный файл)
            string[]? result = await openFileDialog.ShowAsync(this.GetLogicalParent() as MainWindow);

            // Загрузка проекта из выбранного файла YAML и обновление модели представления (DataContext)
            if (DataContext is MainUserControlViewModel dataContext)
            {
                if (result != null)
                {
                    dataContext.Index = 0;
                    dataContext.ChangeElements = true;
                    dataContext.Project = Serializer.Load(result[0]);
                    dataContext.ChangeElements = false;
                }
            }
        }

        // Обработчик события нажатия кнопки "Сохранить в файл YAML" в меню
        private async void SaveFileDialogMenuYamlClick(object sender, RoutedEventArgs routedEventArgs)
        {
            // Создание диалогового окна для сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            List<string> formats = new List<string> { "yaml" };
            saveFileDialog.Filters.Add(new FileDialogFilter { Extensions = formats, Name = "Yaml files" });

            // Отображение диалогового окна и получение результата (путь сохранения файла)
            string? result = await saveFileDialog.ShowAsync(this.GetLogicalParent() as MainWindow);

            // Сохранение текущего проекта в файл YAML и обновление списка проектов в главном окне
            if (DataContext is MainUserControlViewModel dataContext)
            {
                if (result != null)
                {
                    dataContext.Project.Schemes[dataContext.Index].Elements = dataContext.Elements;
                    Serializer.Save(result, dataContext.Project);
                    if (this.GetLogicalParent() is MainWindow mw)
                    {
                        if (mw.DataContext is MainWindowViewModel mainWindow)
                        {
                            mainWindow.AddNewProjectPath(dataContext.Project.Name, result);
                        }
                    }
                }
            }
        }

        private void PointerPressedOnMainCanvas(object sender, PointerPressedEventArgs pointerPressedEventArgs)
        {
            // Проверяем источник события на принадлежность к классу Avalonia.Controls.Control
            if (pointerPressedEventArgs.Source is Avalonia.Controls.Control control)
            {
                // Получаем координаты точки нажатия указателя относительно главного холста
                _pointPointerPressed = pointerPressedEventArgs.GetPosition(
                    this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                        canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                                  canvas.Name.Equals("mainCanvas")));

                // Установка фокуса на соединитель, если элемент - соединитель
                if (control.DataContext is Connector connect)
                    connect.FocusOnElement = SetFocus();

                // Проверка типа элемента и добавление соответствующего элемента в модель представления
                if (control.DataContext is MainUserControlViewModel mainWindow)
                {
                    if (mainWindow.IsInput)
                        mainWindow.Elements.Add(new InputElement { StartPoint = _pointPointerPressed });
                    if (mainWindow.IsOutput)
                        mainWindow.Elements.Add(new OutputElement { StartPoint = _pointPointerPressed });
                    if (mainWindow.IsNotGate)
                        mainWindow.Elements.Add(new NotGate { StartPoint = _pointPointerPressed });
                    if (mainWindow.IsAndGate)
                        mainWindow.Elements.Add(new AndGate { StartPoint = _pointPointerPressed });
                    if (mainWindow.IsOrGate)
                        mainWindow.Elements.Add(new OrGate { StartPoint = _pointPointerPressed });
                    if (mainWindow.IsXorGate)
                        mainWindow.Elements.Add(new XorGate { StartPoint = _pointPointerPressed });
                    if (mainWindow.IsDecoderGate)
                        mainWindow.Elements.Add(new DecoderGate { StartPoint = _pointPointerPressed });
                }

                // Обработка нажатия на элемент в модели представления InputElement
                if (this.DataContext is MainUserControlViewModel viewModel)
                {
                    if (control.DataContext is InputElement input)
                    {
                        input.FocusOnElement = SetFocus();

                        // Проверяем, является ли элемент эллипсом (сигналом)
                        if (control is Ellipse el)
                        {
                            if (el.Name == "Signal")
                            {
                                // Получаем координаты позиции указателя относительно родительского элемента
                                _pointerPositionIntoShape = pointerPressedEventArgs.GetPosition(control.Parent);
                                this.PointerMoved += PointerMoveDragShape;
                                this.PointerReleased += PointerPressedReleasedDragShape;
                            }
                            else
                            {
                                // Создаем соединитель и добавляем его в модель представления
                                viewModel.Elements.Add(new Connector
                                {
                                    StartPoint = _pointPointerPressed,
                                    EndPoint = _pointPointerPressed,
                                    FirstElement = input
                                });
                                this.PointerMoved += PointerMoveDrawLine;
                                this.PointerReleased += PointerPressedReleasedDrawLineToIn;
                            }
                        }
                        else
                        {
                            // Получаем координаты позиции указателя относительно родительского элемента
                            _pointerPositionIntoShape = pointerPressedEventArgs.GetPosition(control.Parent);
                            this.PointerMoved += PointerMoveDragShape;
                            this.PointerReleased += PointerPressedReleasedDragShape;
                        }
                    }

                    // Обработка нажатия на элемент в модели представления OutputElement
                    if (control.DataContext is OutputElement output)
                    {
                        output.FocusOnElement = SetFocus();

                        // Проверяем, является ли элемент эллипсом (сигналом)
                        if (control is Ellipse el)
                        {
                            if (el.Name == "Signal")
                            {
                                // Получаем координаты позиции указателя относительно родительского элемента
                                _pointerPositionIntoShape = pointerPressedEventArgs.GetPosition(control.Parent);
                                this.PointerMoved += PointerMoveDragShape;
                                this.PointerReleased += PointerPressedReleasedDragShape;
                            }
                            else
                            {
                                // Создаем соединитель и добавляем его в модель представления
                                viewModel.Elements.Add(new Connector
                                {
                                    StartPoint = _pointPointerPressed,
                                    EndPoint = _pointPointerPressed,
                                    SecondElement = output,
                                    ConnectToFirstInput = true,
                                    ReverseConnection = true
                                });
                                output.IsConnected = true;
                                this.PointerMoved += PointerMoveDrawLine;
                                this.PointerReleased += PointerPressedReleasedDrawLineToOut;
                            }
                        }
                        else
                        {
                            // Получаем координаты позиции указателя относительно родительского элемента
                            _pointerPositionIntoShape = pointerPressedEventArgs.GetPosition(control.Parent);
                            this.PointerMoved += PointerMoveDragShape;
                            this.PointerReleased += PointerPressedReleasedDragShape;
                        }
                    }

                    // Обработка нажатия на элемент в модели представления Gate
                    if (control.DataContext is Gate gate)
                    {
                        gate.FocusOnElement = SetFocus();

                        // Проверяем, является ли элемент эллипсом (входом или выходом)
                        if (control is Ellipse el)
                        {
                            if (el.Name == "Input" || el.Name == "Input1" ||
                                el.Name == "Input2")
                            {
                                // Обработка нажатия на входной эллипс
                                if (gate is NotGate not)
                                {
                                    if (not.IsConnected == false)
                                    {
                                        // Создаем соединитель и добавляем его в модель представления
                                        viewModel.Elements.Add(new Connector
                                        {
                                            StartPoint = _pointPointerPressed,
                                            EndPoint = _pointPointerPressed,
                                            SecondElement = not,
                                            ConnectToFirstInput = true,
                                            ReverseConnection = true
                                        });
                                        not.IsConnected = true;
                                        this.PointerMoved += PointerMoveDrawLine;
                                        this.PointerReleased += PointerPressedReleasedDrawLineToOut;
                                    }
                                }

                                if (gate is TwoInputsGate two)
                                {
                                    // Обработка нажатия на входной эллипс
                                    if (el.Name == "Input1")
                                    {
                                        if (two.IsConnectedInput1 == false)
                                        {
                                            // Создаем соединитель и добавляем его в модель представления
                                            viewModel.Elements.Add(new Connector
                                            {
                                                StartPoint = _pointPointerPressed,
                                                EndPoint = _pointPointerPressed,
                                                SecondElement = two,
                                                ConnectToFirstInput = true,
                                                ReverseConnection = true
                                            });
                                            two.IsConnectedInput1 = true;
                                            this.PointerMoved += PointerMoveDrawLine;
                                            this.PointerReleased += PointerPressedReleasedDrawLineToOut;
                                        }
                                    }
                                    else
                                    {
                                        if (two.IsConnectedInput2 == false)
                                        {
                                            // Создаем соединитель и добавляем его в модель представления
                                            viewModel.Elements.Add(new Connector
                                            {
                                                StartPoint = _pointPointerPressed,
                                                EndPoint = _pointPointerPressed,
                                                SecondElement = two,
                                                ConnectToFirstInput = false,
                                                ReverseConnection = true
                                            });
                                            two.IsConnectedInput2 = true;
                                            this.PointerMoved += PointerMoveDrawLine;
                                            this.PointerReleased += PointerPressedReleasedDrawLineToOut;
                                        }
                                    }
                                }
                            }

                            if (el.Name == "Output" || el.Name == "Output1" ||
                                el.Name == "Output2")
                            {
                                // Обработка нажатия на выходной эллипс
                                if (el.Name == "Output")
                                {
                                    // Создаем соединитель и добавляем его в модель представления
                                    viewModel.Elements.Add(new Connector
                                    {
                                        StartPoint = _pointPointerPressed,
                                        EndPoint = _pointPointerPressed,
                                        FirstElement = gate
                                    });
                                    this.PointerMoved += PointerMoveDrawLine;
                                    this.PointerReleased += PointerPressedReleasedDrawLineToIn;
                                }
                                else
                                {
                                    if (el.Name == "Output1")
                                    {
                                        // Создаем соединитель и добавляем его в модель представления
                                        viewModel.Elements.Add(new Connector
                                        {
                                            StartPoint = _pointPointerPressed,
                                            EndPoint = _pointPointerPressed,
                                            FirstElement = gate,
                                            ConnectFromFirstInput = true
                                        });
                                    }
                                    else
                                    {
                                        // Создаем соединитель и добавляем его в модель представления
                                        viewModel.Elements.Add(new Connector
                                        {
                                            StartPoint = _pointPointerPressed,
                                            EndPoint = _pointPointerPressed,
                                            FirstElement = gate,
                                            ConnectFromFirstInput = false
                                        });
                                    }

                                    this.PointerMoved += PointerMoveDrawLine;
                                    this.PointerReleased += PointerPressedReleasedDrawLineToIn;
                                }
                            }
                        }
                        else
                        {
                            // Получаем координаты позиции указателя относительно родительского элемента
                            _pointerPositionIntoShape = pointerPressedEventArgs.GetPosition(control.Parent);
                            this.PointerMoved += PointerMoveDragShape;
                            this.PointerReleased += PointerPressedReleasedDragShape;
                        }
                    }
                }
            }
        }

        private void PointerMoveDragShape(object? sender, PointerEventArgs pointerEventArgs)
        {
            // Проверяем, является ли источник события элементом управления типа Avalonia.Controls.Control
            if (pointerEventArgs.Source is Avalonia.Controls.Control control)
            {
                // Получаем текущие координаты указателя относительно главного холста
                Point currentPointerPosition = pointerEventArgs.GetPosition(
                    this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                        canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                                  canvas.Name.Equals("mainCanvas")));

                // Проверяем, является ли DataContext элемента управления типом LogicalElement
                if (control.DataContext is LogicalElement log)
                {
                    // Устанавливаем новую позицию элемента на холсте, основываясь на текущих координатах указателя
                    log.StartPoint =
                        new Point(currentPointerPosition.X - _pointerPositionIntoShape.X,
                            currentPointerPosition.Y - _pointerPositionIntoShape.Y);
                }
            }
        }

        private void PointerPressedReleasedDragShape(object? sender, PointerReleasedEventArgs pointerReleasedEventArgs)
        {
            // Удаляем обработчики событий PointerMoved и PointerReleased, чтобы прекратить перемещение элемента
            this.PointerMoved -= PointerMoveDragShape;
            this.PointerReleased -= PointerPressedReleasedDragShape;
        }

        private void PointerMoveDrawLine(object? sender, PointerEventArgs pointerEventArgs)
        {
            // Проверяем, является ли DataContext главного элемента управления MainUserControlViewModel
            if (this.DataContext is MainUserControlViewModel viewModel)
            {
                // Проверяем, является ли последний элемент модели представления Connector
                if (viewModel.Elements[viewModel.Elements.Count - 1] is Connector connector)
                {
                    // Получаем текущие координаты указателя относительно главного холста
                    Point currentPointerPosition = pointerEventArgs.GetPosition(
                        this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                            canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                                      canvas.Name.Equals("mainCanvas")));

                    // Определяем направление линии связи от начальной точки к текущей позиции указателя
                    var x = currentPointerPosition.X > connector.StartPoint.X ? -1 : 1;
                    var y = currentPointerPosition.Y > connector.StartPoint.Y ? -1 : 1;

                    // Устанавливаем конечную точку линии связи в текущую позицию указателя с учетом направления
                    connector.EndPoint = new Point(currentPointerPosition.X + x,
                        currentPointerPosition.Y + y);
                }
            }
        }

        private void PointerPressedReleasedDrawLineToIn(object? sender,
            PointerReleasedEventArgs pointerReleasedEventArgs)
        {
            // Отписываемся от обработчиков событий PointerMoved и PointerReleased
            this.PointerMoved -= PointerMoveDrawLine;
            this.PointerReleased -= PointerPressedReleasedDrawLineToIn;

            // Получаем холст
            var canvas = this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                          canvas.Name.Equals("mainCanvas"));

            // Получаем координаты указателя
            var coords = pointerReleasedEventArgs.GetPosition(canvas);

            // Выполняем проверку наличия элемента под указателем и получаем ViewModel
            var element = canvas.InputHitTest(coords);
            MainUserControlViewModel viewModel = this.DataContext as MainUserControlViewModel;

            if (element is Ellipse ellipse)
            {
                // Проверяем имя эллипса
                if (ellipse.Name == "Input" || ellipse.Name == "Input1" || ellipse.Name == "Input2")
                {
                    if (viewModel.Elements[viewModel.Elements.Count - 1] is Connector connector)
                    {
                        // Проверяем тип данных контекста эллипса и осуществляем соединение с коннектором
                        if (ellipse.DataContext is NotGate not)
                        {
                            if (not.IsConnected == false)
                            {
                                connector.SecondElement = not;
                                not.IsConnected = true;
                                connector.ConnectToFirstInput = true;
                                if (connector.FirstElement is InputElement)
                                    viewModel.ConnectWithInputElement.Push(connector);
                                viewModel.SignalBypass();
                                return;
                            }
                        }

                        if (ellipse.DataContext is OutputElement output)
                        {
                            if (output.IsConnected == false)
                            {
                                connector.SecondElement = output;
                                output.IsConnected = true;
                                connector.ConnectToFirstInput = true;
                                if (connector.FirstElement is InputElement)
                                    viewModel.ConnectWithInputElement.Push(connector);
                                viewModel.SignalBypass();
                                return;
                            }
                        }

                        if (ellipse.DataContext is TwoInputsGate two)
                        {
                            // Проверяем имя эллипса для определения входа
                            if (ellipse.Name == "Input1")
                            {
                                if (two.IsConnectedInput1 == false)
                                {
                                    connector.SecondElement = two;
                                    two.IsConnectedInput1 = true;
                                    connector.ConnectToFirstInput = true;
                                    if (connector.FirstElement is InputElement)
                                        viewModel.ConnectWithInputElement.Push(connector);
                                    viewModel.SignalBypass();
                                    return;
                                }
                            }

                            if (ellipse.Name == "Input2")
                            {
                                if (two.IsConnectedInput2 == false)
                                {
                                    connector.SecondElement = two;
                                    two.IsConnectedInput2 = true;
                                    connector.ConnectToFirstInput = false;
                                    if (connector.FirstElement is InputElement)
                                        viewModel.ConnectWithInputElement.Push(connector);
                                    viewModel.SignalBypass();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            // Удаляем последний элемент из списка элементов
            viewModel.Elements.RemoveAt(viewModel.Elements.Count - 1);
        }

        private void PointerPressedReleasedDrawLineToOut(object? sender,
            PointerReleasedEventArgs pointerReleasedEventArgs)
        {
            // Отписываемся от обработчиков событий PointerMoved и PointerReleased
            this.PointerMoved -= PointerMoveDrawLine;
            this.PointerReleased -= PointerPressedReleasedDrawLineToOut;

            // Получаем холст
            var canvas = this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                          canvas.Name.Equals("mainCanvas"));

            // Получаем координаты указателя
            var coords = pointerReleasedEventArgs.GetPosition(canvas);

            // Выполняем проверку наличия элемента под указателем и получаем ViewModel
            var element = canvas.InputHitTest(coords);
            MainUserControlViewModel viewModel = this.DataContext as MainUserControlViewModel;

            if (element is Ellipse ellipse)
            {
                // Проверяем имя эллипса
                if (ellipse.Name == "Output" || ellipse.Name == "Output1" || ellipse.Name == "Output2")
                {
                    if (viewModel.Elements[viewModel.Elements.Count - 1] is Connector connector)
                    {
                        // Проверяем тип данных контекста эллипса и осуществляем соединение с коннектором
                        if (ellipse.DataContext is InputElement input)
                        {
                            connector.FirstElement = input;
                            viewModel.ConnectWithInputElement.Push(connector);
                        }

                        if (ellipse.DataContext is DecoderGate dec)
                        {
                            connector.FirstElement = dec;
                            if (ellipse.Name == "Output2")
                                connector.ConnectFromFirstInput = false;
                        }
                        else if (ellipse.DataContext is Gate gat)
                            connector.FirstElement = gat;

                        viewModel.SignalBypass();
                        return;
                    }
                }
            }

            // Проверяем и удаляем последний элемент из списка элементов или сбрасываем связи коннектора
            if (viewModel.Elements[viewModel.Elements.Count - 1] is Connector con)
            {
                if (con.ConnectToFirstInput)
                {
                    if (con.SecondElement is NotGate not)
                        not.IsConnected = false;
                    if (con.SecondElement is OutputElement output)
                        output.IsConnected = false;
                    if (con.SecondElement is TwoInputsGate two)
                        two.IsConnectedInput1 = false;
                }
                else
                {
                    if (con.SecondElement is TwoInputsGate two)
                        two.IsConnectedInput2 = false;
                }
            }

            viewModel.Elements.RemoveAt(viewModel.Elements.Count - 1);
        }

        private void ChangeSignal(object sender, RoutedEventArgs routedEventArgs)
        {
            // Проверяем и изменяем сигнал элемента в ответ на событие
            if (routedEventArgs.Source is Avalonia.Controls.Control control)
            {
                MainUserControlViewModel viewModel = DataContext as MainUserControlViewModel;
                if (control.DataContext is InputElement input)
                {
                    input.SignalOut = !input.SignalOut;
                    viewModel.SignalBypass();
                }
            }
        }

        private bool SetFocus()
        {
            // Снимаем фокус со всех элементов в ViewModel
            MainUserControlViewModel viewModel = DataContext as MainUserControlViewModel;
            foreach (Element el in viewModel.Elements)
                el.FocusOnElement = false;
            return true;
        }

        private void DeleteElement(object sender, RoutedEventArgs routedEventArgs)
        {
            // Удаляем выделенный элемент из списка элементов в ViewModel
            if (this.DataContext is MainUserControlViewModel mainWindow)
            {
                for (int i = 0; i < mainWindow.Elements.Count; ++i)
                {
                    if (mainWindow.Elements[i].FocusOnElement == true)
                    {
                        mainWindow.Elements.Remove(mainWindow.Elements[i]);
                        return;
                    }
                }
            }
        }

        private void CloseWindowButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            // Закрываем родительское окно
            if (this.GetLogicalParent() is MainWindow mw)
                mw.Close();
        }
    }
}
