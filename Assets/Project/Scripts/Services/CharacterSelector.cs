using System;
using System.Threading;
using MessagePipe;
using R3;
using Reflex.Attributes;
using TurnBasedTactics.DI;
using UnityEngine;
using WhaleTee.FSM;
using WhaleTee.MessagePipe.Message;
using WhaleTee.Reactive.Input;

namespace TurnBasedTactics.Gameplay {
  public class CharacterSelector : IStateEnterObserver<GameplayState>, IStateExitObserver<GameplayState>, IInitializable, IDisposable {
    readonly StateObserveManager stateObserveManager;
    readonly UserInput userInput;
    readonly IPublisher<CharacterSelectedEventMessage> characterSelectedEventPublisher;
    readonly LayerMask characterLayerMask = LayerMask.GetMask("Character");
    readonly CancellationTokenSource cts = new();
    IDisposable leftClickSubscription;

    public CharacterSelector(StateObserveManager stateObserveManager, UserInput userInput, IPublisher<CharacterSelectedEventMessage> characterSelectedEventPublisher) {
      this.stateObserveManager = stateObserveManager;
      this.userInput = userInput;
      this.characterSelectedEventPublisher = characterSelectedEventPublisher;
    }

    void HandleLeftClick(bool click) {
      if (!click) return;

      var pointerPosition = userInput.GetPointerPositionWorld();

      characterSelectedEventPublisher.Publish(
        new CharacterSelectedEventMessage {
          gameObject = Raycast2DTool.OverlapOne(
            pointerPosition,
            characterLayerMask,
            -pointerPosition.z,
            pointerPosition.z
          )
        }
      );
    }

    public void Initialize() {
      stateObserveManager.RegisterStateEnterObserver(this, cts.Token);
      stateObserveManager.RegisterStateExitObserver(this, cts.Token);
    }

    public void OnEnter() {
      leftClickSubscription = userInput.LeftClick.Subscribe(HandleLeftClick);
    }

    public void OnExit() {
      leftClickSubscription?.Dispose();
      leftClickSubscription = null;
    }

    public void Dispose() {
      leftClickSubscription?.Dispose();
      cts?.Cancel();
      cts?.Dispose();
    }
  }
}