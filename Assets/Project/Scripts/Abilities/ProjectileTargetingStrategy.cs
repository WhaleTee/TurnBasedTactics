using System;
using System.Collections.Generic;
using R3;
using Reflex.Attributes;
using TurnBasedTactics.Unit;
using UnityEngine;
using WhaleTee.Extensions;
using WhaleTee.Grid;
using WhaleTee.Reactive.Input;
using ZLinq;

namespace TurnBasedTactics.Abilities.Targeting {
  public class ProjectileTargetingStrategy : TargetingStrategy {
    [Inject]
    UserInput userInput;

    [Inject]
    SquadData squad;

    [Inject]
    HexGridNavigationService navigationService;

    [Inject]
    GroundTilemapToWorldPositionService tilemapToWorldService;

    [Inject]
    NavigationTilemapContainer navigationTilemap;

    GameObject target;
    IDisposable subscription;

    void Cleanup() {
      ability = null;
      isTargeting = false;
      target = null;
      subscription?.Dispose();
    }

    public override void Start(Ability ability) {
      this.ability = ability;
      isTargeting = true;
      var subscriptionBuilder = Disposable.CreateBuilder();

      userInput.PointerWorldPosition
               .Select(value => value.AsVector2())
               .Subscribe(value => HandlePointerPosition(value, ability))
               .AddTo(ref subscriptionBuilder);

      userInput.LeftClick
               .Where(value => value)
               .Subscribe(_ => HandleLeftClick(ability))
               .AddTo(ref subscriptionBuilder);

      subscription = subscriptionBuilder.Build();
    }

    void HandleLeftClick(Ability ability) {
      if (target != null) ability.Execute(target.GetComponent<IEffectTarget>());
    }

    void HandlePointerPosition(Vector2 pointerPosition, Ability ability) {
      var unit = new[] { squad.firstUnit, squad.secondUnit, squad.thirdUnit }
                 .AsValueEnumerable()
                 .FirstOrDefault(u => u.state.position.cellPosition == tilemapToWorldService.GetCellPosition(pointerPosition));

      if (unit != null && ability.rule.CanApply(ability, unit.state.gameObject.GetComponent<IEffectTarget>())) target = unit.state.gameObject;
      else target = null;
    }

    public override void Cancel() {
      base.Cancel();
      Cleanup();
    }
  }
}