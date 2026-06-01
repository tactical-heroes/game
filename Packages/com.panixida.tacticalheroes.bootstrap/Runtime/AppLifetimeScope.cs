using Panixida.TacticalHeroes.Features.MainMenu.Composition;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Panixida.TacticalHeroes.Bootstrap
{
    public sealed class AppLifetimeScope : LifetimeScope
    {
        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            new MainMenuFeatureInstaller().Install(builder);

            builder.Register<IStartupSceneLoader, UnityStartupSceneLoader>(Lifetime.Singleton);
            builder.RegisterEntryPoint<AppBootstrapper>(Lifetime.Singleton);
        }
    }
}
