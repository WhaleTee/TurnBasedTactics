using System;
using Reflex.Attributes;
using TurnBasedTactics.UI;
using UnityEngine.UIElements;
using WhaleTee.FSM;

public sealed class MainMenuState : State {
  [Inject]
  MainMenuUI mainMenuUI;

  bool startClicked;

  void OnStartButtonClicked(PointerDownEvent evt) {
    startClicked = true;
  }

  protected override Type GetTransition() {
    return startClicked ? typeof(DeployState) : null;
  }

  protected override void OnEnter() {
    mainMenuUI.NewGamePanel.RegisterStartButtonCallback(OnStartButtonClicked);
  }

  protected override void OnExit() {
    base.OnExit();
    mainMenuUI.NewGamePanel.UnregisterStartButtonCallback(OnStartButtonClicked);
  }
}