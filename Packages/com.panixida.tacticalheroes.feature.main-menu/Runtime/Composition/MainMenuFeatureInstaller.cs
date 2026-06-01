using VContainer;
using VContainer.Unity;

namespace Panixida.TacticalHeroes.Features.MainMenu.Composition
{
    public sealed class MainMenuFeatureInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Main menu is scene-authored for now; this is its composition boundary.
        }
    }
}
