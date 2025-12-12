using UnityEngine;

namespace TurnBasedTactics.Abilities {
  public interface IEffectTarget {
    Vector3 Position { get; }
    Vector3Int CellPosition { get; }
  }
}