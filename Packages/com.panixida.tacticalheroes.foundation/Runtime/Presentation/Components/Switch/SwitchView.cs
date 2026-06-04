using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Foundation.Presentation.Components
{
    [UxmlElement]
    public sealed partial class SwitchView : Button, INotifyValueChanged<bool>
    {
        public const string OnClass = "th-switch--on";

        const string RootClass = "th-switch";
        const string TrackClass = "th-switch__track";
        const string HoverGlowClass = "th-switch__hover-glow";
        const string PressedShadeClass = "th-switch__pressed-shade";
        const string LabelClass = "th-switch__label";
        const string KnobClass = "th-switch__knob";

        readonly Label _label;
        bool _value;

        public SwitchView()
        {
            AddToClassList(RootClass);
            focusable = true;
            pickingMode = PickingMode.Position;

            AddDecorativeElement(TrackClass);
            AddDecorativeElement(HoverGlowClass);
            AddDecorativeElement(PressedShadeClass);

            _label = new Label
            {
                pickingMode = PickingMode.Ignore
            };
            _label.AddToClassList(LabelClass);
            Add(_label);

            AddDecorativeElement(KnobClass);
            SetValueWithoutNotify(false);

            clicked += Toggle;
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        public event Action<bool> ValueChanged;

        public bool value
        {
            get => _value;
            set => SetValue(value, true);
        }

        [UxmlAttribute("value")]
        public bool Value
        {
            get => _value;
            set => SetValueWithoutNotify(value);
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            SetValue(newValue, false);
        }

        public void Toggle()
        {
            if (!enabledInHierarchy)
            {
                return;
            }

            value = !_value;
        }

        void SetValue(bool newValue, bool notify)
        {
            var previousValue = _value;
            _value = newValue;
            EnableInClassList(OnClass, newValue);
            _label.text = newValue ? "ON" : "OFF";

            if (notify && previousValue != newValue)
            {
                using var changeEvent = ChangeEvent<bool>.GetPooled(previousValue, newValue);
                changeEvent.target = this;
                SendEvent(changeEvent);
                ValueChanged?.Invoke(newValue);
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

        void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                case KeyCode.Space:
                    Toggle();
                    evt.StopPropagation();
                    break;
            }
        }
    }
}
