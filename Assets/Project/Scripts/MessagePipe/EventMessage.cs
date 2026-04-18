using TurnBasedTactics.Unit;
using UnityEngine;

namespace WhaleTee.MessagePipe.Message {
  public struct CellPlacedEventMessage {
    public Vector3Int cellPosition;
    public bool blockingMovement;
    public int movementCost;
  }

  public struct CharacterSelectedEventMessage {
    public GameObject gameObject;
  }

  public struct SquadSelectedEventMessage {
    public SquadSO squad;
  }

  public struct UnitDestroyedEventMessage {
    public GameObject gameObject;
  }

  public struct UnitSpawnedEventMessage {
    public GameObject gameObject;
  }
}