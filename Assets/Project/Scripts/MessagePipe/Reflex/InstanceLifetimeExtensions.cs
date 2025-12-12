using MessagePipe;
using Reflex.Enums;

namespace WhaleTee.MessagePipe.Reflex {
  public static class InstanceLifetimeExtensions {
    public static Lifetime AsLifetime(this InstanceLifetime lifetime) {
      return lifetime switch {
               InstanceLifetime.Scoped => Lifetime.Scoped, InstanceLifetime.Singleton => Lifetime.Singleton, var _ => Lifetime.Transient
             };
    }
  }
}