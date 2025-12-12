namespace TurnBasedTactics.Abilities {
  public interface IAbilityRule {
    bool CanApply(Ability ability, IEffectTarget target);
  }
}