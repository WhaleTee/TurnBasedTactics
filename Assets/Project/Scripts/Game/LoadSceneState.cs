using System;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

public abstract class LoadSceneState : ParametrizedState<string> {
  readonly Type nextState;
  [Inject] SceneService sceneService;
  bool isLoadingDone;

  protected LoadSceneState(string parameter, Type nextState) : base(parameter) {
    this.nextState = nextState;
  }

  protected override void OnEnter() {
    UniTask.Void(() => sceneService.LoadScene(parameter, () => isLoadingDone = true));
  }

  protected override Type GetTransition() {
    return isLoadingDone ? nextState : null;
  }
}