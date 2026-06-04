using System.Collections;
using System.Collections.Generic;
using Panixida.TacticalHeroes.Foundation.Presentation.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Features.MainMenu.Presentation
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class MainMenuView : MonoBehaviour
    {
        const string ScreenHiddenClass = "screen-hidden";
        const string AuthPlaceholderHiddenClass = "auth-placeholder-hidden";
        const string AuthCheckboxSelectedClass = "auth-checkbox--selected";

        [SerializeField] VisualTreeAsset _settingsViewAsset;
        [SerializeField] VisualTreeAsset _authorizationViewAsset;

        static readonly string[] NonInteractiveClasses =
        {
            "menu-dark-overlay",
            "auth-dark-overlay",
            "auth-readability-panel",
            "auth-panel-vignette",
            "auth-logo",
            "auth-title-stack",
            "auth-title-layer",
            "auth-title-separator",
            "auth-subtitle",
            "auth-field-label",
            "auth-placeholder",
            "auth-validation-message",
            "auth-eye-icon",
            "auth-option-label",
            "auth-footer-ornament",
            "auth-version-label",
            "auth-icon-button-overlay",
            "menu-logo",
            "menu-title-stack",
            "menu-title-layer",
            "title-separator",
            "menu-subtitle",
            "footer-ornament",
            "version-label",
            "icon-button-overlay",
            "th-panel-chrome",
            "th-panel-shadow",
            "th-panel-background",
            "th-panel-border",
            "th-panel-inner-border",
            "th-panel-corner",
            "th-panel-corner-line",
            "th-panel-corner-diamond",
            "settings-row-icon",
            "settings-dropdown-arrow",
            "settings-toggle-knob"
        };

        static readonly SettingsSnapshot DefaultSettings = new(
            "1920x1080 (16:9)",
            "Fullscreen",
            "120 FPS",
            75f,
            "High",
            "High",
            "High",
            72f,
            85f,
            "English",
            true);

        readonly List<GameButtonView> _menuButtons = new();
        UIDocument _document;
        Coroutine _bindCoroutine;
        VisualElement _menuRoot;
        VisualElement _settingsContainer;
        VisualElement _settingsRoot;
        VisualElement _authorizationContainer;
        VisualElement _authorizationRoot;
        VisualElement _authSignInPage;
        VisualElement _authCreateAccountPage;
        DropdownView _settingsResolutionDropdown;
        DropdownView _settingsDisplayModeDropdown;
        DropdownView _settingsFrameRateDropdown;
        DropdownView _settingsOverallQualityDropdown;
        DropdownView _settingsTexturesDropdown;
        DropdownView _settingsEffectsQualityDropdown;
        DropdownView _settingsLanguageDropdown;
        INotifyValueChanged<float> _settingsBrightnessSlider;
        INotifyValueChanged<float> _settingsMusicVolumeSlider;
        INotifyValueChanged<float> _settingsEffectsVolumeSlider;
        SwitchView _settingsShowDamageNumbersSwitch;
        SettingsSnapshot _appliedSettings = DefaultSettings;
        bool _isSettingsVisible;
        bool _isAuthorizationVisible;
        bool _isBound;

        void OnEnable()
        {
            _document = GetComponent<UIDocument>();
            _bindCoroutine = StartCoroutine(BindWhenDocumentReady());
        }

        void OnDisable()
        {
            if (_bindCoroutine != null)
            {
                StopCoroutine(_bindCoroutine);
                _bindCoroutine = null;
            }

            Unbind();
        }

        IEnumerator BindWhenDocumentReady()
        {
            yield return null;

            if (!isActiveAndEnabled)
            {
                yield break;
            }

            Bind();
            _bindCoroutine = null;
        }

        void Bind()
        {
            Unbind();

            var root = _document.rootVisualElement;
            if (root == null)
            {
                Debug.LogWarning("Main menu UIDocument root was not found.");
                return;
            }

            _menuRoot = root.Q<VisualElement>("menu-root");
            if (_menuRoot == null)
            {
                Debug.LogWarning("Main menu root was not found in UIDocument.");
                return;
            }

            CreateSettingsView(root);
            CreateAuthorizationView(root);
            SetNonInteractivePicking(root);

            SyncLayeredTitleText(root);

            _menuButtons.Clear();
            root.Query<GameButtonView>(className: "menu-button").ForEach(_menuButtons.Add);

            foreach (var button in _menuButtons)
            {
                button.RegisterCallback<ClickEvent>(OnMenuButtonClicked);
            }

            root.Q<Button>("discord-button")?.RegisterCallback<ClickEvent>(OnDiscordClicked);
            root.Q<Button>("support-button")?.RegisterCallback<ClickEvent>(OnSupportClicked);
            root.Q<Button>("footer-settings-button")?.RegisterCallback<ClickEvent>(OnFooterSettingsClicked);

            root.Q<Button>("settings-back-button")?.RegisterCallback<ClickEvent>(OnSettingsBackClicked);
            root.Q<Button>("settings-cancel-button")?.RegisterCallback<ClickEvent>(OnSettingsCancelClicked);
            root.Q<Button>("settings-restore-defaults-button")?.RegisterCallback<ClickEvent>(OnSettingsRestoreDefaultsClicked);
            root.Q<Button>("settings-apply-button")?.RegisterCallback<ClickEvent>(OnSettingsApplyClicked);

            root.Q<Button>("auth-sign-in-submit-button")?.RegisterCallback<ClickEvent>(OnAuthorizationSignInClicked);
            root.Q<Button>("auth-show-create-account-button")?.RegisterCallback<ClickEvent>(OnShowCreateAccountClicked);
            root.Q<Button>("auth-sign-in-back-button")?.RegisterCallback<ClickEvent>(OnAuthorizationBackClicked);
            root.Q<Button>("auth-create-account-submit-button")?.RegisterCallback<ClickEvent>(OnAuthorizationCreateAccountClicked);
            root.Q<Button>("auth-back-to-sign-in-button")?.RegisterCallback<ClickEvent>(OnShowSignInClicked);
            root.Q<Button>("auth-remember-checkbox")?.RegisterCallback<ClickEvent>(OnAuthCheckboxClicked);
            root.Q<Button>("auth-terms-checkbox")?.RegisterCallback<ClickEvent>(OnAuthCheckboxClicked);
            root.Q<Button>("auth-forgot-password-button")?.RegisterCallback<ClickEvent>(OnForgotPasswordClicked);
            root.Q<Button>("auth-sign-in-password-visibility-button")?.RegisterCallback<ClickEvent>(OnAuthPasswordVisibilityClicked);
            root.Q<Button>("auth-create-password-visibility-button")?.RegisterCallback<ClickEvent>(OnAuthPasswordVisibilityClicked);
            root.Q<Button>("auth-create-confirm-password-visibility-button")?.RegisterCallback<ClickEvent>(OnAuthPasswordVisibilityClicked);
            root.Q<Button>("auth-sign-in-discord-button")?.RegisterCallback<ClickEvent>(OnDiscordClicked);
            root.Q<Button>("auth-sign-in-support-button")?.RegisterCallback<ClickEvent>(OnSupportClicked);
            root.Q<Button>("auth-sign-in-settings-button")?.RegisterCallback<ClickEvent>(OnFooterSettingsClicked);
            root.Q<Button>("auth-create-discord-button")?.RegisterCallback<ClickEvent>(OnDiscordClicked);
            root.Q<Button>("auth-create-support-button")?.RegisterCallback<ClickEvent>(OnSupportClicked);
            root.Q<Button>("auth-create-settings-button")?.RegisterCallback<ClickEvent>(OnFooterSettingsClicked);
            BindAuthTextFields(root);
            CacheSettingsControls(root);
            ApplySettingsToControls(_appliedSettings);

            ShowMenu();
            _isBound = true;
        }

        void Unbind()
        {
            if (!_isBound || _document == null)
            {
                return;
            }

            var root = _document.rootVisualElement;
            foreach (var button in _menuButtons)
            {
                button.UnregisterCallback<ClickEvent>(OnMenuButtonClicked);
            }

            if (root != null)
            {
                root.Q<Button>("discord-button")?.UnregisterCallback<ClickEvent>(OnDiscordClicked);
                root.Q<Button>("support-button")?.UnregisterCallback<ClickEvent>(OnSupportClicked);
                root.Q<Button>("footer-settings-button")?.UnregisterCallback<ClickEvent>(OnFooterSettingsClicked);

                root.Q<Button>("settings-back-button")?.UnregisterCallback<ClickEvent>(OnSettingsBackClicked);
                root.Q<Button>("settings-cancel-button")?.UnregisterCallback<ClickEvent>(OnSettingsCancelClicked);
                root.Q<Button>("settings-restore-defaults-button")?.UnregisterCallback<ClickEvent>(OnSettingsRestoreDefaultsClicked);
                root.Q<Button>("settings-apply-button")?.UnregisterCallback<ClickEvent>(OnSettingsApplyClicked);

                root.Q<Button>("auth-sign-in-submit-button")?.UnregisterCallback<ClickEvent>(OnAuthorizationSignInClicked);
                root.Q<Button>("auth-show-create-account-button")?.UnregisterCallback<ClickEvent>(OnShowCreateAccountClicked);
                root.Q<Button>("auth-sign-in-back-button")?.UnregisterCallback<ClickEvent>(OnAuthorizationBackClicked);
                root.Q<Button>("auth-create-account-submit-button")?.UnregisterCallback<ClickEvent>(OnAuthorizationCreateAccountClicked);
                root.Q<Button>("auth-back-to-sign-in-button")?.UnregisterCallback<ClickEvent>(OnShowSignInClicked);
                root.Q<Button>("auth-remember-checkbox")?.UnregisterCallback<ClickEvent>(OnAuthCheckboxClicked);
                root.Q<Button>("auth-terms-checkbox")?.UnregisterCallback<ClickEvent>(OnAuthCheckboxClicked);
                root.Q<Button>("auth-forgot-password-button")?.UnregisterCallback<ClickEvent>(OnForgotPasswordClicked);
                root.Q<Button>("auth-sign-in-password-visibility-button")?.UnregisterCallback<ClickEvent>(OnAuthPasswordVisibilityClicked);
                root.Q<Button>("auth-create-password-visibility-button")?.UnregisterCallback<ClickEvent>(OnAuthPasswordVisibilityClicked);
                root.Q<Button>("auth-create-confirm-password-visibility-button")?.UnregisterCallback<ClickEvent>(OnAuthPasswordVisibilityClicked);
                root.Q<Button>("auth-sign-in-discord-button")?.UnregisterCallback<ClickEvent>(OnDiscordClicked);
                root.Q<Button>("auth-sign-in-support-button")?.UnregisterCallback<ClickEvent>(OnSupportClicked);
                root.Q<Button>("auth-sign-in-settings-button")?.UnregisterCallback<ClickEvent>(OnFooterSettingsClicked);
                root.Q<Button>("auth-create-discord-button")?.UnregisterCallback<ClickEvent>(OnDiscordClicked);
                root.Q<Button>("auth-create-support-button")?.UnregisterCallback<ClickEvent>(OnSupportClicked);
                root.Q<Button>("auth-create-settings-button")?.UnregisterCallback<ClickEvent>(OnFooterSettingsClicked);
                UnbindAuthTextFields(root);
            }

            _menuButtons.Clear();
            _menuRoot = null;
            _settingsContainer = null;
            _settingsRoot = null;
            _authorizationContainer = null;
            _authorizationRoot = null;
            _authSignInPage = null;
            _authCreateAccountPage = null;
            _settingsResolutionDropdown = null;
            _settingsDisplayModeDropdown = null;
            _settingsFrameRateDropdown = null;
            _settingsOverallQualityDropdown = null;
            _settingsTexturesDropdown = null;
            _settingsEffectsQualityDropdown = null;
            _settingsLanguageDropdown = null;
            _settingsBrightnessSlider = null;
            _settingsMusicVolumeSlider = null;
            _settingsEffectsVolumeSlider = null;
            _settingsShowDamageNumbersSwitch = null;
            _isSettingsVisible = false;
            _isAuthorizationVisible = false;
            _isBound = false;
        }

        void CreateSettingsView(VisualElement root)
        {
            var existingSettingsRoot = root.Q<VisualElement>("settings-root");
            if (_settingsViewAsset == null || existingSettingsRoot != null)
            {
                _settingsRoot = existingSettingsRoot;
                _settingsContainer = root.Q<VisualElement>("settings-container") ?? _settingsRoot;
                return;
            }

            var settingsContainer = _settingsViewAsset.CloneTree();
            settingsContainer.name = "settings-container";
            settingsContainer.style.position = Position.Absolute;
            settingsContainer.style.left = 0;
            settingsContainer.style.top = 0;
            settingsContainer.style.right = 0;
            settingsContainer.style.bottom = 0;
            root.Add(settingsContainer);

            _settingsContainer = settingsContainer;
            _settingsRoot = root.Q<VisualElement>("settings-root");
        }

        void CreateAuthorizationView(VisualElement root)
        {
            var existingAuthorizationRoot = root.Q<VisualElement>("authorization-root");
            if (_authorizationViewAsset == null || existingAuthorizationRoot != null)
            {
                _authorizationRoot = existingAuthorizationRoot;
                _authorizationContainer = root.Q<VisualElement>("authorization-container") ?? _authorizationRoot;
                CacheAuthorizationPages();
                return;
            }

            var authorizationContainer = _authorizationViewAsset.CloneTree();
            authorizationContainer.name = "authorization-container";
            authorizationContainer.style.position = Position.Absolute;
            authorizationContainer.style.left = 0;
            authorizationContainer.style.top = 0;
            authorizationContainer.style.right = 0;
            authorizationContainer.style.bottom = 0;
            root.Add(authorizationContainer);

            _authorizationContainer = authorizationContainer;
            _authorizationRoot = root.Q<VisualElement>("authorization-root");
            CacheAuthorizationPages();
        }

        void CacheAuthorizationPages()
        {
            _authSignInPage = _authorizationRoot?.Q<VisualElement>("auth-sign-in-page");
            _authCreateAccountPage = _authorizationRoot?.Q<VisualElement>("auth-create-account-page");
        }

        void CacheSettingsControls(VisualElement root)
        {
            var settingsRoot = _settingsRoot ?? root;
            if (settingsRoot == null)
            {
                return;
            }

            _settingsResolutionDropdown = settingsRoot.Q<DropdownView>("settings-resolution-dropdown");
            _settingsDisplayModeDropdown = settingsRoot.Q<DropdownView>("settings-display-mode-dropdown");
            _settingsFrameRateDropdown = settingsRoot.Q<DropdownView>("settings-frame-rate-dropdown");
            _settingsOverallQualityDropdown = settingsRoot.Q<DropdownView>("settings-overall-quality-dropdown");
            _settingsTexturesDropdown = settingsRoot.Q<DropdownView>("settings-textures-dropdown");
            _settingsEffectsQualityDropdown = settingsRoot.Q<DropdownView>("settings-effects-quality-dropdown");
            _settingsLanguageDropdown = settingsRoot.Q<DropdownView>("settings-language-dropdown");
            _settingsBrightnessSlider = settingsRoot.Q<VisualElement>("settings-brightness-slider") as INotifyValueChanged<float>;
            _settingsMusicVolumeSlider = settingsRoot.Q<VisualElement>("settings-music-volume-slider") as INotifyValueChanged<float>;
            _settingsEffectsVolumeSlider = settingsRoot.Q<VisualElement>("settings-effects-volume-slider") as INotifyValueChanged<float>;
            _settingsShowDamageNumbersSwitch = settingsRoot.Q<SwitchView>("settings-show-damage-numbers-switch");
        }

        SettingsSnapshot CaptureSettingsFromControls()
        {
            return new SettingsSnapshot(
                GetDropdownValue(_settingsResolutionDropdown, DefaultSettings.Resolution),
                GetDropdownValue(_settingsDisplayModeDropdown, DefaultSettings.DisplayMode),
                GetDropdownValue(_settingsFrameRateDropdown, DefaultSettings.FrameRate),
                GetPercentValue(_settingsBrightnessSlider, DefaultSettings.Brightness),
                GetDropdownValue(_settingsOverallQualityDropdown, DefaultSettings.OverallQuality),
                GetDropdownValue(_settingsTexturesDropdown, DefaultSettings.Textures),
                GetDropdownValue(_settingsEffectsQualityDropdown, DefaultSettings.EffectsQuality),
                GetPercentValue(_settingsMusicVolumeSlider, DefaultSettings.MusicVolume),
                GetPercentValue(_settingsEffectsVolumeSlider, DefaultSettings.EffectsVolume),
                GetDropdownValue(_settingsLanguageDropdown, DefaultSettings.Language),
                _settingsShowDamageNumbersSwitch?.value ?? DefaultSettings.ShowDamageNumbers);
        }

        void ApplySettingsToControls(SettingsSnapshot settings)
        {
            SetDropdownValue(_settingsResolutionDropdown, settings.Resolution);
            SetDropdownValue(_settingsDisplayModeDropdown, settings.DisplayMode);
            SetDropdownValue(_settingsFrameRateDropdown, settings.FrameRate);
            SetPercentValue(_settingsBrightnessSlider, settings.Brightness);
            SetDropdownValue(_settingsOverallQualityDropdown, settings.OverallQuality);
            SetDropdownValue(_settingsTexturesDropdown, settings.Textures);
            SetDropdownValue(_settingsEffectsQualityDropdown, settings.EffectsQuality);
            SetPercentValue(_settingsMusicVolumeSlider, settings.MusicVolume);
            SetPercentValue(_settingsEffectsVolumeSlider, settings.EffectsVolume);
            SetDropdownValue(_settingsLanguageDropdown, settings.Language);
            _settingsShowDamageNumbersSwitch?.SetValueWithoutNotify(settings.ShowDamageNumbers);
        }

        static string GetDropdownValue(DropdownView dropdown, string fallback)
        {
            return dropdown == null || string.IsNullOrEmpty(dropdown.Value) ? fallback : dropdown.Value;
        }

        static void SetDropdownValue(DropdownView dropdown, string value)
        {
            if (dropdown != null)
            {
                dropdown.Value = value;
            }
        }

        static float GetPercentValue(INotifyValueChanged<float> control, float fallback)
        {
            return control?.value ?? fallback;
        }

        static void SetPercentValue(INotifyValueChanged<float> control, float value)
        {
            control?.SetValueWithoutNotify(value);
        }

        static void BindAuthTextFields(VisualElement root)
        {
            root.Query<TextField>(className: "auth-text-input").ForEach(textField =>
            {
                textField.isPasswordField = textField.ClassListContains("auth-password-input");
                textField.RegisterValueChangedCallback(OnAuthTextFieldChanged);
                UpdateAuthPlaceholder(textField);
            });
        }

        static void UnbindAuthTextFields(VisualElement root)
        {
            root.Query<TextField>(className: "auth-text-input").ForEach(textField =>
            {
                textField.UnregisterValueChangedCallback(OnAuthTextFieldChanged);
            });
        }

        static void SetNonInteractivePicking(VisualElement root)
        {
            foreach (var className in NonInteractiveClasses)
            {
                root.Query<VisualElement>(className: className)
                    .ForEach(element => element.pickingMode = PickingMode.Ignore);
            }
        }

        static void SyncLayeredTitleText(VisualElement root)
        {
            var title = root.Q<Label>("menu-title");
            if (title != null)
            {
                root.Query<Label>(className: "menu-title-layer")
                    .ForEach(layer => layer.text = title.text);
            }
        }

        void ClearMenuSelection()
        {
            foreach (var button in _menuButtons)
            {
                button.Selected = false;
            }
        }

        void OnMenuButtonClicked(ClickEvent evt)
        {
            if (evt.currentTarget is not GameButtonView button)
            {
                return;
            }

            ClearMenuSelection();
            HandleMenuAction(button.name);
        }

        static void OnDiscordClicked(ClickEvent evt)
        {
            Debug.Log("Menu action: Discord");
        }

        static void OnSupportClicked(ClickEvent evt)
        {
            Debug.Log("Menu action: Tech Support");
        }

        void OnFooterSettingsClicked(ClickEvent evt)
        {
            ShowSettings();
        }

        void OnSettingsBackClicked(ClickEvent evt)
        {
            ShowMenu();
        }

        void OnSettingsCancelClicked(ClickEvent evt)
        {
            ApplySettingsToControls(_appliedSettings);
        }

        void OnSettingsRestoreDefaultsClicked(ClickEvent evt)
        {
            ApplySettingsToControls(DefaultSettings);
            Debug.Log("Settings action: Restore Defaults");
        }

        void OnSettingsApplyClicked(ClickEvent evt)
        {
            _appliedSettings = CaptureSettingsFromControls();
            Debug.Log("Settings action: Apply");
        }

        static void OnAuthorizationSignInClicked(ClickEvent evt)
        {
            Debug.Log("Authorization action: Sign In");
        }

        static void OnAuthorizationCreateAccountClicked(ClickEvent evt)
        {
            Debug.Log("Authorization action: Create Account");
        }

        void OnShowCreateAccountClicked(ClickEvent evt)
        {
            ShowCreateAccountPage();
        }

        void OnShowSignInClicked(ClickEvent evt)
        {
            ShowSignInPage();
        }

        void OnAuthorizationBackClicked(ClickEvent evt)
        {
            ShowMenu();
        }

        static void OnForgotPasswordClicked(ClickEvent evt)
        {
            Debug.Log("Authorization action: Forgot Password");
        }

        static void OnAuthCheckboxClicked(ClickEvent evt)
        {
            if (evt.currentTarget is not Button checkbox)
            {
                return;
            }

            checkbox.EnableInClassList(AuthCheckboxSelectedClass, !checkbox.ClassListContains(AuthCheckboxSelectedClass));
        }

        static void OnAuthPasswordVisibilityClicked(ClickEvent evt)
        {
            if (evt.currentTarget is not Button button)
            {
                return;
            }

            var textField = button.parent?.Q<TextField>(className: "auth-password-input");
            if (textField == null)
            {
                return;
            }

            textField.isPasswordField = !textField.isPasswordField;
            textField.Focus();
        }

        static void OnAuthTextFieldChanged(ChangeEvent<string> evt)
        {
            if (evt.currentTarget is TextField textField)
            {
                UpdateAuthPlaceholder(textField);
            }
        }

        static void UpdateAuthPlaceholder(TextField textField)
        {
            var placeholder = textField.parent?.Q<Label>(className: "auth-placeholder");
            placeholder?.EnableInClassList(AuthPlaceholderHiddenClass, !string.IsNullOrEmpty(textField.value));
        }

        void HandleMenuAction(string buttonName)
        {
            switch (buttonName)
            {
                case "single-player-button":
                    Debug.Log("Menu action: Single Player");
                    break;
                case "multiplayer-button":
                    ShowAuthorization();
                    break;
                case "hero-creation-button":
                    Debug.Log("Menu action: Hero Creation");
                    break;
                case "settings-button":
                    ShowSettings();
                    break;
                case "exit-button":
                    Debug.Log("Menu action: Exit");
                    Application.Quit();
                    break;
                default:
                    Debug.Log($"Menu action: {buttonName}");
                    break;
            }
        }

        void ShowSettings()
        {
            if (_settingsRoot == null)
            {
                Debug.LogWarning("Settings view asset is not assigned.");
                return;
            }

            if (_isSettingsVisible)
            {
                return;
            }

            _isSettingsVisible = true;
            _isAuthorizationVisible = false;
            ApplySettingsToControls(_appliedSettings);
            SetScreenVisible(_menuRoot, false);
            SetScreenVisible(_authorizationRoot, false);
            SetScreenVisible(_authorizationContainer, false);
            SetScreenVisible(_settingsContainer, true);
            SetScreenVisible(_settingsRoot, true);
            _settingsRoot.Q<Button>("settings-back-button")?.Focus();
        }

        void ShowAuthorization()
        {
            if (_authorizationRoot == null)
            {
                Debug.LogWarning("Authorization view asset is not assigned.");
                return;
            }

            if (_isAuthorizationVisible)
            {
                ShowSignInPage();
                _authorizationRoot.Q<Button>("auth-sign-in-submit-button")?.Focus();
                return;
            }

            _isSettingsVisible = false;
            _isAuthorizationVisible = true;
            ShowSignInPage();
            SetScreenVisible(_menuRoot, false);
            SetScreenVisible(_settingsRoot, false);
            SetScreenVisible(_settingsContainer, false);
            SetScreenVisible(_authorizationContainer, true);
            SetScreenVisible(_authorizationRoot, true);
            _authorizationRoot.Q<Button>("auth-sign-in-submit-button")?.Focus();
        }

        void ShowSignInPage()
        {
            SetScreenVisible(_authSignInPage, true);
            SetScreenVisible(_authCreateAccountPage, false);
        }

        void ShowCreateAccountPage()
        {
            SetScreenVisible(_authSignInPage, false);
            SetScreenVisible(_authCreateAccountPage, true);
            _authorizationRoot?.Q<Button>("auth-create-account-submit-button")?.Focus();
        }

        void ShowMenu()
        {
            _isSettingsVisible = false;
            _isAuthorizationVisible = false;
            ClearMenuSelection();
            SetScreenVisible(_menuRoot, true);
            SetScreenVisible(_settingsRoot, false);
            SetScreenVisible(_settingsContainer, false);
            SetScreenVisible(_authorizationRoot, false);
            SetScreenVisible(_authorizationContainer, false);
        }

        readonly struct SettingsSnapshot
        {
            public readonly string Resolution;
            public readonly string DisplayMode;
            public readonly string FrameRate;
            public readonly float Brightness;
            public readonly string OverallQuality;
            public readonly string Textures;
            public readonly string EffectsQuality;
            public readonly float MusicVolume;
            public readonly float EffectsVolume;
            public readonly string Language;
            public readonly bool ShowDamageNumbers;

            public SettingsSnapshot(
                string resolution,
                string displayMode,
                string frameRate,
                float brightness,
                string overallQuality,
                string textures,
                string effectsQuality,
                float musicVolume,
                float effectsVolume,
                string language,
                bool showDamageNumbers)
            {
                Resolution = resolution;
                DisplayMode = displayMode;
                FrameRate = frameRate;
                Brightness = brightness;
                OverallQuality = overallQuality;
                Textures = textures;
                EffectsQuality = effectsQuality;
                MusicVolume = musicVolume;
                EffectsVolume = effectsVolume;
                Language = language;
                ShowDamageNumbers = showDamageNumbers;
            }
        }

        static void SetScreenVisible(VisualElement screen, bool visible)
        {
            if (screen == null)
            {
                return;
            }

            screen.EnableInClassList(ScreenHiddenClass, !visible);
            screen.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            screen.pickingMode = visible ? PickingMode.Position : PickingMode.Ignore;
        }
    }
}
