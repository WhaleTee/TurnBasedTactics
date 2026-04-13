using System;
using TurnBasedTactics.Unit;
using UnityEngine;
using WhaleTee.Extensions;
using WhaleTee.Grid;
using WhaleTee.Reactive.Input;

namespace TurnBasedTactics.Abilities {
  [Serializable]
  public struct PushTargetEffect : IEffect<IEffectTarget> {
    public UserInput userInput;
    public HexGridNavigationService navigationService;
    public GroundTilemapToWorldPositionService tilemapToWorldService;
    public UnitHexGridMovementService unitMovementService;
    public int pushForce;

    public event Action<IEffect<IEffectTarget>> onCompleted;

    public void Apply(Ability ability, IEffectTarget effectTarget) {
      var target = effectTarget as UnitBehaviour;
      var caster = ability.owner.GetComponent<UnitBehaviour>();
      if (target == null || caster == null) return;

      var casterCellPosition = caster.SquadUnit.state.position.cellPosition;
      var targetCellPosition = target.SquadUnit.state.position.cellPosition;

      if (casterCellPosition - targetCellPosition == Vector3Int.zero) return;

      var pushPosition = GetPushPosition(casterCellPosition, targetCellPosition, pushForce);

      unitMovementService.SetPosition(target.SquadUnit, tilemapToWorldService.GetWorldPosition(pushPosition));
      onCompleted?.Invoke(this);
    }

    Vector3Int GetPushPosition(Vector3Int casterCellPosition, Vector3Int targetCellPosition, int force) {
      var pointerPosition = userInput.GetPointerPositionWorld().AsVector2();
      var caster = tilemapToWorldService.GetWorldPosition(casterCellPosition);
      var target = tilemapToWorldService.GetWorldPosition(targetCellPosition);
      var direction = (target - caster).normalized;
      var distanceRatio = Vector3.Distance(target, caster) / navigationService.Distance(casterCellPosition, targetCellPosition);
      var pointerDelta = (pointerPosition - target) / 2;
      var worldPoint = target + direction * (distanceRatio * force) + pointerDelta;
      return tilemapToWorldService.GetCellPosition(worldPoint);
    }

    public void Cancel() {
      // noop
    }
  }
}