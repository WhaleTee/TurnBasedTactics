using WhaleTee.Factory;

namespace TurnBasedTactics.Abilities.Factory {
  public interface IEffectFactory : IFactory<IEffect<IEffectTarget>> { }
}