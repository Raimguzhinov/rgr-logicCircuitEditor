using Avalonia;
using Avalonia.Controls.Primitives;

namespace LogicCircuitEditor.Views.Control
{
    public class AndControl : TemplatedControl
    {
        public static readonly StyledProperty<bool> FocusOnElementProperty =
            AvaloniaProperty.Register<AndControl, bool>("FocusOnElement");

        public bool FocusOnElement
        {
            get => GetValue(FocusOnElementProperty);
            set => SetValue(FocusOnElementProperty, value);
        }
    }
}