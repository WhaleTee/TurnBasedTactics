using System;
using System.Threading;
using MessagePipe;
using R3;
using Reflex.Attributes;
using UnityEngine;
using WhaleTee.FSM;
using WhaleTee.MessagePipe.Message;
using WhaleTee.Reactive.Input;

namespace TurnBasedTactics.Gameplay {
  public class CharacterSelector : IStateObserver<GameplayState> {
    [Inject] UserInput userInput;
    [Inject] IPublisher<CharacterSelectedEventMessage> characterSelectedPublisher;
    readonly LayerMask characterLayerMask = LayerMask.GetMask("Character");
    IDisposable leftClickSubscription;

    CancellationTokenSource IStateObserver<GameplayState>.StateObserverTokenSource { get; } = new();

    void IStateObserver<GameplayState>.OnEnter() => leftClickSubscription = userInput.LeftClick.Subscribe(HandleLeftClick);

    void IStateObserver<GameplayState>.OnExit() => leftClickSubscription?.Dispose();

    void HandleLeftClick(bool click) {
      if (!click) return;
      var pointerPosition = userInput.GetPointerPositionWorld();

      characterSelectedPublisher.Publish(
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

    public void Dispose() {
      leftClickSubscription?.Dispose();
    }
  }
}