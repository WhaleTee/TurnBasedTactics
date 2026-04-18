using System;
using System.Threading;
using MessagePipe;
using R3;

namespace WhaleTee.FSM {
  public sealed class StateObserveManager {
    readonly ISubscriber<StateLifecycleChangedEvent> stateLifecycleChangedEventSubscriber;

    public StateObserveManager(ISubscriber<StateLifecycleChangedEvent> stateLifecycleChangedEventSubscriber) {
      this.stateLifecycleChangedEventSubscriber = stateLifecycleChangedEventSubscriber;
    }

    static bool IsValidStateAndLifecycle(StateLifecycleChangedEvent ctx, Type state, StateLifecycle lifecycle) {
      return ctx.state == state && ctx.lifecycleState == (byte)lifecycle;
    }

    public void RegisterStateEnterObserver<T>(IStateEnterObserver<T> observer, CancellationToken token = default) where T : State {
      stateLifecycleChangedEventSubscriber
      .Subscribe(_ => observer.OnEnter(), ctx => IsValidStateAndLifecycle(ctx, observer.GetObservableState(), StateLifecycle.Enter))
      .RegisterTo(token);
    }

    public void RegisterStateUpdateObserver<T>(IStateUpdateObserver<T> observer, CancellationToken token = default) where T : State {
      stateLifecycleChangedEventSubscriber
      .Subscribe(_ => observer.OnUpdate(), ctx => IsValidStateAndLifecycle(ctx, observer.GetObservableState(), StateLifecycle.Update))
      .RegisterTo(token);
    }

    public void RegisterStateExitObserver<T>(IStateExitObserver<T> observer, CancellationToken token = default) where T : State {
      stateLifecycleChangedEventSubscriber
      .Subscribe(_ => observer.OnExit(), ctx => IsValidStateAndLifecycle(ctx, observer.GetObservableState(), StateLifecycle.Exit))
      .RegisterTo(token);
    }
  }
}