using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Foundation.Presentation.Components
{
    [UxmlElement]
    public sealed partial class CheckboxView : Button, INotifyValueChanged<bool>
    {
        public const string CheckedClass = "th-checkbox--checked";
        public const string ErrorClass = "th-checkbox--error";

        const string RootClass = "th-checkbox";
        const string BoxClass = "th-checkbox__box";
        const string CheckmarkClass = "th-checkbox__checkmark";
        const string CheckmarkShortClass = "th-checkbox__checkmark-short";
        const string CheckmarkLongClass = "th-checkbox__checkmark-long";

        bool _value;
        bool _error;

        public CheckboxView()
        {
            AddToClassList(RootClass);
            focusable = true;
            pickingMode = PickingMode.Position;

            AddDecorativeElement(BoxClass);

            var checkmark = AddDecorativeElement(CheckmarkClass);
            AddDecorativeElement(CheckmarkShortClass, checkmark);
            AddDecorativeElement(CheckmarkLongClass, checkmark);

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

        [UxmlAttribute("error")]
        public bool Error
        {
            get => _error;
            set
            {
                _error = value;
                EnableInClassList(ErrorClass, value);
            }
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
            EnableInClassList(CheckedClass, newValue);

            if (notify && previousValue != newValue)
            {
                using var changeEvent = ChangeEvent<bool>.GetPooled(previousValue, newValue);
                changeEvent.target = this;
                SendEvent(changeEvent);
                ValueChanged?.Invoke(newValue);
            }
        }

        VisualElement AddDecorativeElement(string className, VisualElement parent = null)
        {
            var element = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            element.AddToClassList(className);
            (parent ?? this).Add(element);
            return element;
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
