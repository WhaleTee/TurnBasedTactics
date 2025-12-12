using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WhaleTee.Factory {
  public class GameObjectAsyncFactory : IPrefabAsyncFactory<GameObject> {
    public async UniTask<GameObject> Instantiate(GameObject prefab) {
      return await Instantiate(prefab, Vector3.zero, Quaternion.identity);
    }

    public async UniTask<GameObject> Instantiate(GameObject prefab, Vector3 position, Quaternion rotation) {
      return await Instantiate(prefab, position, rotation, null);
    }

    public async UniTask<GameObject> Instantiate(GameObject prefab, Transform parent, Quaternion rotation) {
      return await Instantiate(prefab, Vector3.zero, rotation, parent);
    }

    public async UniTask<GameObject> Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) {
      if (rotation == default) rotation = Quaternion.identity;
      var result = await Object.InstantiateAsync(prefab, parent, position, rotation);
      var go = result[0];
      go.name = prefab.name;
      go.transform.localScale = Vector3.one;
      return go;
    }
  }
}