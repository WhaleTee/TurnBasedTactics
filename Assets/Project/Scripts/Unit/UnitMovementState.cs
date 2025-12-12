using System;

namespace TurnBasedTactics.Unit {
  [Serializable]
  public class UnitMovementState {
    public int availableSteps;
    public bool canMove;
    public bool isMoving;
  }
}