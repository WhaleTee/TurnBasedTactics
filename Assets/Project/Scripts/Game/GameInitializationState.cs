using System;
using Reflex.Attributes;
using TurnBasedTactics.UI;
using WhaleTee.FSM;

public sealed class GameInitializationState : State {
  [Inject]
  MainMenuUI mainMenuUI;

  bool initialized;

  protected override Type GetTransition() {
    return initialized ? typeof(WaitForAnyInputState) : null;
  }

  protected override void OnEnter() {
    mainMenuUI.ShowBackground();
    mainMenuUI.ShowPressAnyKeyLabel();
    initialized = true;
  }

  protected override void OnExit() {
    initialized = false;
  }
}