using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WhaleTee.Factory {
  public interface IAsyncFactory<in TIn, TOut> {
    UniTask<TOut> Instantiate(TIn ctx);
    UniTask<TOut> Instantiate(TIn ctx, Vector3 position, Quaternion rotation);
    UniTask<TOut> Instantiate(TIn ctx, Transform parent, Quaternion rotation);
    UniTask<TOut> Instantiate(TIn ctx, Vector3 position, Quaternion rotation, Transform parent);
  }
}