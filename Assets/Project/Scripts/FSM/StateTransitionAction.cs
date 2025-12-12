using System;

namespace WhaleTee.FSM {
  public sealed class StateTransitionAction {
    readonly Action<Type> action;

    public StateTransitionAction(Action<Type> action) {
      this.action = action;
    }

    public void Invoke(Type state) {
      action.Invoke(state);
    }
  }
}