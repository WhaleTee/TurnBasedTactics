using UnityEngine;

namespace WhaleTee.Grid {
  public interface ToWorldPositionService {
    Vector2 GetWorldPosition(Vector3Int cellPosition);
    Vector3Int GetCellPosition(Vector2 worldPoint);
  }
}