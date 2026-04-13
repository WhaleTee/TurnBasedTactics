using Reflex.Attributes;
using WhaleTee.FSM;

public class GreetingSceneState : State {
  [Inject]
  SceneService sceneService;

  protected override void OnEnter() { }
}