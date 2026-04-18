using System.Collections.Generic;
using TurnBasedTactics.DI;
using UnityEngine;
using UnityEngine.Pool;
using WhaleTee.Factory;
using ZLinq;
using Object = UnityEngine.Object;

namespace WhaleTee.Pooling {
  public enum PoolType {
    GameObjects, SFX, VFX
  }

  public class ObjectPool : IInitializable {
    readonly IDeactivatedGameObjectFactory objectFactory;
    readonly Dictionary<GameObject, ObjectPool<GameObject>> objectPool = new();
    readonly Dictionary<GameObject, GameObject> objectPrefab = new();
    
    GameObject poolsContainer;
    GameObject gameObjectsContainer;
    GameObject sfxContainer;
    GameObject vfxContainer;

    public ObjectPool(IDeactivatedGameObjectFactory objectFactory) {
      this.objectFactory = objectFactory;
    }

    void SetupContainers() {
      poolsContainer = new GameObject("Object Pools Container");
      
      var parentTransform = poolsContainer.transform;

      gameObjectsContainer = new GameObject("GameObjects");
      gameObjectsContainer.transform.SetParent(parentTransform);

      sfxContainer = new GameObject("SFX");
      sfxContainer.transform.SetParent(parentTransform);

      vfxContainer = new GameObject("VFX");
      vfxContainer.transform.SetParent(parentTransform);

      Object.DontDestroyOnLoad(vfxContainer.transform.root);
      Object.DontDestroyOnLoad(gameObjectsContainer.transform.root);
      Object.DontDestroyOnLoad(sfxContainer.transform.root);
    }

    void OnGetObject(GameObject go) { }

    void OnReleaseObject(GameObject go) {
      go.transform.localScale = Vector3.one;
      go.SetActive(false);
    }

    void OnDestroyObject(GameObject go) {
      objectPrefab.Remove(go);
      Object.Destroy(go);
    }

    void CreatePool(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType) {
      var pool = new ObjectPool<GameObject>(
        () => CreateObject(prefab, position, rotation, poolType),
        OnGetObject,
        OnReleaseObject,
        OnDestroyObject
      );

      objectPool.Add(prefab, pool);
    }

    void CreatePool(GameObject prefab, Transform parent, Quaternion rotation, PoolType poolType) {
      var pool = new ObjectPool<GameObject>(
        () => CreateObject(prefab, parent, rotation, poolType),
        OnGetObject,
        OnReleaseObject,
        OnDestroyObject
      );

      objectPool.Add(prefab, pool);
    }

    GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObjects) {
      var parent = GetPoolContainer(poolType);
      return objectFactory.CreateDeactivated(prefab, position, rotation, parent.transform);
    }

    GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rotation, PoolType poolType = PoolType.GameObjects) {
      var go = objectFactory.CreateDeactivated(prefab, parent.transform, rotation);
      var parentObject = GetPoolContainer(poolType);
      go.transform.SetParent(parentObject.transform);
      return go;
    }

    GameObject GetPoolContainer(PoolType poolType) {
      return poolType switch { PoolType.VFX => vfxContainer, PoolType.GameObjects => gameObjectsContainer, PoolType.SFX => sfxContainer, var _ => null };
    }

    T SpawnObject<T>(
      GameObject prefab,
      Vector3 position,
      Quaternion rotation,
      PoolType poolType = PoolType.GameObjects
    ) where T : Object {
      if (!objectPool.ContainsKey(prefab)) CreatePool(prefab, position, rotation, poolType);

      var go = objectPool[prefab].Get();

      if (!go) return null;

      objectPrefab.TryAdd(go, prefab);

      go.transform.position = position;
      go.transform.rotation = rotation;
      go.SetActive(true);

      if (typeof(T) == typeof(GameObject)) return go as T;

      var component = go.GetComponent<T>();

      if (component) return component;

      Debug.LogError($"Object {prefab.name} doesn't have component of type {typeof(T)}");
      return null;
    }

    T SpawnObject<T>(
      GameObject prefab,
      Transform parent,
      Quaternion rotation,
      PoolType poolType = PoolType.GameObjects
    ) where T : Object {
      if (!objectPool.ContainsKey(prefab)) CreatePool(prefab, parent, rotation, poolType);

      var go = objectPool[prefab].Get();

      if (!go) return null;

      objectPrefab.TryAdd(go, prefab);

      go.transform.SetParent(parent);
      go.transform.localPosition = Vector3.zero;
      go.transform.localRotation = rotation;
      go.SetActive(true);

      var result = go as T;

      if (result) return result;

      Debug.LogError($"Object {prefab.name} doesn't have component of type {typeof(T)}.");

      return null;
    }

    public void Initialize() {
      SetupContainers();
    }

    public T SpawnObject<T>(
      T typePrefab,
      Vector3 position,
      Quaternion rotation,
      PoolType poolType = PoolType.GameObjects
    ) where T : Component {
      return SpawnObject<T>(typePrefab.gameObject, position, rotation, poolType);
    }

    public GameObject SpawnObject(
      GameObject prefab,
      Vector3 position,
      Quaternion rotation,
      PoolType poolType = PoolType.GameObjects
    ) {
      return SpawnObject<GameObject>(prefab, position, rotation, poolType);
    }

    public T SpawnObject<T>(
      T typePrefab,
      Transform parent,
      Quaternion rotation,
      PoolType poolType = PoolType.GameObjects
    ) where T : Component {
      return SpawnObject<T>(typePrefab.gameObject, parent, rotation, poolType);
    }

    public GameObject SpawnObject(
      GameObject prefab,
      Transform parent,
      Quaternion rotation,
      PoolType poolType = PoolType.GameObjects
    ) {
      return SpawnObject<GameObject>(prefab, parent, rotation, poolType);
    }

    public void ReturnObjectToPool(GameObject go, PoolType poolType = PoolType.GameObjects) {
      if (objectPrefab.TryGetValue(go, out var prefab)) {
        var poolContainer = GetPoolContainer(poolType);

        if (go.transform.parent != poolContainer.transform) go.transform.SetParent(poolContainer.transform);
        if (objectPool.TryGetValue(prefab, out var pool)) pool.Release(go);
      } else Debug.LogWarning("Trying to return an object that is not pooled: " + go.name);
    }

    public void ReturnObjectsToPool(GameObject prefab, PoolType poolType = PoolType.GameObjects) {
      var gos = GetObjectsByPrefab(prefab);

      foreach (var go in gos) {
        var poolContainer = GetPoolContainer(poolType);
        if (go.transform.parent != poolContainer.transform) go.transform.SetParent(poolContainer.transform);
        if (objectPool.TryGetValue(prefab, out var pool)) pool.Release(go);
      }
    }

    IEnumerable<GameObject> GetObjectsByPrefab(Object prefab) {
      return objectPrefab
             .Where(pair => pair.Value == prefab)
             .Where(pair => pair.Key.activeInHierarchy)
             .Select(map => map.Key)
             .ToArray();
    }
  }
}