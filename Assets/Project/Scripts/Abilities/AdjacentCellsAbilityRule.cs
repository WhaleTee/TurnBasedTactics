using System;
using Reflex.Attributes;
using TurnBasedTactics.Unit;
using WhaleTee.Extensions;
using WhaleTee.Grid;
using WhaleTee.Reactive.Input;

namespace TurnBasedTactics.Abilities {
  [Serializable]
  public class AdjacentCellsAbilityRule : IAbilityRule {
    [Inject] HexGridNavigationService navigationService;

    public bool CanApply(Ability ability, IEffectTarget target) {
      return navigationService.Distance(ability.owner.gameObject.GetComponent<UnitBehaviour>().CellPosition, target.CellPosition) == 1;
    }
  }
}