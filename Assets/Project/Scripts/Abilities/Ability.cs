using System;
using System.Collections.Generic;
using Reflex.Attributes;
using TurnBasedTactics.Abilities.Factory;
using TurnBasedTactics.Abilities.Targeting;
using UnityEngine;

namespace TurnBasedTactics.Abilities {
  [Serializable]
  public class Ability {
    [Header("FX")]
    public AudioClip castSfx;

    public GameObject castVfx;
    public GameObject targetVfx;

    [Header("Effects")]
    [SerializeReference]
    public List<IEffectFactory> effects = new();

    [Header("Targeting")]
    [SerializeReference]
    public TargetingStrategy targetingStrategy;

    [SerializeReference]
    public IAbilityRule rule;

    [HideInInspector]
    public GameObject owner;

    [Inject]
    TargetingManager targetingManager;

    public void Target() {
      targetingManager.SetCurrentStrategy(targetingStrategy);
      targetingStrategy?.Start(this);
    }

    public void Execute(IEffectTarget target) {
      targetingStrategy.Cancel();
      targetingManager.ClearCurrentStrategy();
      if (target == null) return;

      foreach (var effect in effects) effect.Create().Apply(this, target);
    }
  }
}