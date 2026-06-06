using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Panixida.TacticalHeroes.Foundation.Presentation.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Features.MainMenu.Presentation
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class MainMenuView : MonoBehaviour
    {
        const string ScreenHiddenClass = "screen-hidden";
        const string AuthSubtitleErrorClass = "auth-subtitle--error";
        const string AuthPasswordVisibleClass = "auth-eye-button--visible";
        const float ReferenceWidth = 1920f;
        const float ReferenceHeight = 1080f;
        const float MinimumSidePadding = 20f;
        const float MenuPanelWidth = 610f;
        const float MenuPanelHeight = 1028f;
        const float MenuPanelTop = 26f;
        const float MenuPanelRight = 28f;
        const float FooterLinksWidth = 190f;
        const float FooterLinksHeight = 58f;
        const float FooterLinksLeft = 50f;
        const float FooterLinksBottom = 36f;
        const float AuthSignInPanelTop = 26f;
        const float AuthSignInPanelHeight = 1028f;
        const float AuthCreatePanelTop = 18f;
        const float AuthCreatePanelHeight = 1044f;
        const float LobbyDesignWidth = 1920f;
        const float LobbyDesignHeight = 1080f;
        const string DefaultAuthEmail = "hero@tacticalheroes.test";
        const string DefaultAuthPassword = "Hero1234";
        const string BlockedAuthEmail = "blocked@tacticalheroes.test";
        const string BlockedAuthPassword = "Blocked1234";
        const string AuthValidationSummaryMessage = "Please fix the highlighted fields to continue";
        const string AuthSignInSubtitleMessage = "Access your tactical command center";
        const string AuthCreateSubtitleMessage = "Begin your campaign with a new account";
        const string AuthEmailValidationMessage = "Enter a valid email address.";
        const string AuthPasswordRequiredMessage = "Enter your password.";
        const string AuthIncorrectPasswordMessage = "Password is incorrect.";
        const string AuthBlockedAccountMessage = "Account is blocked.";
        const string AuthLoginValidationMessage = "Use 3-20 characters.";
        const string AuthPasswordValidationMessage = "Use 8+ chars, 1 digit, 1 uppercase and 1 lowercase.";
        const string AuthConfirmPasswordValidationMessage = "Passwords do not match.";
        const string AuthTermsValidationMessage = "Accept the Terms of Service and Privacy Policy to create an account.";
        const string AuthDuplicateEmailValidationMessage = "Account already exists.";

        [SerializeField] VisualTreeAsset _settingsViewAsset;
        [SerializeField] VisualTreeAsset _authorizationViewAsset;
        [SerializeField] VisualTreeAsset _lobbyViewAsset;

        static readonly string[] NonInteractiveClasses =
        {
            "menu-dark-overlay",
            "auth-dark-overlay",
            "auth-logo",
            "auth-title-stack",
            "auth-title-layer",
            "auth-title-separator",
            "auth-subtitle",
            "auth-field-label",
            "auth-validation-message",
            "auth-eye-icon",
            "auth-option-label",
            "auth-footer-ornament",
            "auth-version-label",
            "menu-logo",
            "menu-title-stack",
            "menu-title-layer",
            "title-separator",
            "menu-subtitle",
            "footer-ornament",
            "version-label",
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

        static readonly ResponsiveElementSpec[] SettingsResponsiveElements =
        {
            new("settings-logo", 910f, 12f, 100f, 100f),
            new("settings-title-shadow", 698f, 108f, 524f, 70f),
            new("settings-title", 695f, 102f, 530f, 70f),
            new("settings-title-separator", 713f, 157f, 494f, 50f),
            new("settings-subtitle", 713f, 195f, 494f, 30f),
            new("settings-navigation", 76f, 230f, 330f, 716f),
            new("settings-panels", 434f, 230f, 1410f, 716f),
            new("settings-footer-actions", 76f, 960f, 1768f, 90f)
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

        static readonly List<AuthAccount> AuthAccounts = new()
        {
            new("Default Commander", DefaultAuthEmail, DefaultAuthPassword, false),
            new("Blocked Commander", BlockedAuthEmail, BlockedAuthPassword, true)
        };

        readonly List<GameButtonView> _menuButtons = new();
        readonly List<VisualElement> _authPanels = new();
        readonly List<VisualElement> _authFooterLinks = new();
        readonly List<ResponsiveElement> _settingsResponsiveElements = new();
        UIDocument _document;
        Coroutine _bindCoroutine;
        VisualElement _documentRoot;
        VisualElement _menuRoot;
        VisualElement _menuPanel;
        VisualElement _menuFooterLinks;
        VisualElement _settingsContainer;
        VisualElement _settingsRoot;
        VisualElement _authorizationContainer;
        VisualElement _authorizationRoot;
        VisualElement _lobbyContainer;
        VisualElement _lobbyRoot;
        VisualElement _lobbyContent;
        VisualElement _authSignInPage;
        VisualElement _authCreateAccountPage;
        GameButtonView _authSignInSubmitButton;
        GameButtonView _authCreateAccountSubmitButton;
        InputView _authSignInEmailInput;
        InputView _authSignInPasswordInput;
        InputView _authCreateLoginInput;
        InputView _authCreateEmailInput;
        InputView _authCreatePasswordInput;
        InputView _authCreateConfirmPasswordInput;
        CheckboxView _authTermsCheckbox;
        Label _authSignInSubtitle;
        Label _authCreateSubtitle;
        Label _authTermsValidationMessage;
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
        bool _isLobbyVisible;
        bool _authSignInValidationVisible;
        bool _authSignInCredentialValidationVisible;
        bool _authCreateValidationVisible;
        string _lastFailedSignInEmail = string.Empty;
        string _lastFailedSignInPassword = string.Empty;
        SettingsReturnTarget _settingsReturnTarget;
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

            _documentRoot = root;

            CreateSettingsView(root);
            CreateAuthorizationView(root);
            CreateLobbyView(root);
            SetNonInteractivePicking(root);

            SyncLayeredTitleText(root);
            CacheResponsiveElements(root);

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
            root.Q<Button>("lobby-discord-button")?.RegisterCallback<ClickEvent>(OnDiscordClicked);
            root.Q<Button>("lobby-support-button")?.RegisterCallback<ClickEvent>(OnSupportClicked);
            root.Q<Button>("lobby-settings-button")?.RegisterCallback<ClickEvent>(OnFooterSettingsClicked);
            root.Q<Button>("lobby-chat-settings-button")?.RegisterCallback<ClickEvent>(OnFooterSettingsClicked);
            root.Q<Button>("lobby-start-matchmaking-button")?.RegisterCallback<ClickEvent>(OnLobbyStartMatchmakingClicked);
            root.Q<Button>("lobby-leaderboards-button")?.RegisterCallback<ClickEvent>(OnLobbyLeaderboardsClicked);
            root.Q<Button>("lobby-match-history-button")?.RegisterCallback<ClickEvent>(OnLobbyMatchHistoryClicked);
            root.Q<Button>("lobby-back-button")?.RegisterCallback<ClickEvent>(OnLobbyBackClicked);
            BindAuthTextFields(root);
            CacheSettingsControls(root);
            ApplySettingsToControls(_appliedSettings);
            root.RegisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);

            ShowMenu();
            ApplyResponsiveLayout();
            root.schedule.Execute(ApplyResponsiveLayout).ExecuteLater(0);
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
                root.UnregisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);

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
                root.Q<Button>("lobby-discord-button")?.UnregisterCallback<ClickEvent>(OnDiscordClicked);
                root.Q<Button>("lobby-support-button")?.UnregisterCallback<ClickEvent>(OnSupportClicked);
                root.Q<Button>("lobby-settings-button")?.UnregisterCallback<ClickEvent>(OnFooterSettingsClicked);
                root.Q<Button>("lobby-chat-settings-button")?.UnregisterCallback<ClickEvent>(OnFooterSettingsClicked);
                root.Q<Button>("lobby-start-matchmaking-button")?.UnregisterCallback<ClickEvent>(OnLobbyStartMatchmakingClicked);
                root.Q<Button>("lobby-leaderboards-button")?.UnregisterCallback<ClickEvent>(OnLobbyLeaderboardsClicked);
                root.Q<Button>("lobby-match-history-button")?.UnregisterCallback<ClickEvent>(OnLobbyMatchHistoryClicked);
                root.Q<Button>("lobby-back-button")?.UnregisterCallback<ClickEvent>(OnLobbyBackClicked);
            }

            UnbindAuthControls();

            _menuButtons.Clear();
            _authPanels.Clear();
            _authFooterLinks.Clear();
            _settingsResponsiveElements.Clear();
            _documentRoot = null;
            _menuRoot = null;
            _menuPanel = null;
            _menuFooterLinks = null;
            _settingsContainer = null;
            _settingsRoot = null;
            _authorizationContainer = null;
            _authorizationRoot = null;
            _lobbyContainer = null;
            _lobbyRoot = null;
            _lobbyContent = null;
            _authSignInPage = null;
            _authCreateAccountPage = null;
            _authSignInSubmitButton = null;
            _authCreateAccountSubmitButton = null;
            _authSignInEmailInput = null;
            _authSignInPasswordInput = null;
            _authCreateLoginInput = null;
            _authCreateEmailInput = null;
            _authCreatePasswordInput = null;
            _authCreateConfirmPasswordInput = null;
            _authTermsCheckbox = null;
            _authSignInSubtitle = null;
            _authCreateSubtitle = null;
            _authTermsValidationMessage = null;
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
            _isLobbyVisible = false;
            _authSignInValidationVisible = false;
            _authSignInCredentialValidationVisible = false;
            _authCreateValidationVisible = false;
            _lastFailedSignInEmail = string.Empty;
            _lastFailedSignInPassword = string.Empty;
            _settingsReturnTarget = SettingsReturnTarget.Menu;
            _isBound = false;
        }

        void CreateSettingsView(VisualElement root)
        {
            var existingSettingsRoot = root.Q<VisualElement>("settings-root");
            if (_settingsViewAsset == null || existingSettingsRoot != null)
            {
                _settingsRoot = existingSettingsRoot;
                _settingsContainer = root.Q<VisualElement>("settings-container") ?? _settingsRoot;
                StretchScreenContainer(_settingsContainer);
                return;
            }

            var settingsContainer = _settingsViewAsset.CloneTree();
            settingsContainer.name = "settings-container";
            StretchScreenContainer(settingsContainer);
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
                StretchScreenContainer(_authorizationContainer);
                CacheAuthorizationPages();
                return;
            }

            var authorizationContainer = _authorizationViewAsset.CloneTree();
            authorizationContainer.name = "authorization-container";
            StretchScreenContainer(authorizationContainer);
            root.Add(authorizationContainer);

            _authorizationContainer = authorizationContainer;
            _authorizationRoot = root.Q<VisualElement>("authorization-root");
            CacheAuthorizationPages();
        }

        void CreateLobbyView(VisualElement root)
        {
            var existingLobbyRoot = root.Q<VisualElement>("lobby-root");
            if (_lobbyViewAsset == null || existingLobbyRoot != null)
            {
                _lobbyRoot = existingLobbyRoot;
                _lobbyContainer = root.Q<VisualElement>("lobby-container") ?? _lobbyRoot;
                StretchScreenContainer(_lobbyContainer);
                _lobbyContent = _lobbyRoot?.Q<VisualElement>("lobby-content");
                return;
            }

            var lobbyContainer = _lobbyViewAsset.CloneTree();
            lobbyContainer.name = "lobby-container";
            StretchScreenContainer(lobbyContainer);
            root.Add(lobbyContainer);

            _lobbyContainer = lobbyContainer;
            _lobbyRoot = root.Q<VisualElement>("lobby-root");
            _lobbyContent = _lobbyRoot?.Q<VisualElement>("lobby-content");
        }

        static void StretchScreenContainer(VisualElement container)
        {
            if (container == null)
            {
                return;
            }

            container.style.position = Position.Absolute;
            container.style.left = 0;
            container.style.top = 0;
            container.style.right = 0;
            container.style.bottom = 0;
            container.style.width = Length.Percent(100);
            container.style.height = Length.Percent(100);
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

        void BindAuthTextFields(VisualElement root)
        {
            root.Query<InputView>(className: "auth-text-input").ForEach(input =>
            {
                input.Password = input.ClassListContains("auth-password-input");
            });

            var authRoot = _authorizationRoot ?? root;
            _authSignInSubmitButton = authRoot.Q<GameButtonView>("auth-sign-in-submit-button");
            _authCreateAccountSubmitButton = authRoot.Q<GameButtonView>("auth-create-account-submit-button");
            _authSignInEmailInput = authRoot.Q<InputView>("auth-sign-in-email-input");
            _authSignInPasswordInput = authRoot.Q<InputView>("auth-sign-in-password-input");
            _authCreateLoginInput = authRoot.Q<InputView>("auth-create-login-input");
            _authCreateEmailInput = authRoot.Q<InputView>("auth-create-email-input");
            _authCreatePasswordInput = authRoot.Q<InputView>("auth-create-password-input");
            _authCreateConfirmPasswordInput = authRoot.Q<InputView>("auth-create-confirm-password-input");
            _authTermsCheckbox = authRoot.Q<CheckboxView>("auth-terms-checkbox");
            _authSignInSubtitle = authRoot.Q<Label>("auth-sign-in-subtitle");
            _authCreateSubtitle = authRoot.Q<Label>("auth-create-subtitle");
            _authTermsValidationMessage = authRoot.Q<Label>("auth-terms-validation-message");

            SyncAuthPasswordVisibilityButtons(authRoot);

            _authSignInEmailInput?.RegisterValueChangedCallback(OnSignInInputChanged);
            _authSignInPasswordInput?.RegisterValueChangedCallback(OnSignInInputChanged);
            _authCreateLoginInput?.RegisterValueChangedCallback(OnCreateAccountInputChanged);
            _authCreateEmailInput?.RegisterValueChangedCallback(OnCreateAccountInputChanged);
            _authCreatePasswordInput?.RegisterValueChangedCallback(OnCreateAccountInputChanged);
            _authCreateConfirmPasswordInput?.RegisterValueChangedCallback(OnCreateAccountInputChanged);
            _authTermsCheckbox?.RegisterValueChangedCallback<bool>(OnCreateAccountTermsChanged);

            ClearSignInValidation();
            ClearCreateAccountValidation();
            SyncAuthSubmitButtons();
        }

        void UnbindAuthControls()
        {
            _authSignInEmailInput?.UnregisterValueChangedCallback(OnSignInInputChanged);
            _authSignInPasswordInput?.UnregisterValueChangedCallback(OnSignInInputChanged);
            _authCreateLoginInput?.UnregisterValueChangedCallback(OnCreateAccountInputChanged);
            _authCreateEmailInput?.UnregisterValueChangedCallback(OnCreateAccountInputChanged);
            _authCreatePasswordInput?.UnregisterValueChangedCallback(OnCreateAccountInputChanged);
            _authCreateConfirmPasswordInput?.UnregisterValueChangedCallback(OnCreateAccountInputChanged);
            _authTermsCheckbox?.UnregisterValueChangedCallback<bool>(OnCreateAccountTermsChanged);
        }

        void OnSignInInputChanged(ChangeEvent<string> evt)
        {
            SyncSignInSubmitButton();

            if (_authSignInCredentialValidationVisible && IsCurrentFailedSignInAttempt())
            {
                return;
            }

            if (_authSignInValidationVisible)
            {
                ValidateSignInFields(true);
            }
            else
            {
                ClearSignInValidation();
            }
        }

        void OnCreateAccountInputChanged(ChangeEvent<string> evt)
        {
            SyncCreateAccountSubmitButton();

            if (_authCreateValidationVisible)
            {
                ValidateCreateAccountFields(true);
            }
            else
            {
                ClearCreateAccountValidation();
            }
        }

        void OnCreateAccountTermsChanged(ChangeEvent<bool> evt)
        {
            SyncCreateAccountSubmitButton();

            if (_authCreateValidationVisible)
            {
                ValidateCreateAccountFields(true);
            }
            else
            {
                ClearCreateAccountValidation();
            }
        }

        void SyncAuthSubmitButtons()
        {
            SyncSignInSubmitButton();
            SyncCreateAccountSubmitButton();
        }

        void SyncSignInSubmitButton()
        {
            _authSignInSubmitButton?.SetEnabled(HasInputText(_authSignInEmailInput) && HasInputText(_authSignInPasswordInput));
        }

        void SyncCreateAccountSubmitButton()
        {
            _authCreateAccountSubmitButton?.SetEnabled(
                HasInputText(_authCreateLoginInput)
                && HasInputText(_authCreateEmailInput)
                && HasInputText(_authCreatePasswordInput)
                && HasInputText(_authCreateConfirmPasswordInput)
                && _authTermsCheckbox?.value == true);
        }

        bool ValidateSignInFields(bool showErrors)
        {
            var emailValid = IsValidEmail(GetTrimmedInputValue(_authSignInEmailInput));
            var passwordEntered = HasInputText(_authSignInPasswordInput);
            var isValid = emailValid && passwordEntered;

            if (showErrors)
            {
                ClearSignInCredentialValidationState();
                _authSignInValidationVisible = !isValid;
                SetSubtitleValidation(_authSignInSubtitle, AuthSignInSubtitleMessage, !isValid);
                SetInputValidation(_authSignInEmailInput, !emailValid, AuthEmailValidationMessage);
                SetInputValidation(_authSignInPasswordInput, !passwordEntered, AuthPasswordRequiredMessage);
            }

            return isValid;
        }

        bool ValidateCreateAccountFields(bool showErrors)
        {
            var login = GetTrimmedInputValue(_authCreateLoginInput);
            var email = GetTrimmedInputValue(_authCreateEmailInput);
            var password = GetInputValue(_authCreatePasswordInput);
            var confirmPassword = GetInputValue(_authCreateConfirmPasswordInput);
            var loginValid = login.Length >= 3 && login.Length <= 20;
            var emailValid = IsValidEmail(email);
            var emailAvailable = emailValid && !TryFindAuthAccount(email, out _);
            var passwordValid = IsValidPassword(password);
            var confirmPasswordValid = password.Length > 0 && password == confirmPassword;
            var termsAccepted = _authTermsCheckbox?.value == true;
            var isValid = loginValid && emailValid && emailAvailable && passwordValid && confirmPasswordValid && termsAccepted;

            if (showErrors)
            {
                _authCreateValidationVisible = !isValid;
                SetSubtitleValidation(_authCreateSubtitle, AuthCreateSubtitleMessage, !isValid);
                SetInputValidation(_authCreateLoginInput, !loginValid, AuthLoginValidationMessage);
                SetInputValidation(
                    _authCreateEmailInput,
                    !emailValid || !emailAvailable,
                    emailValid ? AuthDuplicateEmailValidationMessage : AuthEmailValidationMessage);
                SetInputValidation(_authCreatePasswordInput, !passwordValid, AuthPasswordValidationMessage);
                SetInputValidation(_authCreateConfirmPasswordInput, !confirmPasswordValid, AuthConfirmPasswordValidationMessage);
                SetCheckboxValidation(_authTermsCheckbox, _authTermsValidationMessage, !termsAccepted, AuthTermsValidationMessage);
            }

            return isValid;
        }

        void ClearSignInValidation()
        {
            _authSignInValidationVisible = false;
            ClearSignInCredentialValidationState();
            SetSubtitleValidation(_authSignInSubtitle, AuthSignInSubtitleMessage, false);
            SetInputValidation(_authSignInEmailInput, false, AuthEmailValidationMessage);
            SetInputValidation(_authSignInPasswordInput, false, AuthPasswordRequiredMessage);
        }

        void ClearCreateAccountValidation()
        {
            _authCreateValidationVisible = false;
            SetSubtitleValidation(_authCreateSubtitle, AuthCreateSubtitleMessage, false);
            SetInputValidation(_authCreateLoginInput, false, AuthLoginValidationMessage);
            SetInputValidation(_authCreateEmailInput, false, AuthEmailValidationMessage);
            SetInputValidation(_authCreatePasswordInput, false, AuthPasswordValidationMessage);
            SetInputValidation(_authCreateConfirmPasswordInput, false, AuthConfirmPasswordValidationMessage);
            SetCheckboxValidation(_authTermsCheckbox, _authTermsValidationMessage, false, AuthTermsValidationMessage);
        }

        void ShowSignInEmailError(string message)
        {
            _authSignInValidationVisible = true;
            RememberFailedSignInAttempt();
            SetSubtitleValidation(_authSignInSubtitle, AuthSignInSubtitleMessage, true);
            SetInputValidation(_authSignInEmailInput, true, message);
            SetInputValidation(_authSignInPasswordInput, false, AuthPasswordRequiredMessage);
        }

        void ShowSignInPasswordError(string message)
        {
            _authSignInValidationVisible = true;
            RememberFailedSignInAttempt();
            SetSubtitleValidation(_authSignInSubtitle, AuthSignInSubtitleMessage, true);
            SetInputValidation(_authSignInEmailInput, false, AuthEmailValidationMessage);
            SetInputValidation(_authSignInPasswordInput, true, message);
        }

        void RememberFailedSignInAttempt()
        {
            _authSignInCredentialValidationVisible = true;
            _lastFailedSignInEmail = GetTrimmedInputValue(_authSignInEmailInput);
            _lastFailedSignInPassword = GetInputValue(_authSignInPasswordInput);
        }

        bool IsCurrentFailedSignInAttempt()
        {
            return string.Equals(_lastFailedSignInEmail, GetTrimmedInputValue(_authSignInEmailInput), StringComparison.Ordinal)
                && string.Equals(_lastFailedSignInPassword, GetInputValue(_authSignInPasswordInput), StringComparison.Ordinal);
        }

        void ClearSignInCredentialValidationState()
        {
            _authSignInCredentialValidationVisible = false;
            _lastFailedSignInEmail = string.Empty;
            _lastFailedSignInPassword = string.Empty;
        }

        static void SetSubtitleValidation(Label label, string defaultMessage, bool hasError)
        {
            if (label == null)
            {
                return;
            }

            label.text = hasError ? AuthValidationSummaryMessage : defaultMessage;
            label.EnableInClassList(AuthSubtitleErrorClass, hasError);
            label.pickingMode = PickingMode.Ignore;
        }

        static void SetInputValidation(InputView input, bool hasError, string message)
        {
            if (input == null)
            {
                return;
            }

            input.ValidationMessage = message;
            input.Error = hasError;
        }

        static void SetCheckboxValidation(CheckboxView checkbox, Label label, bool hasError, string message)
        {
            if (checkbox != null)
            {
                checkbox.Error = hasError;
            }

            SetValidationMessage(label, message, hasError);
        }

        static void SetValidationMessage(Label label, string message, bool visible)
        {
            if (label == null)
            {
                return;
            }

            label.text = message;
            SetScreenVisible(label, visible);
            label.pickingMode = PickingMode.Ignore;
        }

        static bool HasInputText(InputView input)
        {
            return !string.IsNullOrWhiteSpace(input?.value);
        }

        static string GetInputValue(InputView input)
        {
            return input?.value ?? string.Empty;
        }

        static string GetTrimmedInputValue(InputView input)
        {
            return GetInputValue(input).Trim();
        }

        static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)
                || email.IndexOf('@') != email.LastIndexOf('@')
                || email.IndexOfAny(new[] { ' ', '\t', '\r', '\n' }) >= 0)
            {
                return false;
            }

            try
            {
                var address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase)
                    && address.Host.Contains(".")
                    && !address.Host.StartsWith(".", StringComparison.Ordinal)
                    && !address.Host.EndsWith(".", StringComparison.Ordinal);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return false;
            }

            var hasDigit = false;
            var hasUppercase = false;
            var hasLowercase = false;

            foreach (var character in password)
            {
                hasDigit |= char.IsDigit(character);
                hasUppercase |= char.IsUpper(character);
                hasLowercase |= char.IsLower(character);
            }

            return hasDigit && hasUppercase && hasLowercase;
        }

        static bool TryFindAuthAccount(string email, out AuthAccount account)
        {
            foreach (var existingAccount in AuthAccounts)
            {
                if (string.Equals(existingAccount.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    account = existingAccount;
                    return true;
                }
            }

            account = null;
            return false;
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

        void CacheResponsiveElements(VisualElement root)
        {
            _menuPanel = root.Q<VisualElement>(className: "menu-panel");
            _menuFooterLinks = root.Q<VisualElement>(className: "footer-links");
            _authPanels.Clear();
            root.Query<VisualElement>(className: "auth-panel").ForEach(_authPanels.Add);

            _authFooterLinks.Clear();
            root.Query<VisualElement>(className: "auth-footer-links").ForEach(_authFooterLinks.Add);

            _settingsResponsiveElements.Clear();
            foreach (var spec in SettingsResponsiveElements)
            {
                var element = root.Q<VisualElement>(className: spec.ClassName);
                if (element != null)
                {
                    _settingsResponsiveElements.Add(new ResponsiveElement(element, spec.DesignRect));
                }
            }
        }

        void OnRootGeometryChanged(GeometryChangedEvent evt)
        {
            ApplyResponsiveLayout();
        }

        void ApplyResponsiveLayout()
        {
            if (_documentRoot == null)
            {
                return;
            }

            var width = _documentRoot.resolvedStyle.width;
            var height = _documentRoot.resolvedStyle.height;
            if (!HasValidSize(width, height))
            {
                width = _documentRoot.layout.width;
                height = _documentRoot.layout.height;
            }

            if (!HasValidSize(width, height))
            {
                return;
            }

            var safeArea = GetSafeAreaInsets(width, height);
            ApplyMenuResponsiveLayout(width, height, safeArea);
            ApplyAuthorizationResponsiveLayout(width, height, safeArea);
            ApplyLobbyResponsiveLayout(width, height, safeArea);
            ApplySettingsResponsiveLayout(width, height, safeArea);
        }

        void ApplyMenuResponsiveLayout(float width, float height, SafeAreaInsets safeArea)
        {
            var scale = GetAnchoredPanelScale(width, height, safeArea);
            var panelLeft = GetRightAnchoredLeft(width, safeArea, MenuPanelWidth, MenuPanelRight, scale);
            var panelTop = GetDesignTop(height, safeArea, MenuPanelTop, scale);

            ApplyScaledElement(_menuPanel, panelLeft, panelTop, MenuPanelWidth, MenuPanelHeight, scale);
            ApplyFooterLinksLayout(_menuFooterLinks, width, height, safeArea);
        }

        void ApplyAuthorizationResponsiveLayout(float width, float height, SafeAreaInsets safeArea)
        {
            var scale = GetAnchoredPanelScale(width, height, safeArea);
            var xOffset = GetRightAnchoredLeft(width, safeArea, MenuPanelWidth, MenuPanelRight, scale);
            var yOffset = GetDesignYOffset(height, safeArea, scale);

            foreach (var panel in _authPanels)
            {
                var isCreateAccountPanel = panel.ClassListContains("auth-create-account-panel");
                var top = isCreateAccountPanel ? AuthCreatePanelTop : AuthSignInPanelTop;
                var panelHeight = isCreateAccountPanel ? AuthCreatePanelHeight : AuthSignInPanelHeight;
                ApplyScaledElement(panel, xOffset, yOffset + top * scale, MenuPanelWidth, panelHeight, scale);
            }

            foreach (var footerLinks in _authFooterLinks)
            {
                ApplyFooterLinksLayout(footerLinks, width, height, safeArea);
            }
        }

        void ApplySettingsResponsiveLayout(float width, float height, SafeAreaInsets safeArea)
        {
            var availableWidth = Mathf.Max(1f, width - safeArea.Left - safeArea.Right);
            var availableHeight = Mathf.Max(1f, height - safeArea.Top - safeArea.Bottom);
            var scale = Mathf.Min(1f, availableWidth / ReferenceWidth, availableHeight / ReferenceHeight);
            var xOffset = safeArea.Left + Mathf.Max(0f, (availableWidth - ReferenceWidth * scale) * 0.5f);
            var yOffset = safeArea.Top + Mathf.Max(0f, (availableHeight - ReferenceHeight * scale) * 0.5f);

            foreach (var item in _settingsResponsiveElements)
            {
                ApplyScaledElement(item.Element, item.DesignRect, xOffset, yOffset, scale);
            }
        }

        void ApplyLobbyResponsiveLayout(float width, float height, SafeAreaInsets safeArea)
        {
            if (_lobbyContent == null)
            {
                return;
            }

            var availableWidth = Mathf.Max(1f, width - safeArea.Left - safeArea.Right);
            var availableHeight = Mathf.Max(1f, height - safeArea.Top - safeArea.Bottom);
            var scale = Mathf.Min(1f, availableWidth / LobbyDesignWidth, availableHeight / LobbyDesignHeight);
            var xOffset = safeArea.Left + Mathf.Max(0f, (availableWidth - LobbyDesignWidth * scale) * 0.5f);
            var yOffset = safeArea.Top + Mathf.Max(0f, (availableHeight - LobbyDesignHeight * scale) * 0.5f);

            ApplyScaledElement(_lobbyContent, xOffset, yOffset, LobbyDesignWidth, LobbyDesignHeight, scale);
        }

        static void ApplyFooterLinksLayout(VisualElement footerLinks, float width, float height, SafeAreaInsets safeArea)
        {
            if (footerLinks == null)
            {
                return;
            }

            var availableWidth = Mathf.Max(1f, width - safeArea.Left - safeArea.Right);
            var availableHeight = Mathf.Max(1f, height - safeArea.Top - safeArea.Bottom);
            var scale = Mathf.Min(
                1f,
                availableWidth / (FooterLinksLeft + FooterLinksWidth + MinimumSidePadding),
                availableHeight / (FooterLinksBottom + FooterLinksHeight + MinimumSidePadding));
            var left = safeArea.Left + FooterLinksLeft * scale;
            var top = height - safeArea.Bottom - (FooterLinksBottom + FooterLinksHeight) * scale;

            ApplyScaledElement(footerLinks, left, top, FooterLinksWidth, FooterLinksHeight, scale);
        }

        static void ApplyScaledElement(VisualElement element, Rect designRect, float xOffset, float yOffset, float scale)
        {
            ApplyScaledElement(
                element,
                xOffset + designRect.x * scale,
                yOffset + designRect.y * scale,
                designRect.width,
                designRect.height,
                scale);
        }

        static void ApplyScaledElement(VisualElement element, float left, float top, float width, float height, float scale)
        {
            if (element == null)
            {
                return;
            }

            element.style.left = left;
            element.style.top = top;
            element.style.right = StyleKeyword.Auto;
            element.style.bottom = StyleKeyword.Auto;
            element.style.width = width;
            element.style.height = height;
            element.style.transformOrigin = new TransformOrigin(0, 0);
            element.style.scale = new Scale(new Vector2(scale, scale));
        }

        static float GetAnchoredPanelScale(float width, float height, SafeAreaInsets safeArea)
        {
            var availableWidth = Mathf.Max(1f, width - safeArea.Left - safeArea.Right);
            var availableHeight = Mathf.Max(1f, height - safeArea.Top - safeArea.Bottom);
            var requiredWidth = MenuPanelWidth + MenuPanelRight + MinimumSidePadding;

            return Mathf.Min(1f, availableWidth / requiredWidth, availableHeight / ReferenceHeight);
        }

        static float GetRightAnchoredLeft(float width, SafeAreaInsets safeArea, float panelWidth, float panelRight, float scale)
        {
            var rightAnchoredLeft = width - safeArea.Right - (panelRight + panelWidth) * scale;
            return Mathf.Max(safeArea.Left + MinimumSidePadding * scale, rightAnchoredLeft);
        }

        static float GetDesignTop(float height, SafeAreaInsets safeArea, float designTop, float scale)
        {
            return GetDesignYOffset(height, safeArea, scale) + designTop * scale;
        }

        static float GetDesignYOffset(float height, SafeAreaInsets safeArea, float scale)
        {
            var availableHeight = Mathf.Max(1f, height - safeArea.Top - safeArea.Bottom);
            return safeArea.Top + Mathf.Max(0f, (availableHeight - ReferenceHeight * scale) * 0.5f);
        }

        static SafeAreaInsets GetSafeAreaInsets(float panelWidth, float panelHeight)
        {
            if (Screen.width <= 0 || Screen.height <= 0)
            {
                return default;
            }

            var safeArea = Screen.safeArea;
            if (safeArea.width <= 0f || safeArea.height <= 0f)
            {
                return default;
            }

            var scaleX = panelWidth / Screen.width;
            var scaleY = panelHeight / Screen.height;
            return new SafeAreaInsets(
                Mathf.Max(0f, safeArea.xMin * scaleX),
                Mathf.Max(0f, (Screen.width - safeArea.xMax) * scaleX),
                Mathf.Max(0f, (Screen.height - safeArea.yMax) * scaleY),
                Mathf.Max(0f, safeArea.yMin * scaleY));
        }

        static bool HasValidSize(float width, float height)
        {
            return width > 0f && height > 0f && !float.IsNaN(width) && !float.IsNaN(height);
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
            _settingsReturnTarget = _isLobbyVisible ? SettingsReturnTarget.Lobby : SettingsReturnTarget.Menu;
            ShowSettings();
        }

        void OnSettingsBackClicked(ClickEvent evt)
        {
            if (_settingsReturnTarget == SettingsReturnTarget.Lobby)
            {
                ShowLobby();
                return;
            }

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

        void OnAuthorizationSignInClicked(ClickEvent evt)
        {
            if (!ValidateSignInFields(true))
            {
                return;
            }

            var email = GetTrimmedInputValue(_authSignInEmailInput);
            var password = GetInputValue(_authSignInPasswordInput);

            if (TryFindAuthAccount(email, out var account) && account.IsBlocked)
            {
                ShowSignInEmailError(AuthBlockedAccountMessage);
                return;
            }

            if (account != null && string.Equals(account.Password, password, StringComparison.Ordinal))
            {
                ClearSignInValidation();
                Debug.Log($"Authorization success: signed in as {account.Login}");
                ShowLobby();
                return;
            }

            ShowSignInPasswordError(AuthIncorrectPasswordMessage);
        }

        void OnAuthorizationCreateAccountClicked(ClickEvent evt)
        {
            if (!ValidateCreateAccountFields(true))
            {
                return;
            }

            var account = new AuthAccount(
                GetTrimmedInputValue(_authCreateLoginInput),
                GetTrimmedInputValue(_authCreateEmailInput),
                GetInputValue(_authCreatePasswordInput),
                false);

            AuthAccounts.Add(account);
            ClearCreateAccountValidation();
            Debug.Log($"Authorization success: account created for {account.Email}");
            ShowLobby();
        }

        void OnShowCreateAccountClicked(ClickEvent evt)
        {
            ClearSignInValidation();
            SyncAuthSubmitButtons();
            ShowCreateAccountPage();
        }

        void OnShowSignInClicked(ClickEvent evt)
        {
            ClearCreateAccountValidation();
            SyncAuthSubmitButtons();
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

        static void OnLobbyStartMatchmakingClicked(ClickEvent evt)
        {
            Debug.Log("Lobby action: Start Matchmaking");
        }

        static void OnLobbyLeaderboardsClicked(ClickEvent evt)
        {
            Debug.Log("Lobby action: Leaderboards");
        }

        static void OnLobbyMatchHistoryClicked(ClickEvent evt)
        {
            Debug.Log("Lobby action: Match History");
        }

        void OnLobbyBackClicked(ClickEvent evt)
        {
            ShowMenu();
        }

        static void OnAuthPasswordVisibilityClicked(ClickEvent evt)
        {
            if (evt.currentTarget is not Button button)
            {
                return;
            }

            var input = button.parent?.Q<InputView>(className: "auth-password-input");
            if (input == null)
            {
                return;
            }

            input.Password = !input.Password;
            SyncAuthPasswordVisibilityButton(button);
            input.Focus();
        }

        static void SyncAuthPasswordVisibilityButtons(VisualElement root)
        {
            if (root == null)
            {
                return;
            }

            root.Query<Button>(className: "auth-eye-button").ForEach(SyncAuthPasswordVisibilityButton);
        }

        static void SyncAuthPasswordVisibilityButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            var input = button.parent?.Q<InputView>(className: "auth-password-input");
            var passwordVisible = input != null && !input.Password;
            button.EnableInClassList(AuthPasswordVisibleClass, passwordVisible);
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
                    _settingsReturnTarget = SettingsReturnTarget.Menu;
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
            _isLobbyVisible = false;
            ApplySettingsToControls(_appliedSettings);
            SetScreenVisible(_menuRoot, false);
            SetScreenVisible(_authorizationRoot, false);
            SetScreenVisible(_authorizationContainer, false);
            SetScreenVisible(_lobbyRoot, false);
            SetScreenVisible(_lobbyContainer, false);
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

            ClearSignInValidation();
            ClearCreateAccountValidation();
            SyncAuthSubmitButtons();

            if (_isAuthorizationVisible)
            {
                ShowSignInPage();
                _authorizationRoot.Q<Button>("auth-sign-in-submit-button")?.Focus();
                return;
            }

            _isSettingsVisible = false;
            _isAuthorizationVisible = true;
            _isLobbyVisible = false;
            ShowSignInPage();
            SetScreenVisible(_menuRoot, false);
            SetScreenVisible(_settingsRoot, false);
            SetScreenVisible(_settingsContainer, false);
            SetScreenVisible(_lobbyRoot, false);
            SetScreenVisible(_lobbyContainer, false);
            SetScreenVisible(_authorizationContainer, true);
            SetScreenVisible(_authorizationRoot, true);
            _authorizationRoot.Q<Button>("auth-sign-in-submit-button")?.Focus();
        }

        void ShowLobby()
        {
            if (_lobbyRoot == null)
            {
                Debug.LogWarning("Lobby view asset is not assigned.");
                return;
            }

            _isSettingsVisible = false;
            _isAuthorizationVisible = false;
            _isLobbyVisible = true;
            ClearMenuSelection();
            ClearSignInValidation();
            ClearCreateAccountValidation();
            SyncAuthSubmitButtons();
            SetScreenVisible(_menuRoot, false);
            SetScreenVisible(_settingsRoot, false);
            SetScreenVisible(_settingsContainer, false);
            SetScreenVisible(_authorizationRoot, false);
            SetScreenVisible(_authorizationContainer, false);
            SetScreenVisible(_lobbyContainer, true);
            SetScreenVisible(_lobbyRoot, true);
            _lobbyRoot.Q<Button>("lobby-start-matchmaking-button")?.Focus();
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
            _isLobbyVisible = false;
            _settingsReturnTarget = SettingsReturnTarget.Menu;
            ClearMenuSelection();
            ClearSignInValidation();
            ClearCreateAccountValidation();
            SyncAuthSubmitButtons();
            SetScreenVisible(_menuRoot, true);
            SetScreenVisible(_settingsRoot, false);
            SetScreenVisible(_settingsContainer, false);
            SetScreenVisible(_authorizationRoot, false);
            SetScreenVisible(_authorizationContainer, false);
            SetScreenVisible(_lobbyRoot, false);
            SetScreenVisible(_lobbyContainer, false);
        }

        sealed class AuthAccount
        {
            public readonly string Login;
            public readonly string Email;
            public readonly string Password;
            public readonly bool IsBlocked;

            public AuthAccount(string login, string email, string password, bool isBlocked)
            {
                Login = login;
                Email = email;
                Password = password;
                IsBlocked = isBlocked;
            }
        }

        enum SettingsReturnTarget
        {
            Menu,
            Lobby
        }

        readonly struct ResponsiveElementSpec
        {
            public readonly string ClassName;
            public readonly Rect DesignRect;

            public ResponsiveElementSpec(string className, float left, float top, float width, float height)
            {
                ClassName = className;
                DesignRect = new Rect(left, top, width, height);
            }
        }

        readonly struct ResponsiveElement
        {
            public readonly VisualElement Element;
            public readonly Rect DesignRect;

            public ResponsiveElement(VisualElement element, Rect designRect)
            {
                Element = element;
                DesignRect = designRect;
            }
        }

        readonly struct SafeAreaInsets
        {
            public readonly float Left;
            public readonly float Right;
            public readonly float Top;
            public readonly float Bottom;

            public SafeAreaInsets(float left, float right, float top, float bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
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
