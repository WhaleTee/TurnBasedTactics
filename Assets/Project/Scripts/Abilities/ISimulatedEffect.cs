namespace TurnBasedTactics.Abilities {
  public interface ISimulatedEffect<in TTarget> : IEffect<TTarget> where TTarget : IEffectTarget {
    void Simulate(Ability ability, IEffectTarget target);
  }
}