using System;
using System.Collections.Generic;
using System.Linq;
using WhaleTee.Factory;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace WhaleTee.Pooling {
  public enum PoolType {
    GameObjects, SFX, VFX
  }

  public class ObjectPool : IInitializable {
    readonly IDeactivatedGameObjectFactory factory;
    GameObject emptyHolder;
    GameObject gameObjectsEmpty;
    GameObject sfxEmpty;
    GameObject vfxEmpty;

    Dictionary<GameObject, ObjectPool<GameObject>> objectPools;
    Dictionary<GameObject, GameObject> cloneToPrefabMap;
    
    public ObjectPool(IDeactivatedGameObjectFactory factory) => this.factory = factory;

    void SetupEmpties() {
      emptyHolder = new GameObject("ObjectPools");

      gameObjectsEmpty = new GameObject("GameObjects");
      gameObjectsEmpty.transform.SetParent(emptyHolder.transform);

      sfxEmpty = new GameObject("SFX");
      sfxEmpty.transform.SetParent(emptyHolder.transform);

      vfxEmpty = new GameObject("VFX");
      vfxEmpty.transform.SetParent(emptyHolder.transform);

      Object.DontDestroyOnLoad(vfxEmpty.transform.root);
      Object.DontDestroyOnLoad(gameObjectsEmpty.transform.root);
      Object.DontDestroyOnLoad(sfxEmpty.transform.root);
    }

    void OnGetObject(GameObject go) { }

    void OnReleaseObject(GameObject go) {
      go.transform.localScale = Vector3.one;
      go.SetActive(false);
    }

    void OnDestroyObject(GameObject go) {
      cloneToPrefabMap.Remove(go);
      Object.Destroy(go);
    }

    void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType) {
      var pool = new ObjectPool<GameObject>(
        () => CreateObject(prefab, pos, rot, poolType),
        OnGetObject,
        OnReleaseObject,
        OnDestroyObject
      );

      objectPools.Add(prefab, pool);
    }

    void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType) {
      var pool = new ObjectPool<GameObject>(
        () => CreateObject(prefab, parent, rot, poolType),
        OnGetObject,
        OnReleaseObject,
        OnDestroyObject
      );

      objectPools.Add(prefab, pool);
    }

    GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObjects) {
      var parent = SetParentObject(poolType);
      return factory.CreateDeactivated(prefab, position, rotation, parent.transform);
    }

    GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rotation, PoolType poolType = PoolType.GameObjects) {
      var go = factory.CreateDeactivated(prefab, parent.transform, rotation);
      var parentObject = SetParentObject(poolType);
      go.transform.SetParent(parentObject.transform);
      return go;
    }

    GameObject SetParentObject(PoolType poolType) {
      return poolType switch {
               PoolType.VFX => vfxEmpty,
               PoolType.GameObjects => gameObjectsEmpty,
               PoolType.SFX => sfxEmpty,
               var _ => null
             };
    }

    T SpawnObject<T>(
      GameObject objectToSpawn,
      Vector3 spawnPos,
      Quaternion spawnRotation,
      PoolType poolType = PoolType.GameObjects
    ) where T : Object {
      if (!objectPools.ContainsKey(objectToSpawn)) CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);

      var go = objectPools[objectToSpawn].Get();

      if (!go) return null;

      cloneToPrefabMap.TryAdd(go, objectToSpawn);

      go.transform.position = spawnPos;
      go.transform.rotation = spawnRotation;
      go.SetActive(true);

      if (typeof(T) == typeof(GameObject)) return go as T;

      var component = go.GetComponent<T>();

      if (component) return component;

      Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
      return null;
    }

    T SpawnObject<T>(
      GameObject objectToSpawn,
      Transform parent,
      Quaternion spawnRotation,
      PoolType poolType = PoolType.GameObjects
    ) where T : Object {
      if (!objectPools.ContainsKey(objectToSpawn)) CreatePool(objectToSpawn, parent, spawnRotation, poolType);

      var go = objectPools[objectToSpawn].Get();

      if (!go) return null;

      cloneToPrefabMap.TryAdd(go, objectToSpawn);

      go.transform.SetParent(parent);
      go.transform.localPosition = Vector3.zero;
      go.transform.localRotation = spawnRotation;
      go.SetActive(true);

      var result = go as T;

      if (result) return result;

      Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}.");

      return null;
    }

    public void Initialize() {
      objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
      cloneToPrefabMap = new Dictionary<GameObject, GameObject>();
      SetupEmpties();
    }

    public T SpawnObject<T>(
      T typePrefab,
      Vector3 spawnPos,
      Quaternion spawnRotation,
      PoolType poolType = PoolType.GameObjects
    ) where T : Component {
      return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }

    public GameObject SpawnObject(
      GameObject objectToSpawn,
      Vector3 spawnPos,
      Quaternion spawnRotation,
      PoolType poolType = PoolType.GameObjects
    ) {
      return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }

    public T SpawnObject<T>(
      T typePrefab,
      Transform parent,
      Quaternion spawnRotation,
      PoolType poolType = PoolType.GameObjects
    ) where T : Component {
      return SpawnObject<T>(typePrefab.gameObject, parent, spawnRotation, poolType);
    }

    public GameObject SpawnObject(
      GameObject objectToSpawn,
      Transform parent,
      Quaternion spawnRotation,
      PoolType poolType = PoolType.GameObjects
    ) {
      return SpawnObject<GameObject>(objectToSpawn, parent, spawnRotation, poolType);
    }

    public void ReturnObjectToPool(GameObject go, PoolType poolType = PoolType.GameObjects) {
      if (cloneToPrefabMap.TryGetValue(go, out var prefab)) {
        var parentObject = SetParentObject(poolType);

        if (go.transform.parent != parentObject.transform) go.transform.SetParent(parentObject.transform);
        if (objectPools.TryGetValue(prefab, out var pool)) pool.Release(go);
      } else Debug.LogWarning("Trying to return an object that is not pooled: " + go.name);
    }

    public void ReturnObjectsToPool(GameObject prefab, PoolType poolType = PoolType.GameObjects) {
      var gos = cloneToPrefabMap
                .Where(map => map.Value == prefab && map.Key.activeInHierarchy)
                .Select(map => map.Key);

      foreach (var go in gos) {
        var parentObject = SetParentObject(poolType);
        if (go.transform.parent != parentObject.transform) go.transform.SetParent(parentObject.transform);
        if (objectPools.TryGetValue(prefab, out var pool)) pool.Release(go);
      }
    }
  }
}