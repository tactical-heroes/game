using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Foundation.Presentation.Components
{
    [UxmlElement]
    public sealed partial class InputView : TextField
    {
        public const string ErrorClass = "th-input--error";

        const string RootClass = "th-input";
        const string FrameClass = "th-input__frame";
        const string FocusedClass = "th-input--focused";
        const string PasswordClass = "th-input--password";
        const string PlaceholderClass = "th-input__placeholder";
        const string PlaceholderHiddenClass = "th-input__placeholder--hidden";
        const string ValidationMessageClass = "th-input__validation-message";
        const string ValidationMessageVisibleClass = "th-input__validation-message--visible";

        readonly VisualElement _frame;
        readonly Label _placeholderLabel;
        readonly Label _validationLabel;
        string _placeholder = string.Empty;
        string _validationMessage = string.Empty;
        bool _error;

        public InputView()
        {
            AddToClassList(RootClass);
            focusable = true;

            _frame = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            _frame.AddToClassList(FrameClass);
            Insert(0, _frame);

            _placeholderLabel = new Label
            {
                pickingMode = PickingMode.Ignore
            };
            _placeholderLabel.AddToClassList(PlaceholderClass);
            Add(_placeholderLabel);

            _validationLabel = new Label
            {
                pickingMode = PickingMode.Ignore
            };
            _validationLabel.AddToClassList(ValidationMessageClass);
            Add(_validationLabel);

            this.RegisterValueChangedCallback(_ => SyncPlaceholderVisibility());
            RegisterCallback<FocusInEvent>(_ => EnableInClassList(FocusedClass, true));
            RegisterCallback<FocusOutEvent>(_ => EnableInClassList(FocusedClass, false));
            RegisterCallback<AttachToPanelEvent>(_ => SyncPlaceholderVisibility());
            SyncPlaceholderVisibility();
            SyncValidationMessage();
        }

        [UxmlAttribute("placeholder")]
        public string Placeholder
        {
            get => _placeholder;
            set
            {
                _placeholder = value ?? string.Empty;
                _placeholderLabel.text = _placeholder;
                SyncPlaceholderVisibility();
            }
        }

        [UxmlAttribute("password")]
        public bool Password
        {
            get => isPasswordField;
            set
            {
                isPasswordField = value;
                EnableInClassList(PasswordClass, value);
            }
        }

        [UxmlAttribute("error")]
        public bool Error
        {
            get => _error;
            set
            {
                _error = value;
                EnableInClassList(ErrorClass, value);
                SyncValidationMessage();
            }
        }

        [UxmlAttribute("validation-message")]
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value ?? string.Empty;
                _validationLabel.text = _validationMessage;
                SyncValidationMessage();
            }
        }

        public new void SetValueWithoutNotify(string newValue)
        {
            base.SetValueWithoutNotify(newValue);
            SyncPlaceholderVisibility();
        }

        void SyncPlaceholderVisibility()
        {
            _placeholderLabel.EnableInClassList(
                PlaceholderHiddenClass,
                string.IsNullOrEmpty(_placeholder) || !string.IsNullOrEmpty(value));
        }

        void SyncValidationMessage()
        {
            _validationLabel.EnableInClassList(
                ValidationMessageVisibleClass,
                _error && !string.IsNullOrEmpty(_validationMessage));
        }
    }
}
