using Reflex.Core;
using TurnBasedTactics.Abilities.Targeting;
using TurnBasedTactics.Gameplay;
using TurnBasedTactics.Unit;
using UnityEngine;
using UnityEngine.Tilemaps;
using WhaleTee.Grid;

namespace WhaleTee.TurnBasedTactics.DI {
  public class GameplaySceneInstaller : MonoBehaviour, IInstaller {
    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    NavigationTilemapContainer navigationTilemapContainer;

    [SerializeField]
    MainMenuUIContainer mainMenuUI;

    [SerializeField]
    SquadSO squad;

    Container container;

    void Awake() {
      Initialize(container);
    }

    void Initialize(Container container) {
      foreach (var initializable in container.All<IInitializable>()) initializable.Initialize();
    }

    void OnContainerBuilt(Container container) {
      this.container = container;
    }

    void InstallServices(ContainerBuilder containerBuilder) {
      containerBuilder.AddSingleton(mainMenuUI, typeof(MainMenuUIContainer), typeof(IInitializable));
      containerBuilder.AddSingleton(new GroundTilemapContainer(tilemap));
      containerBuilder.AddSingleton(navigationTilemapContainer);
      containerBuilder.AddSingleton(squad.GetSquadData());
      containerBuilder.AddSingleton(typeof(GroundTilemapToWorldPositionService));
      containerBuilder.AddSingleton(typeof(CharacterSelector), typeof(IInitializable));
      containerBuilder.AddSingleton(typeof(TargetingManager), typeof(TargetingManager), typeof(IInitializable));
    }

    public void InstallBindings(ContainerBuilder containerBuilder) {
      InstallServices(containerBuilder);
      containerBuilder.OnContainerBuilt += OnContainerBuilt;
    }
  }
}