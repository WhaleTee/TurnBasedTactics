using UnityEngine;

namespace WhaleTee.Factory {
  public abstract class SimpleFactory<TIn, TOut> : IFactory<TIn, TOut> {
    public abstract TOut Create(TIn ctx);

    public TOut Create(TIn ctx, Vector3 position, Quaternion rotation) {
      return Create(ctx);
    }

    public TOut Create(TIn ctx, Transform parent, Quaternion rotation) {
      return Create(ctx);
    }

    public TOut Create(TIn ctx, Vector3 position, Quaternion rotation, Transform parent) {
      return Create(ctx);
    }
  }
}