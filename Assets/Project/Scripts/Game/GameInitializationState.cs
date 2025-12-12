using System;
using Cysharp.Threading.Tasks;
using WhaleTee.Factory;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhaleTee.Assemblies;
using WhaleTee.FSM;
using WhaleTee.TurnBasedTactics.DI;

public sealed class GameInitializationState : State {
  [Inject] IPrefabAsyncFactory<GameObject> factory;
  [Inject] MainMenuUIContainer mainMenuUI;
  bool initialized;

  public GameInitializationState() {
    AttributeInjector.Inject(this, SceneManager.GetActiveScene().GetSceneContainer());
  }

  async UniTaskVoid InstantiateUIAsync() {
    await UniTask.WaitForSeconds(3);
    await mainMenuUI.GetComponent<MainMenuUI>().ShowBackground();
    initialized = true;
  }

  protected override Type GetTransition() {
    return initialized ? typeof(WaitForAnyInputState) : null;
  }

  protected override void OnEnter() {
    UniTask.Void(InstantiateUIAsync);
  }

  protected override void OnExit() {
    initialized = false;
  }
}