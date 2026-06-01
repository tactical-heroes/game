using UnityEngine.SceneManagement;

namespace Panixida.TacticalHeroes.Bootstrap
{
    public interface IStartupSceneLoader
    {
        void LoadScene(string scenePath);
    }

    public sealed class UnityStartupSceneLoader : IStartupSceneLoader
    {
        public void LoadScene(string scenePath)
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.path == scenePath)
            {
                return;
            }

            SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
        }
    }

    internal static class ScenePaths
    {
        public const string MainMenu = "Assets/Scenes/10_MainMenu.unity";
    }
}
