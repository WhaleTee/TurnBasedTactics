using UnityEngine;

namespace WhaleTee.Factory {
  public interface IPrefabAsyncFactory<TOut> : IAsyncFactory<GameObject, TOut> { }
}