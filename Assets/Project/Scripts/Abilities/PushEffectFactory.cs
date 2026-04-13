using System;
using Reflex.Attributes;
using WhaleTee.Grid;
using WhaleTee.Reactive.Input;

namespace TurnBasedTactics.Abilities.Factory {
  [Serializable]
  public class PushEffectFactory : IEffectFactory {
    [Inject]
    UserInput userInput;

    [Inject]
    HexGridNavigationService navigationService;

    [Inject]
    GroundTilemapToWorldPositionService tilemapToWorldService;

    [Inject]
    UnitHexGridMovementService unitMovementService;

    public int pushCells;

    public IEffect<IEffectTarget> Create() {
      return new PushTargetEffect {
        userInput = userInput,
        navigationService = navigationService,
        tilemapToWorldService = tilemapToWorldService,
        unitMovementService = unitMovementService,
        pushForce = pushCells
      };
    }
  }
}