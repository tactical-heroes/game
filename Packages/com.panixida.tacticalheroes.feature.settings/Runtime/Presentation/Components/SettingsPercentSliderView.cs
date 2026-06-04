using System;
using System.Globalization;
using Panixida.TacticalHeroes.Foundation.Presentation.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Features.Settings.Presentation.Components
{
    [UxmlElement]
    public sealed partial class SettingsPercentSliderView : VisualElement, INotifyValueChanged<float>
    {
        const string RootClass = "settings-percent-slider";
        const string ProgressClass = "settings-percent-slider__progress";
        const string FieldClass = "settings-percent-slider__field";

        readonly ProgressBarView _progress;
        readonly TextField _field;
        bool _syncing;
        float _value;

        public SettingsPercentSliderView()
        {
            AddToClassList(RootClass);

            _progress = new ProgressBarView();
            _progress.AddToClassList(ProgressClass);
            _progress.RegisterValueChangedCallback(OnProgressValueChanged);
            Add(_progress);

            _field = new TextField
            {
                isDelayed = false
            };
            _field.AddToClassList(FieldClass);
            _field.RegisterValueChangedCallback(OnFieldValueChanged);
            _field.RegisterCallback<BlurEvent>(_ => UpdateFieldText());
            Add(_field);

            SetValueWithoutNotify(0f);
        }

        public float value
        {
            get => _value;
            set => SetValue(value, true, true);
        }

        public event Action<float> ValueChanged;

        [UxmlAttribute("value")]
        public float Value
        {
            get => _value;
            set => SetValueWithoutNotify(value);
        }

        public void SetValueWithoutNotify(float newValue)
        {
            SetValue(newValue, false, true);
        }

        void SetValue(float newValue, bool notify, bool syncField)
        {
            var previousValue = _value;
            _value = Mathf.Clamp(newValue, 0f, 100f);

            _syncing = true;
            _progress.SetValueWithoutNotify(_value);
            if (syncField)
            {
                UpdateFieldText();
            }
            _syncing = false;

            if (notify && !Mathf.Approximately(previousValue, _value))
            {
                using var changeEvent = ChangeEvent<float>.GetPooled(previousValue, _value);
                changeEvent.target = this;
                SendEvent(changeEvent);
                ValueChanged?.Invoke(_value);
            }
        }

        void OnProgressValueChanged(ChangeEvent<float> evt)
        {
            if (_syncing)
            {
                return;
            }

            value = evt.newValue;
        }

        void OnFieldValueChanged(ChangeEvent<string> evt)
        {
            if (_syncing)
            {
                return;
            }

            if (!TryParsePercent(evt.newValue, out var parsedValue))
            {
                UpdateFieldText();
                return;
            }

            SetValue(parsedValue, true, true);
        }

        void UpdateFieldText()
        {
            _field.SetValueWithoutNotify($"{Mathf.RoundToInt(_value)}%");
        }

        static bool TryParsePercent(string text, out float parsedValue)
        {
            parsedValue = 0f;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var normalizedText = text.Trim().TrimEnd('%').Trim();
            return float.TryParse(normalizedText, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedValue)
                || float.TryParse(normalizedText, NumberStyles.Float, CultureInfo.CurrentCulture, out parsedValue);
        }
    }
}
