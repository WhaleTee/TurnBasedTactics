using System;
using Reflex.Core;
using TurnBasedTactics.Abilities.Targeting;
using TurnBasedTactics.Gameplay;
using TurnBasedTactics.UI;
using TurnBasedTactics.Unit;
using UnityEngine;
using UnityEngine.Tilemaps;
using WhaleTee.Factory;
using WhaleTee.FSM;
using WhaleTee.Grid;
using WhaleTee.Lifecycle;
using WhaleTee.MessagePipe.Message;
using WhaleTee.MessagePipe.Reflex;
using WhaleTee.Pooling;
using WhaleTee.Reactive.Input;

namespace TurnBasedTactics.DI {
  public class GameplayContainerInstaller : ContainerInstaller<GameplayContainerInstaller> {
    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    NavigationTilemapContainer navigationTilemapContainer;

    [SerializeField]
    MainMenuUI mainMenuUI;

    [SerializeField]
    SquadSO squad;
    
    protected override string ContainerName => "Gameplay Container";

    void RegisterMessagePipe(ContainerBuilder containerBuilder) {
      containerBuilder.RegisterMessagePipe();
      containerBuilder.RegisterMessageBroker<CellPlacedEventMessage>();
      containerBuilder.RegisterMessageBroker<CharacterSelectedEventMessage>();
      containerBuilder.RegisterMessageBroker<UnitSpawnedEventMessage>();
      containerBuilder.RegisterMessageBroker<UnitDestroyedEventMessage>();
      containerBuilder.RegisterMessageBroker<SquadSelectedEventMessage>();
      containerBuilder.RegisterMessageBroker<StateLifecycleChangedEvent>();
    }
    
    void RegisterServices(ContainerBuilder containerBuilder) {
      containerBuilder.AddSingleton(typeof(UserInput));
      containerBuilder.AddSingleton(typeof(StateObserveManager));
      containerBuilder.AddSingleton(typeof(GameObjectFactory), typeof(IDeactivatedGameObjectFactory), typeof(IPrefabFactory<GameObject>));
      containerBuilder.AddSingleton(typeof(GameObjectAsyncFactory), typeof(IPrefabAsyncFactory<GameObject>));
      containerBuilder.AddSingleton(typeof(ObjectPool), typeof(ObjectPool), typeof(IInitializable));
      containerBuilder.AddSingleton(typeof(UpdatableRunner), typeof(IInitializable), typeof(IDisposable));
      containerBuilder.AddSingleton(typeof(FixedUpdatableRunner), typeof(IInitializable), typeof(IDisposable));
      
      containerBuilder.AddSingleton(mainMenuUI);
      containerBuilder.AddSingleton(new GroundTilemapContainer(tilemap));
      containerBuilder.AddSingleton(navigationTilemapContainer);
      // containerBuilder.AddSingleton(squad.GetSquadData());
      containerBuilder.AddSingleton(typeof(GroundTilemapToWorldPositionService));
      containerBuilder.AddSingleton(typeof(CharacterSelector), typeof(IInitializable), typeof(IDisposable));
      containerBuilder.AddSingleton(typeof(TargetingManager), typeof(IInitializable), typeof(IDisposable));
      containerBuilder.AddSingleton(typeof(MainMenuStateUIProcessor), typeof(IInitializable), typeof(IDisposable));
      
      containerBuilder.AddSingleton(typeof(HexGridNavigationService));
      containerBuilder.AddSingleton(typeof(UnitHexGridMovementService));
      containerBuilder.AddSingleton(typeof(UnitSpawnService));
    }

    public override void InstallBindings(ContainerBuilder containerBuilder) {
      base.InstallBindings(containerBuilder);
      RegisterMessagePipe(containerBuilder);
      RegisterServices(containerBuilder);
    }
  }
}