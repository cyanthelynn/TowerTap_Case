using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameParameters gameParameters;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(gameParameters);
        builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<CameraController>();
        builder.RegisterComponentInHierarchy<BlockPoolManager>();
        builder.RegisterComponentInHierarchy<TowerStackManager>();
        builder.RegisterComponentInHierarchy<GameManager>();
        builder.RegisterComponentInHierarchy<ScoreManager>();
        builder.RegisterComponentInHierarchy<UIManager>();
        builder.RegisterComponentInHierarchy<ParticleManager>();
        builder.RegisterComponentInHierarchy<SoundManager>();
    }
}