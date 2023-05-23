using Avalonia;
using Avalonia.Controls.Primitives;

namespace LogicCircuitEditor.Views.Control
{
    public class OutputControl : TemplatedControl
    {
        public static readonly StyledProperty<bool> SignalProperty =
            AvaloniaProperty.Register<OutputControl, bool>("Signal");
        public static readonly StyledProperty<bool> FocusOnElementProperty =
            AvaloniaProperty.Register<OutputControl, bool>("FocusOnElement");

        public bool FocusOnElement
        {
            get => GetValue(FocusOnElementProperty);
            set => SetValue(FocusOnElementProperty, value);
        }

        public bool Signal
        {
            get => GetValue(SignalProperty);
            set => SetValue(SignalProperty, value);
        }
    }
}