using Managers;
using Pooling;
using UnityEngine;
using VContainer.Unity;

namespace VContainer
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameParameters gameParameters;
        [SerializeField] private GameData.GameData gameData;
        [SerializeField] private MissionData missionData;
        [SerializeField] private ShopData shopData;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameParameters);
            builder.RegisterInstance(gameData).As<GameData.GameData>();;
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
            builder.RegisterComponentInHierarchy<HapticManager>().As<IHapticManager>().WithParameter(_ => gameData);
            builder.RegisterComponentInHierarchy<IncreaseTextHandler>();
            builder.RegisterComponentInHierarchy<DifficultyManager>();
            builder.RegisterComponentInHierarchy<MissionSystem.MissionSystem>();
            builder.RegisterComponentInHierarchy<ShopSystem>();
        }
    }
}