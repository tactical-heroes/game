using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TacticalHeroes.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class MenuController : MonoBehaviour
    {
        const string SelectedClass = "selected";

        readonly List<Button> menuButtons = new();

        void OnEnable()
        {
            var document = GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            SyncLayeredText(root);

            menuButtons.Clear();
            root.Query<Button>(className: "menu-button").ForEach(menuButtons.Add);

            foreach (var button in menuButtons)
            {
                button.RegisterCallback<PointerEnterEvent>(_ => SelectButton(button));
                button.RegisterCallback<FocusInEvent>(_ => SelectButton(button));
                button.clicked += () => HandleMenuAction(button.name);
            }

            root.Q<Button>("discord-button")?.RegisterCallback<ClickEvent>(_ => Debug.Log("Menu action: Discord"));
            root.Q<Button>("support-button")?.RegisterCallback<ClickEvent>(_ => Debug.Log("Menu action: Tech Support"));
            root.Q<Button>("footer-settings-button")?.RegisterCallback<ClickEvent>(_ => Debug.Log("Menu action: Footer Settings"));

            var selected = root.Q<Button>(className: SelectedClass) ?? root.Q<Button>("single-player-button");
            if (selected != null)
            {
                SelectButton(selected);
                selected.Focus();
            }
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
            foreach (var button in menuButtons)
            {
                button.EnableInClassList(SelectedClass, button == selectedButton);
            }
        }

        static void HandleMenuAction(string buttonName)
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
                    Debug.Log("Menu action: Settings");
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
    }
}
