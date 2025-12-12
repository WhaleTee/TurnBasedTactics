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
    [Inject] readonly UserInput userInput;
    readonly HashSet<GameObject> trackingGameObjects;
    readonly List<RaycastResult> raycastResults;
    readonly PointerEventData pointerEventData;
    readonly IDisposable subscriptions;
    GameObject pointerOverGameObject;

    static EventSystem EventSystem => EventSystem.current;

    public PointerOverGameObjectTracker(UserInput userInput) {
      this.userInput = userInput;
      trackingGameObjects = new HashSet<GameObject>();
      pointerEventData = new PointerEventData(EventSystem);
      raycastResults = new List<RaycastResult>();
      subscriptions = userInput.PointerPosition.Subscribe(_ => pointerOverGameObject = null);
    }

    static bool IsGameObjectEnabled(GameObject gameObject) => gameObject.activeSelf && gameObject.activeInHierarchy;

    bool IsPointerOverGameObject() {
      if (userInput == null) return false;
      if (pointerOverGameObject.OrNull() != null) return true;

      pointerEventData.position = userInput.PointerPosition.Value;

      raycastResults.Clear();
      EventSystem.RaycastAll(pointerEventData, raycastResults);

      pointerOverGameObject = raycastResults.AsValueEnumerable()
                                            .Select(result => result.gameObject)
                                            .Where(IsGameObjectEnabled)
                                            .FirstOrDefault(trackingGameObjects.Contains);

      return pointerOverGameObject.OrNull() != null;
    }

    public void Track(GameObject element) => trackingGameObjects.Add(element);
    
    public void Untrack(GameObject element) => trackingGameObjects.Remove(element);

    public bool IsTracked(GameObject element) => trackingGameObjects.Contains(element);
    
    public bool IsPointerOverUI() => IsPointerOverGameObject();

    public void Dispose() => subscriptions?.Dispose();
  }
}