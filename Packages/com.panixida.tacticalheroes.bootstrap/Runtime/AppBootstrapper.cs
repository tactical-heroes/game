using VContainer.Unity;

namespace Panixida.TacticalHeroes.Bootstrap
{
    public sealed class AppBootstrapper : IStartable
    {
        readonly IStartupSceneLoader _sceneLoader;

        public AppBootstrapper(IStartupSceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Start()
        {
            _sceneLoader.LoadScene(ScenePaths.MainMenu);
        }
    }
}
