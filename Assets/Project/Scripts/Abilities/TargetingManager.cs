using System.Threading;
using WhaleTee.FSM;

namespace TurnBasedTactics.Abilities.Targeting {
  public class TargetingManager : IStateObserver<GameplayState> {
    TargetingStrategy currentStrategy;

    public static TargetingManager instance;
    CancellationTokenSource IStateObserver<GameplayState>.StateObserverTokenSource { get; } = new();

    public TargetingManager() => instance ??= this;
    ~TargetingManager() => instance = null;

    void IStateObserver<GameplayState>.OnUpdate() {
      if (currentStrategy is { IsTargeting: true }) currentStrategy.Update();
    }

    public void SetCurrentStrategy(TargetingStrategy strategy) => currentStrategy = strategy;
    public void ClearCurrentStrategy() => currentStrategy = null;
  }
}