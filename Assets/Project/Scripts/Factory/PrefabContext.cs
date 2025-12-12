using UnityEngine;

namespace WhaleTee.Factory.Context {
  public struct PrefabContext {
    public GameObject prefab;
    public Vector3 position;
    public Quaternion rotation;
    public Transform parent;
  }
}