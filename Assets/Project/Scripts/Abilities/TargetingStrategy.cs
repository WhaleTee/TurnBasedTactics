using System;

namespace TurnBasedTactics.Abilities.Targeting {
  [Serializable]
  public class SelfTargetingStrategy : TargetingStrategy {
    public override void Start(Ability ability) {
      if (ability.owner.TryGetComponent(out IEffectTarget target)) ability.Execute(target);
    }
  }

  [Serializable]
  public abstract class TargetingStrategy {
    protected Ability ability;
    protected bool isTargeting;

    public bool IsTargeting => isTargeting;

    public abstract void Start(Ability ability);
    public virtual void Update() { }
    public virtual void Cancel() { }
  }
}