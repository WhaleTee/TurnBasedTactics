using System;
using TurnBasedTactics.Abilities;
using UnityEngine;

namespace TurnBasedTactics.Unit {
  [Serializable]
  public class UnitState {
    public GameObject gameObject;
    public Ability[] abilities;
    public UnitMovementState movement;
    public UnitPositionState position;
  }
}