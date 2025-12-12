using WhaleTee.Factory.Context;

namespace WhaleTee.Factory {
  public interface IPrefabFactory<out TOut> : IFactory<PrefabContext, TOut> { }
}