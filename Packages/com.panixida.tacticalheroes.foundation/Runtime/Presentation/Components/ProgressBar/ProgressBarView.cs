using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Foundation.Presentation.Components
{
    [UxmlElement]
    public sealed partial class ProgressBarView : VisualElement, INotifyValueChanged<float>
    {
        const string RootClass = "th-progress-bar";
        const string TrackClass = "th-progress-bar__track";
        const string FillClass = "th-progress-bar__fill";
        const float TrackWidth = 300f;
        const float FillInset = 2f;

        readonly VisualElement _fill;
        float _value;
        bool _dragging;

        public ProgressBarView()
        {
            AddToClassList(RootClass);
            focusable = true;
            pickingMode = PickingMode.Position;

            var track = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            track.AddToClassList(TrackClass);
            Add(track);

            _fill = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            _fill.AddToClassList(FillClass);
            Add(_fill);

            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<PointerUpEvent>(OnPointerUp);
            RegisterCallback<PointerCancelEvent>(OnPointerCancel);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<GeometryChangedEvent>(_ => SyncFill());
            SetValueWithoutNotify(0f);
        }

        public float value
        {
            get => _value;
            set => SetValue(value, true);
        }

        public event Action<float> ValueChanged;

        [UxmlAttribute("value")]
        public float Value
        {
            get => _value;
            set => SetValueWithoutNotify(value);
        }

        [UxmlAttribute("editable")]
        public bool Editable { get; set; } = true;

        public void SetValueWithoutNotify(float newValue)
        {
            SetValue(newValue, false);
        }

        void SetValue(float newValue, bool notify)
        {
            var previousValue = _value;
            _value = Mathf.Clamp(newValue, 0f, 100f);
            SyncFill();

            if (notify && !Mathf.Approximately(previousValue, _value))
            {
                using var changeEvent = ChangeEvent<float>.GetPooled(previousValue, _value);
                changeEvent.target = this;
                SendEvent(changeEvent);
                ValueChanged?.Invoke(_value);
            }
        }

        void SyncFill()
        {
            var width = resolvedStyle.width;
            if (float.IsNaN(width) || width <= 0f)
            {
                width = TrackWidth;
            }

            _fill.style.width = Mathf.Max(0f, (width - FillInset * 2f) * _value / 100f);
        }

        void SetValueFromPointer(PointerEventBase<PointerDownEvent> evt)
        {
            SetValueFromPosition(evt.position);
        }

        void SetValueFromPointer(PointerEventBase<PointerMoveEvent> evt)
        {
            SetValueFromPosition(evt.position);
        }

        void SetValueFromPosition(Vector3 panelPosition)
        {
            var localPosition = this.WorldToLocal(new Vector2(panelPosition.x, panelPosition.y));
            var width = resolvedStyle.width;
            if (float.IsNaN(width) || width <= 0f)
            {
                width = TrackWidth;
            }

            value = Mathf.Clamp01(localPosition.x / width) * 100f;
        }

        bool CanEdit()
        {
            return Editable && enabledInHierarchy;
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (!CanEdit())
            {
                return;
            }

            _dragging = true;
            this.CapturePointer(evt.pointerId);
            Focus();
            SetValueFromPointer(evt);
            evt.StopPropagation();
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_dragging || !this.HasPointerCapture(evt.pointerId))
            {
                return;
            }

            SetValueFromPointer(evt);
            evt.StopPropagation();
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (!_dragging)
            {
                return;
            }

            _dragging = false;
            if (this.HasPointerCapture(evt.pointerId))
            {
                this.ReleasePointer(evt.pointerId);
            }

            SetValueFromPosition(evt.position);
            evt.StopPropagation();
        }

        void OnPointerCancel(PointerCancelEvent evt)
        {
            _dragging = false;
            if (this.HasPointerCapture(evt.pointerId))
            {
                this.ReleasePointer(evt.pointerId);
            }
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            if (!CanEdit())
            {
                return;
            }

            switch (evt.keyCode)
            {
                case KeyCode.LeftArrow:
                case KeyCode.DownArrow:
                    value -= 1f;
                    evt.StopPropagation();
                    break;
                case KeyCode.RightArrow:
                case KeyCode.UpArrow:
                    value += 1f;
                    evt.StopPropagation();
                    break;
                case KeyCode.Home:
                    value = 0f;
                    evt.StopPropagation();
                    break;
                case KeyCode.End:
                    value = 100f;
                    evt.StopPropagation();
                    break;
            }
        }
    }
}
