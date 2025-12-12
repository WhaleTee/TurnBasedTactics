using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using TurnBasedTactics.Unit;
using UnityEngine;
using PrimeTween;

namespace WhaleTee.Grid {
  public class UnitHexGridMovementService {
    [Inject] GroundTilemapToWorldPositionService tilemapToWorldPositionService;

    public void SetPosition(SquadUnit unit, Vector2 position) {
      var newPosition = position + unit.configuration.position.cellOffset;
      unit.state.gameObject.transform.position = newPosition;
      unit.state.position.cellPosition = tilemapToWorldPositionService.GetCellPosition(newPosition);
    }

    public async UniTask MoveUnitAlongGridPathAsync(SquadUnit unit, List<Vector3Int> path) {
      if (path.Count <= 0) return;

      unit.state.movement.isMoving = true;

      foreach (var position in path)
        await Tween.LocalPosition(
          unit.state.gameObject.transform,
          tilemapToWorldPositionService.GetWorldPosition(position) + unit.configuration.position.cellOffset,
          unit.configuration.movement.stepDuration,
          Ease.Linear
        );

      unit.state.movement.isMoving = false;
      unit.state.position.cellPosition = tilemapToWorldPositionService.GetCellPosition(unit.state.gameObject.transform.position);
    }
  }
}