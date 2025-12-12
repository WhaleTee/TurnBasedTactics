using UnityEngine;

namespace WhaleTee.MessagePipe.Message {
  public struct UnitDestroyedEventMessage : IEventMessage {
    public GameObject gameObject;
  }
}