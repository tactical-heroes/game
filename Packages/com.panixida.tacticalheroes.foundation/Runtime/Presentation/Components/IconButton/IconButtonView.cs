using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Foundation.Presentation.Components
{
    [UxmlElement]
    public sealed partial class IconButtonView : Button
    {
        public const string SelectedClass = "th-icon-button--selected";

        const string RootClass = "th-icon-button";
        const string HoverGlowClass = "th-icon-button__hover-glow";
        const string SelectedGlowClass = "th-icon-button__selected-glow";
        const string ActiveTintClass = "th-icon-button__active-tint";
        const string DisabledTintClass = "th-icon-button__disabled-tint";
        const string IconElementClass = "th-icon-button__icon";

        readonly VisualElement _icon;
        string _iconClass = string.Empty;
        bool _selected;

        public IconButtonView()
        {
            AddToClassList(RootClass);
            focusable = true;

            AddDecorativeElement(HoverGlowClass);
            AddDecorativeElement(SelectedGlowClass);
            AddDecorativeElement(ActiveTintClass);
            AddDecorativeElement(DisabledTintClass);

            _icon = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            _icon.AddToClassList(IconElementClass);
            Add(_icon);
        }

        [UxmlAttribute("icon-class")]
        public string IconClass
        {
            get => _iconClass;
            set
            {
                if (!string.IsNullOrEmpty(_iconClass))
                {
                    _icon.RemoveFromClassList(_iconClass);
                }

                _iconClass = value ?? string.Empty;

                if (!string.IsNullOrEmpty(_iconClass))
                {
                    _icon.AddToClassList(_iconClass);
                }
            }
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

        void AddDecorativeElement(string className)
        {
            var element = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            element.AddToClassList(className);
            Add(element);
        }
    }
}
