using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WhaleTee.FSM;

namespace WhaleTee.Timers {
  public class TimerBootstrapper : IStateObserver<GameplayState> {
    CancellationTokenSource cts;
    CancellationToken token;

    CancellationTokenSource IStateObserver<GameplayState>.StateObserverTokenSource { get; } = new();

    async UniTaskVoid RunTimers() {
      cts?.Cancel();
      cts = new CancellationTokenSource();
      token = cts.Token;

      while (!token.IsCancellationRequested) {
        await UniTask.Yield(token);
        TimerManager.UpdateTimers();
      }
    }

    void IStateObserver<GameplayState>.OnEnter() {
      UniTask.Void(RunTimers);
    }

    void IStateObserver<GameplayState>.OnExit() {
      cts.Cancel();
    }

    public void Dispose() {
      cts?.Cancel();
      cts?.Dispose();
    }
  }
}