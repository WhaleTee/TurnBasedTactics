using UnityEngine;
using WhaleTee.Factory.Context;
using Object = UnityEngine.Object;

namespace WhaleTee.Factory {
  public class GameObjectFactory : IDeactivatedGameObjectFactory {
    public GameObject Create(PrefabContext ctx) {
      if (ctx.rotation == default) ctx.rotation = Quaternion.identity;
      var go = Object.Instantiate(ctx.prefab, ctx.position, ctx.rotation, ctx.parent);
      go.name = ctx.prefab.name;
      go.transform.localScale = Vector3.one;
      return go;
    }
  }
}