using System;
using UnityEngine;
using WhaleTee.Factory.Context;

namespace WhaleTee.Factory {
  public interface IDeactivatedGameObjectFactory : IPrefabFactory<GameObject> {
    private GameObject CreateDeactivated(GameObject prefab, Func<GameObject> factory) {
      prefab.SetActive(false);
      var go = factory?.Invoke();
      prefab.SetActive(true);
      return go;
    }

    GameObject CreateDeactivated(GameObject prefab) => CreateDeactivated(prefab, () => Create(new PrefabContext { prefab = prefab }));

    GameObject CreateDeactivated(GameObject prefab, Vector3 position, Quaternion rotation) {
      return CreateDeactivated(prefab, () => Create(new PrefabContext { prefab = prefab, position = position, rotation = rotation}));
    }

    GameObject CreateDeactivated(GameObject prefab, Transform parent, Quaternion rotation) {
      return CreateDeactivated(prefab, () => Create(new PrefabContext { prefab = prefab, rotation = rotation, parent = parent }));
    }

    GameObject CreateDeactivated(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) {
      return CreateDeactivated(
        prefab,
        () => Create(new PrefabContext { prefab = prefab, position = position, rotation = rotation, parent = parent })
      );
    }
  }
}