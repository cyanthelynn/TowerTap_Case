using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private GameData gameData;
    [SerializeField] private MissionData missionData;
    [SerializeField] private ShopData shopData;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(gameParameters);
        builder.RegisterInstance(gameData);
        builder.RegisterInstance(missionData);
        builder.RegisterInstance(shopData);
        builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<SaveManager>();
        builder.RegisterComponentInHierarchy<CameraController>();
        builder.RegisterComponentInHierarchy<BlockPoolManager>();
        builder.RegisterComponentInHierarchy<TowerStackManager>();
        builder.RegisterComponentInHierarchy<GameManager>();
        builder.RegisterComponentInHierarchy<ScoreManager>();
        builder.RegisterComponentInHierarchy<UIManager>();
        builder.RegisterComponentInHierarchy<ParticleManager>();
        builder.RegisterComponentInHierarchy<SoundManager>();
        builder.RegisterComponentInHierarchy<IncreaseTextHandler>();
        builder.RegisterComponentInHierarchy<DifficultyManager>();
        builder.RegisterComponentInHierarchy<MissionSystem>();
        builder.RegisterComponentInHierarchy<ShopSystem>();
    }
}