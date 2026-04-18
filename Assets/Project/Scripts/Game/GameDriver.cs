using WhaleTee.FSM;
using WhaleTee.Lifecycle;
using TurnBasedTactics.DI;
using WhaleTee.Reflex.Extensions;

namespace TurnBasedTactics.Game {
  public class GameDriver : IInitializable, IUpdateable, IFixedUpdateable {
    StateMachine stateMachine;

    public void Initialize() {
      stateMachine = StateMachine.StateMachineBuilder.Create()
                                 .AddInitialState(new GameInitializationState().Inject())
                                 .AddState(new WaitForAnyInputState().Inject())
                                 .AddState(new MainMenuState().Inject())
                                 .AddState(new DeployState())
                                 .AddState(new GameplayState())
                                 .Build();
    }

    public void Update() {
      stateMachine.Update();
    }

    public void FixedUpdate() {
      stateMachine.FixedUpdate();
    }
  }
}