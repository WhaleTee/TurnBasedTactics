using System;
using System.Collections.Generic;
using WhaleTee.Factory;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhaleTee.Extensions;
using WhaleTee.Factory.Context;

[Serializable]
public abstract class ComponentProvider : IInitializable {
  [SerializeField] GameObject prefab;
  [Inject] IDeactivatedGameObjectFactory factory;
  GameObject instance;
  Dictionary<Type, Component> components = new();

  GameObject GetInstance() => instance ??= factory.Create(new PrefabContext { prefab = prefab });

  void Spawn() => instance = GetInstance();

  public T GetComponent<T>() where T : Component {
    if (!components.TryGetValue(typeof(T), out var component)) {
      component = GetInstance().GetComponent<T>();
      if (component == null) return null;

      components.Add(typeof(T), component);
    }

    return component as T;
  }

  public T GetOrAddComponent<T>() where T : Component {
    if (!components.TryGetValue(typeof(T), out var component)) components.Add(typeof(T), GetInstance().GetOrAdd<T>());
    return component as T;
  }

  public void Initialize() {
    AttributeInjector.Inject(this, SceneManager.GetActiveScene().GetSceneContainer());
    Spawn();
  }
}