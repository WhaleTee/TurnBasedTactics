using System;
using System.Collections.Generic;
using R3;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using WhaleTee.Extensions;
using WhaleTee.Reactive.Input;
using ZLinq;

namespace WhaleTee.Input {
  public sealed class PointerOverGameObjectTracker : IPointerOverTracker<GameObject>, IDisposable {
    [Inject]
    readonly UserInput userInput;

    readonly HashSet<GameObject> trackingGameObjects;
    readonly List<RaycastResult> raycastResults;
    readonly PointerEventData pointerEventData;
    readonly IDisposable subscriptions;
    GameObject gameObjectPointerOver;

    static EventSystem EventSystem => EventSystem.current;

    public PointerOverGameObjectTracker(UserInput userInput) {
      this.userInput = userInput;
      trackingGameObjects = new HashSet<GameObject>();
      pointerEventData = new PointerEventData(EventSystem);
      raycastResults = new List<RaycastResult>();
      subscriptions = userInput.PointerPosition.Subscribe(_ => gameObjectPointerOver = null);
    }

    static bool IsGameObjectEnabled(GameObject gameObject) {
      return gameObject.activeSelf && gameObject.activeInHierarchy;
    }

    bool IsPointerOverGameObject() {
      if (userInput == null) return false;
      if (gameObjectPointerOver.OrNull() != null) return true;

      pointerEventData.position = userInput.PointerPosition.Value;

      raycastResults.Clear();
      EventSystem.RaycastAll(pointerEventData, raycastResults);

      gameObjectPointerOver = raycastResults.AsValueEnumerable()
                                            .Select(result => result.gameObject)
                                            .Where(IsGameObjectEnabled)
                                            .FirstOrDefault(trackingGameObjects.Contains);

      return gameObjectPointerOver.OrNull() != null;
    }

    public void Track(GameObject element) {
      trackingGameObjects.Add(element);
    }

    public void Untrack(GameObject element) {
      trackingGameObjects.Remove(element);
    }

    public bool IsTracked(GameObject element) {
      return trackingGameObjects.Contains(element);
    }

    public bool IsPointerOver() {
      return IsPointerOverGameObject();
    }

    public void Dispose() {
      subscriptions?.Dispose();
    }
  }
}