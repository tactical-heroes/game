using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Foundation.Presentation.Components
{
    [UxmlElement]
    public sealed partial class DropdownView : Button
    {
        public const string OpenClass = "th-dropdown--open";

        const string RootClass = "th-dropdown";
        const string GlowClass = "th-dropdown__glow";
        const string BackgroundClass = "th-dropdown__background";
        const string TopHighlightClass = "th-dropdown__top-highlight";
        const string ValueClass = "th-dropdown__value";
        const string ArrowClass = "th-dropdown__arrow";
        const string MenuClass = "th-dropdown__menu";
        const string OptionClass = "th-dropdown__option";
        const string SelectedOptionClass = "th-dropdown__option--selected";
        const int MenuTop = 46;
        const int MenuWidth = 252;
        const int OptionHeight = 36;
        static readonly Color TransparentColor = new(0f, 0f, 0f, 0f);
        static readonly Color MenuBackgroundColor = new(0f, 0f, 0f, 0.84f);
        static readonly Color MenuBorderColor = new(199f / 255f, 125f / 255f, 41f / 255f, 0.78f);
        static readonly Color OptionTextColor = new(182f / 255f, 146f / 255f, 90f / 255f, 1f);
        static readonly Color ActiveOptionTextColor = new(234f / 255f, 209f / 255f, 153f / 255f, 1f);
        static readonly Color ActiveOptionBackgroundColor = new(58f / 255f, 27f / 255f, 8f / 255f, 0.72f);

        readonly List<string> _options = new();
        readonly Label _valueLabel;
        readonly VisualElement _menu;
        VisualElement _rootWithCloseCallback;
        string _value = string.Empty;
        bool _isOpen;
        bool _ignoreNextClick;

        public DropdownView()
        {
            AddToClassList(RootClass);
            focusable = true;

            AddDecorativeElement(GlowClass);
            AddDecorativeElement(BackgroundClass);
            AddDecorativeElement(TopHighlightClass);

            _valueLabel = new Label
            {
                pickingMode = PickingMode.Ignore
            };
            _valueLabel.AddToClassList(ValueClass);
            Add(_valueLabel);

            AddDecorativeElement(ArrowClass);

            _menu = new VisualElement
            {
                pickingMode = PickingMode.Position
            };
            _menu.AddToClassList(MenuClass);
            ApplyMenuFallbackStyles();
            Add(_menu);

            RegisterCallback<ClickEvent>(OnDropdownClicked);
            RegisterCallback<BlurEvent>(OnBlur);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public event Action<string> ValueChanged;

        public new string text
        {
            get => Value;
            set => Value = value;
        }

        [UxmlAttribute("value")]
        public string Value
        {
            get => _value;
            set => SetValue(value, false);
        }

        [UxmlAttribute("options")]
        public string Options
        {
            get => string.Join("|", _options);
            set => SetOptions(value);
        }

        public void Open()
        {
            if (_isOpen || _options.Count == 0 || !enabledInHierarchy)
            {
                return;
            }

            CloseOtherDropdowns();
            _isOpen = true;
            EnableInClassList(OpenClass, true);
            AttachMenuToRoot();
            RegisterRootCloseCallback();
            parent?.BringToFront();
            BringToFront();
            _menu.BringToFront();
        }

        public void Close()
        {
            if (!_isOpen)
            {
                return;
            }

            _isOpen = false;
            EnableInClassList(OpenClass, false);
            RestoreMenuToDropdown();
            UnregisterRootCloseCallback();
        }

        void ToggleOpen()
        {
            if (_isOpen)
            {
                Close();
                return;
            }

            Open();
        }

        void SetOptions(string value)
        {
            _options.Clear();

            if (!string.IsNullOrWhiteSpace(value))
            {
                var options = value.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (var option in options)
                {
                    var trimmedOption = option.Trim();
                    if (!string.IsNullOrEmpty(trimmedOption))
                    {
                        _options.Add(trimmedOption);
                    }
                }
            }

            RebuildMenu();

            if (_options.Count == 0)
            {
                Close();
            }
            else if (_isOpen)
            {
                AttachMenuToRoot();
            }

            if (string.IsNullOrEmpty(_value) && _options.Count > 0)
            {
                SetValue(_options[0], false);
            }
            else
            {
                SyncOptionSelection();
            }
        }

        void RebuildMenu()
        {
            _menu.Clear();

            foreach (var option in _options)
            {
                var optionElement = new Label(option)
                {
                    pickingMode = PickingMode.Position,
                    userData = option
                };
                optionElement.AddToClassList(OptionClass);
                ApplyOptionFallbackStyles(optionElement);
                optionElement.RegisterCallback<PointerDownEvent>(OnOptionPointerDown);
                optionElement.RegisterCallback<PointerEnterEvent>(OnOptionPointerEnter);
                optionElement.RegisterCallback<PointerLeaveEvent>(OnOptionPointerLeave);
                optionElement.RegisterCallback<ClickEvent>(OnOptionClicked);
                _menu.Add(optionElement);
            }
        }

        void SetValue(string value, bool notify)
        {
            var nextValue = value ?? string.Empty;
            var previousValue = _value;

            _value = nextValue;
            _valueLabel.text = _value;
            SyncOptionSelection();

            if (notify && previousValue != _value)
            {
                ValueChanged?.Invoke(_value);
            }
        }

        void SyncOptionSelection()
        {
            foreach (var option in _menu.Children())
            {
                ApplyOptionState(option, false);
            }
        }

        void SelectOffsetOption(int offset)
        {
            if (_options.Count == 0)
            {
                return;
            }

            var selectedIndex = _options.IndexOf(_value);
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex = (selectedIndex + offset + _options.Count) % _options.Count;
            }

            SetValue(_options[selectedIndex], true);
        }

        void CloseOtherDropdowns()
        {
            var root = GetRootElement();
            root.Query<DropdownView>().ForEach(dropdown =>
            {
                if (dropdown != this)
                {
                    dropdown.Close();
                }
            });
        }

        void RegisterRootCloseCallback()
        {
            var root = GetRootElement();
            if (_rootWithCloseCallback == root)
            {
                return;
            }

            UnregisterRootCloseCallback();
            _rootWithCloseCallback = root;
            _rootWithCloseCallback.RegisterCallback<PointerDownEvent>(OnRootPointerDown, TrickleDown.TrickleDown);
        }

        void UnregisterRootCloseCallback()
        {
            if (_rootWithCloseCallback == null)
            {
                return;
            }

            _rootWithCloseCallback.UnregisterCallback<PointerDownEvent>(OnRootPointerDown, TrickleDown.TrickleDown);
            _rootWithCloseCallback = null;
        }

        VisualElement GetRootElement()
        {
            var root = (VisualElement)this;
            while (root.parent != null)
            {
                root = root.parent;
            }

            return root;
        }

        void AttachMenuToRoot()
        {
            var overlayRoot = GetOverlayRootElement();
            AttachMenuStyleSheets();

            if (_menu.parent != overlayRoot)
            {
                _menu.RemoveFromHierarchy();
                overlayRoot.Add(_menu);
            }

            var position = overlayRoot.WorldToLocal(worldBound.position);
            _menu.style.position = Position.Absolute;
            _menu.style.left = position.x;
            _menu.style.top = position.y + MenuTop;
            _menu.style.display = DisplayStyle.Flex;
            ApplyMenuFallbackStyles();
            SyncOptionSelection();
        }

        void RestoreMenuToDropdown()
        {
            _menu.style.display = DisplayStyle.None;
            _menu.style.left = 0;
            _menu.style.top = MenuTop;

            if (_menu.parent == this)
            {
                return;
            }

            _menu.RemoveFromHierarchy();
            Add(_menu);
        }

        VisualElement GetOverlayRootElement()
        {
            var documentRoot = GetRootElement();
            var overlayRoot = (VisualElement)this;

            while (overlayRoot.parent != null && overlayRoot.parent != documentRoot)
            {
                overlayRoot = overlayRoot.parent;
            }

            return overlayRoot == this ? documentRoot : overlayRoot;
        }

        void AttachMenuStyleSheets()
        {
            for (VisualElement current = this; current != null; current = current.parent)
            {
                var styleSheets = current.styleSheets;
                for (var i = 0; i < styleSheets.count; i++)
                {
                    var styleSheet = styleSheets[i];
                    if (!_menu.styleSheets.Contains(styleSheet))
                    {
                        _menu.styleSheets.Add(styleSheet);
                    }
                }
            }
        }

        void ApplyMenuFallbackStyles()
        {
            _menu.style.width = MenuWidth;
            _menu.style.flexDirection = FlexDirection.Column;
            _menu.style.backgroundColor = MenuBackgroundColor;
            _menu.style.borderTopLeftRadius = 2;
            _menu.style.borderTopRightRadius = 2;
            _menu.style.borderBottomLeftRadius = 2;
            _menu.style.borderBottomRightRadius = 2;
            _menu.style.borderLeftWidth = 1;
            _menu.style.borderRightWidth = 1;
            _menu.style.borderTopWidth = 1;
            _menu.style.borderBottomWidth = 1;
            _menu.style.borderLeftColor = MenuBorderColor;
            _menu.style.borderRightColor = MenuBorderColor;
            _menu.style.borderTopColor = MenuBorderColor;
            _menu.style.borderBottomColor = MenuBorderColor;
            _menu.style.paddingLeft = 0;
            _menu.style.paddingRight = 0;
            _menu.style.paddingTop = 1;
            _menu.style.paddingBottom = 3;
        }

        static void ApplyOptionFallbackStyles(VisualElement option)
        {
            option.style.height = OptionHeight;
            option.style.fontSize = 18;
            option.style.unityTextAlign = TextAnchor.MiddleLeft;
            option.style.marginLeft = 0;
            option.style.marginRight = 0;
            option.style.marginTop = 0;
            option.style.marginBottom = 0;
            option.style.paddingLeft = 15;
            option.style.paddingRight = 15;
            option.style.paddingTop = 0;
            option.style.paddingBottom = 0;
            ApplyOptionBaseStyle(option);
        }

        static void ApplyOptionBaseStyle(VisualElement option)
        {
            option.style.color = OptionTextColor;
            option.style.backgroundColor = TransparentColor;
        }

        void ApplyOptionState(VisualElement option, bool highlighted)
        {
            var selected = option.userData is string optionValue && optionValue == _value;
            option.EnableInClassList(SelectedOptionClass, selected);

            if (selected || highlighted)
            {
                option.style.color = ActiveOptionTextColor;
                option.style.backgroundColor = ActiveOptionBackgroundColor;
                return;
            }

            ApplyOptionBaseStyle(option);
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

        void OnDropdownClicked(ClickEvent evt)
        {
            if (_ignoreNextClick)
            {
                _ignoreNextClick = false;
                evt.StopPropagation();
                return;
            }

            ToggleOpen();
            evt.StopPropagation();
        }

        void OnOptionPointerDown(PointerDownEvent evt)
        {
            if (evt.currentTarget is not VisualElement optionElement || optionElement.userData is not string option)
            {
                return;
            }

            _ignoreNextClick = true;
            SetValue(option, true);
            Close();
            Focus();
            evt.StopImmediatePropagation();
        }

        void OnOptionClicked(ClickEvent evt)
        {
            evt.StopPropagation();
        }

        void OnOptionPointerEnter(PointerEnterEvent evt)
        {
            if (evt.currentTarget is VisualElement optionElement)
            {
                ApplyOptionState(optionElement, true);
            }
        }

        void OnOptionPointerLeave(PointerLeaveEvent evt)
        {
            if (evt.currentTarget is VisualElement optionElement)
            {
                ApplyOptionState(optionElement, false);
            }
        }

        void OnBlur(BlurEvent evt)
        {
            Close();
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            Close();
        }

        void OnRootPointerDown(PointerDownEvent evt)
        {
            if (evt.target is VisualElement target && (IsSelfOrDescendant(target) || IsMenuOrDescendant(target)))
            {
                return;
            }

            Close();
        }

        bool IsSelfOrDescendant(VisualElement target)
        {
            for (var current = target; current != null; current = current.parent)
            {
                if (current == this)
                {
                    return true;
                }
            }

            return false;
        }

        bool IsMenuOrDescendant(VisualElement target)
        {
            for (var current = target; current != null; current = current.parent)
            {
                if (current == _menu)
                {
                    return true;
                }
            }

            return false;
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                case KeyCode.Space:
                    ToggleOpen();
                    evt.StopPropagation();
                    break;
                case KeyCode.Escape:
                    Close();
                    evt.StopPropagation();
                    break;
                case KeyCode.DownArrow:
                    if (!_isOpen)
                    {
                        Open();
                    }
                    else
                    {
                        SelectOffsetOption(1);
                    }
                    evt.StopPropagation();
                    break;
                case KeyCode.UpArrow:
                    if (!_isOpen)
                    {
                        Open();
                    }
                    else
                    {
                        SelectOffsetOption(-1);
                    }
                    evt.StopPropagation();
                    break;
            }
        }
    }
}
