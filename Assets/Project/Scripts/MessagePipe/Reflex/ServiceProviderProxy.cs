using System;
using Reflex.Core;

namespace WhaleTee.MessagePipe.Reflex {
  public class ServiceProviderProxy : IServiceProvider {
    readonly Container container;

    public ServiceProviderProxy(Container container) {
      this.container = container;
    }

    public object GetService(Type serviceType) {
      return container.Resolve(serviceType);
    }
  }
}