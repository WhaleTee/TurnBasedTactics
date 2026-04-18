using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using Reflex.Attributes;
using TurnBasedTactics.Unit;
using UnityEngine;
using WhaleTee.Extensions;

namespace WhaleTee.Grid {
  public class UnitHexGridMovementService {
    [Inject]
    GroundTilemapToWorldPositionService tilemapToWorldPositionService;

    public void SetPosition(SquadUnit unit, Vector2 position) {
      var newPosition = position + unit.configuration.position.cellOffset;
      unit.state.gameObject.transform.position = newPosition;
      unit.state.position.cellPosition = tilemapToWorldPositionService.GetCellPosition(newPosition);
    }

    public async UniTask MoveUnitAlongGridPathAsync(SquadUnit unit, List<Vector3Int> path) {
      if (path.Count <= 0) return;

      unit.state.movement.isMoving = true;

      var unitTransform = unit.state.gameObject.transform;
      var unitPosition = unitTransform.position;

      foreach (var position in path) {
        var newUnitPosition = tilemapToWorldPositionService.GetWorldPosition(position) + unit.configuration.position.cellOffset;
        var stepDuration = unit.configuration.movement.stepDuration;

        await LMotion.Create(unitPosition.XY(), newUnitPosition, stepDuration)
                     .WithEase(Ease.Linear)
                     .BindToLocalPositionXY(unitTransform)
                     .ToUniTask();
      }

      unit.state.movement.isMoving = false;
      unit.state.position.cellPosition = tilemapToWorldPositionService.GetCellPosition(unitPosition);
    }
  }
}