using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using System.Linq;

namespace LogicCircuitEditor.Views.Control
{
    public class EditableTextBlock : TemplatedControl
    {
        public static readonly StyledProperty<string> CustomTextProperty =
            AvaloniaProperty.Register<EditableTextBlock, string>("CustomText", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public static readonly StyledProperty<double> FontSizeTextProperty =
            AvaloniaProperty.Register<EditableTextBlock, double>("FontSizeText");
        public static readonly StyledProperty<FontWeight> FontWeightTextProperty =
            AvaloniaProperty.Register<EditableTextBlock, FontWeight>("FontWeightText");

        public string CustomText
        {
            get => GetValue(CustomTextProperty);
            set => SetValue(CustomTextProperty, value);
        }
        public double FontSizeText
        {
            get => GetValue(FontSizeTextProperty);
            set => SetValue(FontSizeTextProperty, value);
        }
        public FontWeight FontWeightText
        {
            get => GetValue(FontWeightTextProperty);
            set => SetValue(FontWeightTextProperty, value);
        }

        static EditableTextBlock()
        {
            DoubleTappedEvent.AddClassHandler<EditableTextBlock>(
                (sender, args) => sender.OnDoubleTapped(args));
        }

        protected virtual void OnDoubleTapped(RoutedEventArgs routedEventArgs)
        {
            var descendants = this.GetVisualDescendants();


            var textBox = descendants.OfType<TextBox>()
                                     .FirstOrDefault(
                                         control =>
                                         string.IsNullOrEmpty(control.Name) == false &&
                                         control.Name.Equals("customTextBox"));

            if (textBox != null)
            {
                textBox.IsVisible = true;
                textBox.LostFocus += OnLostFocus;
                textBox.Focus();
            }


        }

        protected virtual void OnLostFocus(object? sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is TextBox textBox)
            {
                if (textBox != null)
                {
                    textBox.IsVisible = false;
                }
                if (textBox.Text == "") textBox.Text = "No name";
                textBox.LostFocus -= OnLostFocus;
            }
        }
    }
}