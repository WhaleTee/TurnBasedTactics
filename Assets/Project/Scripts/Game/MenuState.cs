using System;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhaleTee.FSM;

public sealed class MenuState : State {
  [Inject] MainMenuUIContainer mainMenuUI;
  bool startClicked;

  public MenuState() {
    AttributeInjector.Inject(this, SceneManager.GetActiveScene().GetSceneContainer());
  }

  protected override Type GetTransition() {
    return startClicked ? typeof(DeployState) : null;
  }

  protected override void OnEnter() {
    var ui = mainMenuUI.GetComponent<MainMenuUI>();
    ui.SetupButtons().Forget();
    ui.RegisterNewGamePanelStartButtonCallback(() => startClicked = true);
  }

  protected override void OnExit() {
    base.OnExit();
  }
}