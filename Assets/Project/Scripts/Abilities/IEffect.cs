using System;

namespace TurnBasedTactics.Abilities {
  public interface IEffect<in TTarget> where TTarget : IEffectTarget {
    event Action<IEffect<TTarget>> onCompleted;
    void Apply(Ability ability, TTarget target);
    void Cancel();
  }
}