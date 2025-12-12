using System;
using Cysharp.Threading.Tasks;
using R3;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine.SceneManagement;
using WhaleTee.FSM;
using WhaleTee.Reactive.Input;

public sealed class WaitForAnyInputState : State {
  [Inject] UserInput userInput;
  [Inject] MainMenuUIContainer mainMenuUI;
  bool anyKeyPerformed;
  IDisposable subscription;

  public WaitForAnyInputState() {
    AttributeInjector.Inject(this, SceneManager.GetActiveScene().GetSceneContainer());
  }

  protected override Type GetTransition() {
    return anyKeyPerformed ? typeof(MenuState) : null;
  }

  protected override void OnEnter() {
    subscription = userInput.AnyKey.Where(value => value)
                            .Subscribe(_ => {
                                         anyKeyPerformed = true;
                                         UniTask.Void(mainMenuUI.GetComponent<MainMenuUI>().HidePressAnyKeyLabel);
                                       }
                            );
  }

  protected override void OnExit() {
    anyKeyPerformed = false;
    subscription?.Dispose();
  }
}