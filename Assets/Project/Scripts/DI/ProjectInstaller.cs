using System;
using WhaleTee.Factory;
using Reflex.Core;
using UnityEngine;
using WhaleTee.FSM;
using WhaleTee.Grid;
using WhaleTee.MessagePipe.Message;
using WhaleTee.MessagePipe.Reflex;
using WhaleTee.Pooling;
using WhaleTee.Reactive.Input;
using WhaleTee.Timers;

namespace WhaleTee.TurnBasedTactics.DI {
  public class ProjectInstaller : MonoBehaviour, IInstaller {
    const string ROOT_CONTAINER_NAME = "Root Container";

    static void InstallMessagePipe(ContainerBuilder rootContainerBuilder) {
      var decorator = new MessagePipeContainerDecorator(rootContainerBuilder);
      decorator.RegisterMessageBroker<CellPlacedEventMessage>();
      decorator.RegisterMessageBroker<CharacterSelectedEventMessage>();
      decorator.RegisterMessageBroker<UnitSpawnedEventMessage>();
      decorator.RegisterMessageBroker<UnitDestroyedEventMessage>();
      decorator.RegisterMessageBroker<SquadSelectedEventMessage>();
      decorator.RegisterMessageBroker<StateLifecycleChangedEvent>();
    }

    void InstallServices(ContainerBuilder containerBuilder) {
      containerBuilder.AddSingleton(typeof(UserInput));
      containerBuilder.AddSingleton(typeof(HexGridNavigationService));
      containerBuilder.AddSingleton(typeof(UnitHexGridMovementService));
      containerBuilder.AddSingleton(typeof(UnitSpawnService));
      containerBuilder.AddSingleton(typeof(GameObjectFactory), typeof(IDeactivatedGameObjectFactory));
      containerBuilder.AddSingleton(typeof(GameObjectAsyncFactory), typeof(IPrefabAsyncFactory<GameObject>));
      containerBuilder.AddSingleton(typeof(ObjectPool), typeof(ObjectPool), typeof(IInitializable));
      containerBuilder.AddSingleton(typeof(TimerBootstrapper), typeof(IInitializable), typeof(IDisposable));
    }

    public void InstallBindings(ContainerBuilder containerBuilder) {
      containerBuilder.SetName(ROOT_CONTAINER_NAME);
      InstallMessagePipe(containerBuilder);
      InstallServices(containerBuilder);
      containerBuilder.AddSingleton(typeof(GameDriver), typeof(IInitializable));
    }
  }
}