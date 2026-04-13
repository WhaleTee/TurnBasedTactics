using Reflex.Attributes;
using UnityEngine;

namespace WhaleTee.Grid {
  public class GroundTilemapToWorldPositionService : ToWorldPositionService {
    [Inject]
    GroundTilemapContainer tilemapContainer;

    public Vector2 GetWorldPosition(Vector3Int cellPosition) {
      return tilemapContainer.tilemap.CellToWorld(cellPosition);
    }

    public Vector3Int GetCellPosition(Vector2 worldPoint) {
      return tilemapContainer.tilemap.WorldToCell(worldPoint);
    }
  }
}