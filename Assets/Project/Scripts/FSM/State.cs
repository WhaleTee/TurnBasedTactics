using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WhaleTee.FSM {
  public abstract class State {
    bool firstUpdate = true;
    CancellationTokenSource cts;
    CancellationToken token;
    public StateTransitionAction StateTransitionAction { private get; set; }

    void CreateToken() {
      cts?.Cancel();
      cts = new CancellationTokenSource();
      token = cts.Token;
    }

    bool CheckTransition() {
      var transition = GetTransition();
      if (transition != null) StateTransitionAction.Invoke(transition);
      return transition != null;
    }

    protected virtual Type GetTransition() {
      return null;
    }

    protected virtual void OnEnter() { }

    protected virtual void OnFirstUpdate() { }

    protected virtual void OnUpdate() { }

    protected virtual void OnFixedUpdate() { }

    protected virtual void OnExit() { }

    public void Enter() {
      firstUpdate = true;
      OnEnter();
    }

    public void Update() {
      if (CheckTransition()) return;

      if (firstUpdate) {
        firstUpdate = false;
        OnFirstUpdate();
      } else OnUpdate();
    }

    public void FixedUpdate() {
      OnFixedUpdate();
    }

    public void Exit() {
      OnExit();
    }
  }

  [Flags]
  public enum StateLifecycle {
    Enter = 0b01, Update = 0b10, FixedUpdate = 0b11, Exit = 0b100
  }
}