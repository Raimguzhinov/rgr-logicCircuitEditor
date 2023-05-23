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
        private Point pointPointerPressed;
        private Point pointerPositionIntoShape;
        public MainUserControlView() { InitializeComponent(); }

        private async void
        OpenFileDialogMenuYamlClick(object sender, RoutedEventArgs routedEventArgs)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            List<string> formates = new List<string> { "yaml" };
            openFileDialog.Filters.Add(
                new FileDialogFilter { Extensions = formates, Name = "Yaml files" });
            openFileDialog.AllowMultiple = false;
            string[]? result =
                await openFileDialog.ShowAsync(this.GetLogicalParent() as MainWindow);
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
        private async void
        SaveFileDialogMenuYamlClick(object sender, RoutedEventArgs routedEventArgs)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            List<string> formates = new List<string> { "yaml" };
            saveFileDialog.Filters.Add(
                new FileDialogFilter { Extensions = formates, Name = "Yaml files" });
            string? result =
                await saveFileDialog.ShowAsync(this.GetLogicalParent() as MainWindow);
            if (DataContext is MainUserControlViewModel dataContext)
            {
                if (result != null)
                {
                    dataContext.Project.Schemes[dataContext.Index].Elements =
                        dataContext.Elements;
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

        private void
        PointerPressedOnMainCanvas(object sender,
                                   PointerPressedEventArgs pointerPressedEventArgs)
        {
            if (pointerPressedEventArgs.Source is Avalonia.Controls.Control control)
            {

                pointPointerPressed = pointerPressedEventArgs.GetPosition(
                    this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                        canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                                  canvas.Name.Equals("mainCanvas")));
                if (control.DataContext is Connector connect)
                    connect.FocusOnElement = SetFocus();
                if (control.DataContext is MainUserControlViewModel mainWindow)
                {
                    if (mainWindow.IsInput)
                        mainWindow.Elements.Add(
                            new InputElement { StartPoint = pointPointerPressed });
                    if (mainWindow.IsOutput)
                        mainWindow.Elements.Add(
                            new OutputElement { StartPoint = pointPointerPressed });
                    if (mainWindow.IsNotGate)
                        mainWindow.Elements.Add(
                            new NotGate { StartPoint = pointPointerPressed });
                    if (mainWindow.IsAndGate)
                        mainWindow.Elements.Add(
                            new AndGate { StartPoint = pointPointerPressed });
                    if (mainWindow.IsOrGate)
                        mainWindow.Elements.Add(
                            new OrGate { StartPoint = pointPointerPressed });
                    if (mainWindow.IsXorGate)
                        mainWindow.Elements.Add(
                            new XorGate { StartPoint = pointPointerPressed });
                    if (mainWindow.IsDecoderGate)
                        mainWindow.Elements.Add(
                            new DecoderGate { StartPoint = pointPointerPressed });
                }
                if (this.DataContext is MainUserControlViewModel viewModel)
                {
                    if (control.DataContext is InputElement input)
                    {
                        input.FocusOnElement = SetFocus();
                        if (control is Ellipse el)
                        {
                            if (el.Name == "Signal")
                            {
                                pointerPositionIntoShape =
                                    pointerPressedEventArgs.GetPosition(control.Parent);
                                this.PointerMoved += PointerMoveDragShape;
                                this.PointerReleased += PointerPressedReleasedDragShape;
                            }
                            else
                            {
                                viewModel.Elements.Add(
                                    new Connector
                                    {
                                        StartPoint = pointPointerPressed,
                                        EndPoint = pointPointerPressed,
                                        FirstElement = input
                                    });
                                this.PointerMoved += PointerMoveDrawLine;
                                this.PointerReleased += PointerPressedReleasedDrawLineToIn;
                            }
                        }
                        else
                        {
                            pointerPositionIntoShape =
                                pointerPressedEventArgs.GetPosition(control.Parent);
                            this.PointerMoved += PointerMoveDragShape;
                            this.PointerReleased += PointerPressedReleasedDragShape;
                        }
                    }
                    if (control.DataContext is OutputElement output)
                    {
                        output.FocusOnElement = SetFocus();
                        if (control is Ellipse el)
                        {
                            if (el.Name == "Signal")
                            {
                                pointerPositionIntoShape =
                                    pointerPressedEventArgs.GetPosition(control.Parent);
                                this.PointerMoved += PointerMoveDragShape;
                                this.PointerReleased += PointerPressedReleasedDragShape;
                            }
                            else
                            {
                                viewModel.Elements.Add(new Connector
                                {
                                    StartPoint = pointPointerPressed,
                                    EndPoint = pointPointerPressed,
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
                            pointerPositionIntoShape =
                                pointerPressedEventArgs.GetPosition(control.Parent);
                            this.PointerMoved += PointerMoveDragShape;
                            this.PointerReleased += PointerPressedReleasedDragShape;
                        }
                    }
                    if (control.DataContext is Gate gate)
                    {
                        gate.FocusOnElement = SetFocus();
                        if (control is Ellipse el)
                        {
                            if (el.Name == "Input" || el.Name == "Input1" ||
                                el.Name == "Input2")
                            {
                                if (gate is NotGate not)
                                {
                                    if (not.IsConnected == false)
                                    {
                                        viewModel.Elements.Add(new Connector
                                        {
                                            StartPoint = pointPointerPressed,
                                            EndPoint = pointPointerPressed,
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
                                    if (el.Name == "Input1")
                                    {
                                        if (two.IsConnectedInput1 == false)
                                        {
                                            viewModel.Elements.Add(new Connector
                                            {
                                                StartPoint = pointPointerPressed,
                                                EndPoint = pointPointerPressed,
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
                                            viewModel.Elements.Add(new Connector
                                            {
                                                StartPoint = pointPointerPressed,
                                                EndPoint = pointPointerPressed,
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
                                if (el.Name == "Output")
                                {
                                    viewModel.Elements.Add(
                                        new Connector
                                        {
                                            StartPoint = pointPointerPressed,
                                            EndPoint = pointPointerPressed,
                                            FirstElement = gate
                                        });
                                    this.PointerMoved += PointerMoveDrawLine;
                                    this.PointerReleased += PointerPressedReleasedDrawLineToIn;
                                }
                                else
                                {
                                    if (el.Name == "Output1")
                                    {
                                        viewModel.Elements.Add(
                                            new Connector
                                            {
                                                StartPoint = pointPointerPressed,
                                                EndPoint = pointPointerPressed,
                                                FirstElement = gate,
                                                ConnectFromFirstInput = true
                                            });
                                    }
                                    else
                                    {
                                        viewModel.Elements.Add(
                                            new Connector
                                            {
                                                StartPoint = pointPointerPressed,
                                                EndPoint = pointPointerPressed,
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
                            pointerPositionIntoShape =
                                pointerPressedEventArgs.GetPosition(control.Parent);
                            this.PointerMoved += PointerMoveDragShape;
                            this.PointerReleased += PointerPressedReleasedDragShape;
                        }
                    }
                }
            }
        }

        private void PointerMoveDragShape(object? sender,
                                          PointerEventArgs pointerEventArgs)
        {
            if (pointerEventArgs.Source is Avalonia.Controls.Control control)
            {
                Point currentPointerPosition = pointerEventArgs.GetPosition(
                    this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                        canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                                  canvas.Name.Equals("mainCanvas")));
                if (control.DataContext is LogicalElement log)
                {
                    log.StartPoint =
                        new Point(currentPointerPosition.X - pointerPositionIntoShape.X,
                                  currentPointerPosition.Y - pointerPositionIntoShape.Y);
                }
            }
        }

        private void PointerPressedReleasedDragShape(
            object? sender, PointerReleasedEventArgs pointerReleasedEventArgs)
        {
            this.PointerMoved -= PointerMoveDragShape;
            this.PointerReleased -= PointerPressedReleasedDragShape;
        }

        private void PointerMoveDrawLine(object? sender,
                                         PointerEventArgs pointerEventArgs)
        {
            if (this.DataContext is MainUserControlViewModel viewModel)
            {
                if (viewModel.Elements[viewModel.Elements.Count - 1] is Connector
                        connector)
                {
                    Point currentPointerPosition = pointerEventArgs.GetPosition(
                        this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                            canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                                      canvas.Name.Equals("mainCanvas")));
                    var x = currentPointerPosition.X > connector.StartPoint.X ? -1 : 1;
                    var y = currentPointerPosition.Y > connector.StartPoint.Y ? -1 : 1;
                    connector.EndPoint = new Point(currentPointerPosition.X + x,
                                                   currentPointerPosition.Y + y);
                }
            }
        }
        private void PointerPressedReleasedDrawLineToIn(
            object? sender, PointerReleasedEventArgs pointerReleasedEventArgs)
        {
            this.PointerMoved -= PointerMoveDrawLine;
            this.PointerReleased -= PointerPressedReleasedDrawLineToIn;

            var canvas = this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                          canvas.Name.Equals("mainCanvas"));

            var coords = pointerReleasedEventArgs.GetPosition(canvas);

            var element = canvas.InputHitTest(coords);
            MainUserControlViewModel viewModel =
                this.DataContext as MainUserControlViewModel;

            if (element is Ellipse ellipse)
            {
                if (ellipse.Name == "Input" || ellipse.Name == "Input1" ||
                    ellipse.Name == "Input2")
                {
                    if (viewModel.Elements[viewModel.Elements.Count - 1] is Connector
                            connector)
                    {
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
            viewModel.Elements.RemoveAt(viewModel.Elements.Count - 1);
        }

        private void PointerPressedReleasedDrawLineToOut(
            object? sender, PointerReleasedEventArgs pointerReleasedEventArgs)
        {
            this.PointerMoved -= PointerMoveDrawLine;
            this.PointerReleased -= PointerPressedReleasedDrawLineToOut;

            var canvas = this.GetVisualDescendants().OfType<Canvas>().FirstOrDefault(
                canvas => string.IsNullOrEmpty(canvas.Name) == false &&
                          canvas.Name.Equals("mainCanvas"));

            var coords = pointerReleasedEventArgs.GetPosition(canvas);

            var element = canvas.InputHitTest(coords);
            MainUserControlViewModel viewModel =
                this.DataContext as MainUserControlViewModel;

            if (element is Ellipse ellipse)
            {
                if (ellipse.Name == "Output" || ellipse.Name == "Output1" ||
                    ellipse.Name == "Output2")
                {
                    if (viewModel.Elements[viewModel.Elements.Count - 1] is Connector
                            connector)
                    {
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
            if (routedEventArgs.Source is Avalonia.Controls.Control control)
            {
                MainUserControlViewModel viewModel =
                    DataContext as MainUserControlViewModel;
                if (control.DataContext is InputElement input)
                {
                    input.SignalOut = !input.SignalOut;
                    viewModel.SignalBypass();
                }
            }
        }

        private bool SetFocus()
        {
            MainUserControlViewModel viewModel =
                DataContext as MainUserControlViewModel;
            foreach (Element el in viewModel.Elements)
                el.FocusOnElement = false;
            return true;
        }

        private void DeleteElement(object sender, RoutedEventArgs routedEventArgs)
        {
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

        private void CloseWindowButtonClick(object sender,
                                            RoutedEventArgs routedEventArgs)
        {
            if (this.GetLogicalParent() is MainWindow mw)
                mw.Close();
        }
    }
}
