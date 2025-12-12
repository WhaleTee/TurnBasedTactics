using System;
using UnityEngine;

namespace TurnBasedTactics.Unit {
  [Serializable]
  public class UnitPositionState {
    public Vector3Int cellPosition;
    public Vector3Int deployPosition;
  }
}