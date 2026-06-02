using System;
using System.Linq;
using UnityEngine;

namespace Panixida.TacticalHeroes.Bootstrap.Input
{
    static class RuntimeUiEventSystemBootstrapper
    {
        const string EventSystemTypeName = "UnityEngine.EventSystems.EventSystem";
        const string InputSystemUiInputModuleTypeName = "UnityEngine.InputSystem.UI.InputSystemUIInputModule";
        const string StandaloneInputModuleTypeName = "UnityEngine.EventSystems.StandaloneInputModule";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EnsureEventSystem()
        {
            var eventSystemType = FindType(EventSystemTypeName, "UnityEngine.UI");
            if (eventSystemType == null || EventSystemExists(eventSystemType))
            {
                return;
            }

            var inputModuleType =
                FindType(InputSystemUiInputModuleTypeName, "Unity.InputSystem") ??
                FindType(StandaloneInputModuleTypeName, "UnityEngine.UI");

            if (inputModuleType == null)
            {
                Debug.LogWarning("Runtime UI input module type was not found. UI Toolkit controls will not receive pointer input.");
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            UnityEngine.Object.DontDestroyOnLoad(eventSystem);
            eventSystem.AddComponent(eventSystemType);
            eventSystem.AddComponent(inputModuleType);
        }

        static Type FindType(string typeName, string assemblyName)
        {
            return Type.GetType($"{typeName}, {assemblyName}") ??
                   AppDomain.CurrentDomain.GetAssemblies()
                       .Select(assembly => assembly.GetType(typeName))
                       .FirstOrDefault(type => type != null);
        }

        static bool EventSystemExists(Type eventSystemType)
        {
#pragma warning disable CS0618
            return UnityEngine.Object.FindObjectOfType(eventSystemType) != null;
#pragma warning restore CS0618
        }
    }
}
