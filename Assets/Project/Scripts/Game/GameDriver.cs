using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TurnBasedTactics.Unit;
using WhaleTee.FSM;

public class GameDriver : IInitializable, IDisposable {
  StateMachine stateMachine;
  StateMachine turnStateMachine;
  SquadData squad;
  IDisposable subscription;
  CancellationTokenSource cts;

  public void Initialize() {
    cts = new CancellationTokenSource();

    stateMachine = StateMachine.StateMachineBuilder.Create()
                               .AddInitialState(new GameInitializationState())
                               .AddState(new WaitForAnyInputState())
                               .AddState(new MenuState())
                               .AddState(new DeployState())
                               .AddState(new GameplayState())
                               .Build();

    UniTask.Void(Update);
    UniTask.Void(FixedUpdate);
  }

  async UniTaskVoid Update() {
    while (!cts.Token.IsCancellationRequested) {
      stateMachine.Update();
      // turnStateMachine.Update();
      await UniTask.Yield(PlayerLoopTiming.LastUpdate, cts.Token, true);
    }
  }

  async UniTaskVoid FixedUpdate() {
    while (!cts.Token.IsCancellationRequested) {
      stateMachine.FixedUpdate();
      // turnStateMachine.FixedUpdate();
      await UniTask.WaitForFixedUpdate(cts.Token, true);
    }
  }

  public void Dispose() {
    cts?.Cancel();
    cts?.Dispose();
    subscription?.Dispose();
    stateMachine?.Dispose();
    // turnStateMachine?.Dispose();
  }
}