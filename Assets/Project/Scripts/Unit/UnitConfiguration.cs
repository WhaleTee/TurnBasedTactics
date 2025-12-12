using System;
using TurnBasedTactics.Abilities;
using UnityEngine;

namespace TurnBasedTactics.Unit {
  [Serializable]
  public class UnitConfiguration {
    public GameObject prefab;
    public Ability[] abilities;
    public UnitMovementConfiguration movement;
    public UnitPositionConfiguration position;
  }
}