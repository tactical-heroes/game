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

        [SerializeField] VisualTreeAsset _settingsViewAsset;

        readonly List<GameButtonView> _menuButtons = new();
        UIDocument _document;
        Coroutine _bindCoroutine;
        VisualElement _menuRoot;
        VisualElement _settingsContainer;
        VisualElement _settingsRoot;
        bool _isSettingsVisible;
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
            _menuRoot = root.Q<VisualElement>("menu-root");
            if (_menuRoot == null)
            {
                Debug.LogWarning("Main menu root was not found in UIDocument.");
                return;
            }

            CreateSettingsView(root);

            SyncLayeredTitleText(root);

            _menuButtons.Clear();
            root.Query<GameButtonView>(className: "menu-button").ForEach(_menuButtons.Add);

            foreach (var button in _menuButtons)
            {
                button.RegisterCallback<FocusInEvent>(OnMenuButtonFocusIn);
                button.RegisterCallback<ClickEvent>(OnMenuButtonClicked);
            }

            root.Q<Button>("discord-button")?.RegisterCallback<ClickEvent>(OnDiscordClicked);
            root.Q<Button>("support-button")?.RegisterCallback<ClickEvent>(OnSupportClicked);
            root.Q<Button>("footer-settings-button")?.RegisterCallback<ClickEvent>(OnFooterSettingsClicked);

            root.Q<Button>("settings-back-button")?.RegisterCallback<ClickEvent>(OnSettingsBackClicked);
            root.Q<Button>("settings-cancel-button")?.RegisterCallback<ClickEvent>(OnSettingsCancelClicked);
            root.Q<Button>("settings-restore-defaults-button")?.RegisterCallback<ClickEvent>(OnSettingsRestoreDefaultsClicked);
            root.Q<Button>("settings-apply-button")?.RegisterCallback<ClickEvent>(OnSettingsApplyClicked);

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
                button.UnregisterCallback<FocusInEvent>(OnMenuButtonFocusIn);
                button.UnregisterCallback<ClickEvent>(OnMenuButtonClicked);
            }

            root.Q<Button>("discord-button")?.UnregisterCallback<ClickEvent>(OnDiscordClicked);
            root.Q<Button>("support-button")?.UnregisterCallback<ClickEvent>(OnSupportClicked);
            root.Q<Button>("footer-settings-button")?.UnregisterCallback<ClickEvent>(OnFooterSettingsClicked);

            root.Q<Button>("settings-back-button")?.UnregisterCallback<ClickEvent>(OnSettingsBackClicked);
            root.Q<Button>("settings-cancel-button")?.UnregisterCallback<ClickEvent>(OnSettingsCancelClicked);
            root.Q<Button>("settings-restore-defaults-button")?.UnregisterCallback<ClickEvent>(OnSettingsRestoreDefaultsClicked);
            root.Q<Button>("settings-apply-button")?.UnregisterCallback<ClickEvent>(OnSettingsApplyClicked);

            _menuButtons.Clear();
            _menuRoot = null;
            _settingsContainer = null;
            _settingsRoot = null;
            _isSettingsVisible = false;
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

        static void SyncLayeredTitleText(VisualElement root)
        {
            var title = root.Q<Label>("menu-title");
            if (title != null)
            {
                root.Query<Label>(className: "menu-title-layer")
                    .ForEach(layer => layer.text = title.text);
            }
        }

        void SelectButton(GameButtonView selectedButton)
        {
            foreach (var button in _menuButtons)
            {
                button.Selected = button == selectedButton;
            }
        }

        void ClearMenuSelection()
        {
            foreach (var button in _menuButtons)
            {
                button.Selected = false;
            }
        }

        void OnMenuButtonFocusIn(FocusInEvent evt)
        {
            if (evt.currentTarget is GameButtonView button)
            {
                SelectButton(button);
            }
        }

        void OnMenuButtonClicked(ClickEvent evt)
        {
            if (evt.currentTarget is not GameButtonView button)
            {
                return;
            }

            SelectButton(button);
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
            ShowMenu();
        }

        static void OnSettingsRestoreDefaultsClicked(ClickEvent evt)
        {
            Debug.Log("Settings action: Restore Defaults");
        }

        static void OnSettingsApplyClicked(ClickEvent evt)
        {
            Debug.Log("Settings action: Apply");
        }

        void HandleMenuAction(string buttonName)
        {
            switch (buttonName)
            {
                case "single-player-button":
                    Debug.Log("Menu action: Single Player");
                    break;
                case "multiplayer-button":
                    Debug.Log("Menu action: Multiplayer");
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
            SetScreenVisible(_menuRoot, false);
            SetScreenVisible(_settingsContainer, true);
            SetScreenVisible(_settingsRoot, true);
            _settingsRoot.Q<Button>("settings-back-button")?.Focus();
        }

        void ShowMenu()
        {
            _isSettingsVisible = false;
            ClearMenuSelection();
            SetScreenVisible(_menuRoot, true);
            SetScreenVisible(_settingsRoot, false);
            SetScreenVisible(_settingsContainer, false);
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
