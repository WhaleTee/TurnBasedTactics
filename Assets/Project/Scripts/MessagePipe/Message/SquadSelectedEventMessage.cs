using TurnBasedTactics.Unit;

namespace WhaleTee.MessagePipe.Message {
  public struct SquadSelectedEventMessage : IEventMessage {
    public SquadSO squad;
  }
}