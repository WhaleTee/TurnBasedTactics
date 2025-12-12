using UnityEngine;

namespace WhaleTee.MessagePipe.Message {
  public struct CellPlacedEventMessage : IEventMessage {
    public Vector3Int cellPosition;
    public bool blockingMovement;
    public int movementCost;
  }
}