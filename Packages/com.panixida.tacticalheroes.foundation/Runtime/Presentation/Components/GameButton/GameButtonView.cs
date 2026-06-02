using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Foundation.Presentation.Components
{
    [UxmlElement]
    public sealed partial class GameButtonView : Button
    {
        public const string SelectedClass = "th-button--selected";

        const string RootClass = "th-button";
        const string TintClass = "th-button__tint";
        const string GlowClass = "th-button__glow";
        const string PressedShadeClass = "th-button__pressed-shade";
        const string LabelStackClass = "th-button__label-stack";
        const string LabelLayerClass = "th-button__label-layer";

        readonly Label[] _labelLayers;
        string _buttonText = string.Empty;
        bool _selected;

        public GameButtonView()
        {
            AddToClassList(RootClass);
            focusable = true;

            AddDecorativeElement(TintClass);
            AddDecorativeElement(GlowClass);
            AddDecorativeElement(PressedShadeClass);

            var labelStack = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            labelStack.AddToClassList(LabelStackClass);
            Add(labelStack);

            _labelLayers = new[]
            {
                CreateLabel("th-button__label-shadow"),
                CreateLabel("th-button__label-glow"),
                CreateLabel("th-button__label"),
                CreateLabel("th-button__label-highlight")
            };

            foreach (var layer in _labelLayers)
            {
                labelStack.Add(layer);
            }
        }

        public new string text
        {
            get => _buttonText;
            set => SetText(value);
        }

        [UxmlAttribute("label")]
        public string Label
        {
            get => _buttonText;
            set => SetText(value);
        }

        [UxmlAttribute("selected")]
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                EnableInClassList(SelectedClass, value);
            }
        }

        void SetText(string value)
        {
            _buttonText = value ?? string.Empty;

            foreach (var layer in _labelLayers)
            {
                layer.text = _buttonText;
            }
        }

        void AddDecorativeElement(string className)
        {
            var element = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            element.AddToClassList(className);
            Add(element);
        }

        static Label CreateLabel(string className)
        {
            var label = new Label
            {
                pickingMode = PickingMode.Ignore
            };
            label.AddToClassList(LabelLayerClass);
            label.AddToClassList(className);
            return label;
        }
    }
}
