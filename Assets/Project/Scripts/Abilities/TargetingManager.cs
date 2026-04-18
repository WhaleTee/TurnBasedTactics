using System;
using System.Threading;
using Reflex.Attributes;
using TurnBasedTactics.DI;
using WhaleTee.FSM;

namespace TurnBasedTactics.Abilities.Targeting {
  public class TargetingManager : IStateUpdateObserver<GameplayState>, IInitializable, IDisposable {
    [Inject]
    readonly StateObserveManager stateObserveManager;

    readonly CancellationTokenSource cts = new();
    TargetingStrategy currentStrategy;

    public void Initialize() {
      stateObserveManager.RegisterStateUpdateObserver(this, cts.Token);
    }

    public void OnUpdate() {
      if (currentStrategy is { IsTargeting: true }) currentStrategy.Update();
    }

    public void SetCurrentStrategy(TargetingStrategy strategy) {
      currentStrategy = strategy;
    }

    public void ClearCurrentStrategy() {
      currentStrategy = null;
    }

    public void Dispose() {
      cts?.Cancel();
      cts?.Dispose();
    }
  }
}