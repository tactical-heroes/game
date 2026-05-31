using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Panixida.TacticalHeroes.Features.MainMenu.Presentation
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class MainMenuView : MonoBehaviour
    {
        const string SelectedClass = "selected";
        const string ScreenHiddenClass = "screen-hidden";

        [SerializeField] VisualTreeAsset _settingsViewAsset;

        readonly List<Button> _menuButtons = new();
        VisualElement _menuRoot;
        VisualElement _settingsRoot;
        bool _isSettingsVisible;

        void OnEnable()
        {
            var document = GetComponent<UIDocument>();
            var root = document.rootVisualElement;
            _menuRoot = root.Q<VisualElement>("menu-root");

            CreateSettingsView(root);

            SyncLayeredText(root);

            _menuButtons.Clear();
            root.Query<Button>(className: "menu-button").ForEach(_menuButtons.Add);

            foreach (var button in _menuButtons)
            {
                button.RegisterCallback<PointerEnterEvent>(_ => SelectButton(button));
                button.RegisterCallback<FocusInEvent>(_ => SelectButton(button));
                button.clicked += () => HandleMenuAction(button.name);
            }

            root.Q<Button>("discord-button")?.RegisterCallback<ClickEvent>(_ => Debug.Log("Menu action: Discord"));
            root.Q<Button>("support-button")?.RegisterCallback<ClickEvent>(_ => Debug.Log("Menu action: Tech Support"));
            root.Q<Button>("footer-settings-button")?.RegisterCallback<ClickEvent>(_ => ShowSettings());

            root.Q<Button>("settings-back-button")?.RegisterCallback<ClickEvent>(_ => ShowMenu());
            root.Q<Button>("settings-cancel-button")?.RegisterCallback<ClickEvent>(_ => ShowMenu());
            root.Q<Button>("settings-restore-defaults-button")?.RegisterCallback<ClickEvent>(_ => Debug.Log("Settings action: Restore Defaults"));
            root.Q<Button>("settings-apply-button")?.RegisterCallback<ClickEvent>(_ => Debug.Log("Settings action: Apply"));

            var selected = root.Q<Button>(className: SelectedClass) ?? root.Q<Button>("single-player-button");
            if (selected != null)
            {
                SelectButton(selected);
                selected.Focus();
            }

            ShowMenu();
        }

        void CreateSettingsView(VisualElement root)
        {
            if (_settingsViewAsset == null || root.Q<VisualElement>("settings-root") != null)
            {
                _settingsRoot = root.Q<VisualElement>("settings-root");
                return;
            }

            var settingsContainer = _settingsViewAsset.CloneTree();
            settingsContainer.style.position = Position.Absolute;
            settingsContainer.style.left = 0;
            settingsContainer.style.top = 0;
            settingsContainer.style.right = 0;
            settingsContainer.style.bottom = 0;
            root.Add(settingsContainer);

            _settingsRoot = root.Q<VisualElement>("settings-root");
        }

        static void SyncLayeredText(VisualElement root)
        {
            var title = root.Q<Label>("menu-title");
            if (title != null)
            {
                root.Query<Label>(className: "menu-title-layer")
                    .ForEach(layer => layer.text = title.text);
            }

            root.Query<VisualElement>(className: "menu-button-label-stack").ForEach(stack =>
            {
                var source = stack.Q<Label>(className: "menu-button-label-source");
                if (source == null)
                {
                    return;
                }

                stack.Query<Label>(className: "menu-button-label-layer")
                    .ForEach(layer => layer.text = source.text);
            });
        }

        void SelectButton(Button selectedButton)
        {
            foreach (var button in _menuButtons)
            {
                button.EnableInClassList(SelectedClass, button == selectedButton);
            }
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
            SetScreenVisible(_settingsRoot, true);
            _settingsRoot.Q<Button>("settings-back-button")?.Focus();
        }

        void ShowMenu()
        {
            _isSettingsVisible = false;
            SetScreenVisible(_menuRoot, true);
            SetScreenVisible(_settingsRoot, false);
        }

        static void SetScreenVisible(VisualElement screen, bool visible)
        {
            if (screen == null)
            {
                return;
            }

            screen.EnableInClassList(ScreenHiddenClass, !visible);
            screen.pickingMode = visible ? PickingMode.Position : PickingMode.Ignore;
        }
    }
}
