using UnityEngine;

namespace WhaleTee.MessagePipe.Message {
  public struct UnitSpawnedEventMessage : IEventMessage {
    public GameObject gameObject;
  }
}