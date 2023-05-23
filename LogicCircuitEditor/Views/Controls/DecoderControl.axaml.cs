using Avalonia;
using Avalonia.Controls.Primitives;

namespace LogicCircuitEditor.Views.Control
{
    public class DecoderControl : TemplatedControl
    {
        public static readonly StyledProperty<bool> FocusOnElementProperty =
            AvaloniaProperty.Register<DecoderControl, bool>("FocusOnElement");

        public bool FocusOnElement
        {
            get => GetValue(FocusOnElementProperty);
            set => SetValue(FocusOnElementProperty, value);
        }
    }
}
