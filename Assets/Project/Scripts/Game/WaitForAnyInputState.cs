using System;
using R3;
using Reflex.Attributes;
using TurnBasedTactics.UI;
using WhaleTee.FSM;
using WhaleTee.Reactive.Input;

public sealed class WaitForAnyInputState : State {
  [Inject]
  readonly UserInput userInput;

  [Inject]
  readonly MainMenuUI mainMenuUI;

  bool anyKeyPerformed;
  IDisposable subscription;

  protected override Type GetTransition() {
    return anyKeyPerformed ? typeof(MainMenuState) : null;
  }

  protected override void OnEnter() {
    subscription = userInput.AnyKey.Where(isKeyPerformed => isKeyPerformed).Subscribe(_ => OnAnyKeyPerformed());
  }

  void OnAnyKeyPerformed() {
    anyKeyPerformed = true;
    mainMenuUI.HidePressAnyKeyLabel();
    mainMenuUI.ShowGameNameLabel();
    
  }

  protected override void OnExit() {
    anyKeyPerformed = false;
    subscription?.Dispose();
  }
}