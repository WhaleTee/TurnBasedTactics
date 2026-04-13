using TurnBasedTactics.Abilities;
using UnityEngine;

namespace TurnBasedTactics.Unit {
  public class UnitBehaviour : MonoBehaviour, IEffectTarget {
    public SquadUnit SquadUnit { get; set; }

    public Vector3 Position => transform.position;
    public Vector3Int CellPosition => SquadUnit.state.position.cellPosition;
  }
}