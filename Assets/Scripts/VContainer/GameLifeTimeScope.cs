using UnityEngine;
using VContainer;
using VContainer.Unity;
public class GameLifeTimeScope : LifetimeScope
    {
        [SerializeField] private GameParameters gameParameters;
   
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameParameters);
            builder.RegisterComponentInHierarchy<BlockPoolManager>();
            builder.RegisterComponentInHierarchy<TowerStackManager>();
        }
    }