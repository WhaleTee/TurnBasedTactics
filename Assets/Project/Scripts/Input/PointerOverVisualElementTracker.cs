using System;
using System.Collections.Generic;
using MessagePipe;
using R3;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;
using WhaleTee.MessagePipe.Message;
using WhaleTee.Reactive.Input;
using ZLinq;
using Screen = UnityEngine.Device.Screen;

namespace WhaleTee.Input {

  public sealed class PointerOverVisualElementTracker : IPointerOverTracker<VisualElement>, IDisposable {
    [Inject] readonly UserInput userInput;
    [Inject] readonly IPublisher<PointerExitVisualElementEvent> exitVisualElementPublisher;
    [Inject] readonly IPublisher<PointerEnterVisualElementEvent> enterVisualElementPublisher;
    readonly HashSet<VisualElement> trackingElements = new();
    readonly IDisposable subscriptions;
    VisualElement pointerOverElement;

    public PointerOverVisualElementTracker(UserInput userInput, IPublisher<PointerEnterVisualElementEvent> enterVisualElementPublisher) {
      this.userInput = userInput;
      this.enterVisualElementPublisher = enterVisualElementPublisher;
      subscriptions = userInput.PointerPosition.Subscribe(HandlePointerPosition);
    }

    static bool IsElementEnabled(VisualElement element) => element.visible && element.resolvedStyle.display == DisplayStyle.Flex;

    void HandlePointerPosition(Vector2 position) {
      if (pointerOverElement == null && IsPointerOverUI()) {
        enterVisualElementPublisher.Publish(new PointerEnterVisualElementEvent(pointerOverElement));
      }
      
      if (pointerOverElement != null && !IsElementContainsPointer(pointerOverElement)) {
        exitVisualElementPublisher.Publish(new PointerExitVisualElementEvent(pointerOverElement));
        if (IsPointerOverUI()) enterVisualElementPublisher.Publish(new PointerEnterVisualElementEvent(pointerOverElement));
      }
    }

    bool IsElementContainsPointer(VisualElement element) {
      if (userInput == null || element == null) return false;

      // we need to get y-inverted pointer position here
      // UI Toolkit coordinates origin is a top-left, but for the Screen is a bottom-left
      var pointerPosition = userInput.GetPointerPositionInvertY(Screen.height);

      if (!element.worldBound.Contains(RuntimePanelUtils.ScreenToPanel(element.panel, pointerPosition))) return false;

      pointerOverElement = element;
      return true;
    }

    public void Track(VisualElement element) => trackingElements.Add(element);

    public void Untrack(VisualElement element) => trackingElements.Remove(element);

    public bool IsTracked(VisualElement element) => trackingElements.Contains(element) && IsElementEnabled(element);

    public bool IsPointerOverUI() => trackingElements.AsValueEnumerable().Where(IsElementEnabled).Any(IsElementContainsPointer);

    public void Dispose() => subscriptions?.Dispose();
  }
}