using System;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnBasedTactics.Unit {
  [Serializable]
  public class SquadUnit {
    public UnitConfiguration configuration;

    [HideInInspector]
    public UnitState state;

    public void InjectDependencies() {
      foreach (var ability in state.abilities) {
        AttributeInjector.Inject(ability, SceneManager.GetActiveScene().GetSceneContainer());
        AttributeInjector.Inject(ability.rule, SceneManager.GetActiveScene().GetSceneContainer());
        AttributeInjector.Inject(ability.targetingStrategy, SceneManager.GetActiveScene().GetSceneContainer());
        ability.effects.ForEach(e => AttributeInjector.Inject(e, SceneManager.GetActiveScene().GetSceneContainer()));
      }
    }
  }
}