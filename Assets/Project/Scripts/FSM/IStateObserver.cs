using System;
using System.Threading;
using MessagePipe;
using R3;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhaleTee.FSM;

namespace WhaleTee.FSM {
  public interface IStateObserver<T> : IInitializable, IDisposable where T : State {
    protected CancellationTokenSource StateObserverTokenSource { get; }

    private bool IsValidStateAndLifecycle(StateLifecycleChangedEvent ctx, StateLifecycle lifecycle) {
      return ctx.state == typeof(T) && ctx.lifecycleState == (byte)lifecycle;
    }

    protected CancellationToken GetToken() {
      return StateObserverTokenSource.Token;
    }

    protected void OnEnter() { }
    protected void OnUpdate() { }
    protected void OnExit() { }

    void IInitializable.Initialize() {
      var subscriber = SceneManager.GetActiveScene().GetSceneContainer().Resolve<ISubscriber<StateLifecycleChangedEvent>>();
      if (subscriber == null) return;

      subscriber.Subscribe(_ => OnEnter(), ctx => IsValidStateAndLifecycle(ctx, StateLifecycle.Enter)).RegisterTo(GetToken());
      subscriber.Subscribe(_ => OnUpdate(), ctx => IsValidStateAndLifecycle(ctx, StateLifecycle.Update)).RegisterTo(GetToken());
      subscriber.Subscribe(_ => OnExit(), ctx => IsValidStateAndLifecycle(ctx, StateLifecycle.Exit)).RegisterTo(GetToken());
    }

    void IDisposable.Dispose() {
      StateObserverTokenSource?.Cancel();
      StateObserverTokenSource?.Dispose();
    }
  }
}