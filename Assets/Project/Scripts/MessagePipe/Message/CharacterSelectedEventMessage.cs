using UnityEngine;

namespace WhaleTee.MessagePipe.Message {
  public struct CharacterSelectedEventMessage : IEventMessage {
    public GameObject gameObject;
  }
}