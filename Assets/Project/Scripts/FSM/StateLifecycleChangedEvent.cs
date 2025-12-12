using System;
using WhaleTee.MessagePipe.Message;

namespace WhaleTee.FSM {
  public struct StateLifecycleChangedEvent : IEventMessage {
    public readonly Type state;
    public readonly byte lifecycleState;

    public StateLifecycleChangedEvent(Type state, byte lifecycleState) {
      this.state = state;
      this.lifecycleState = lifecycleState;
    }
  }
}