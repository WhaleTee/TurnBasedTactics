using System;
using WhaleTee.FSM;

public sealed class MissionSelectionState : State {
  protected override Type GetTransition() {
    return typeof(DeployState);
  }
}