using Project.Scripts.Miscellaneous;
using Reflex.Core;

namespace TurnBasedTactics.DI {
  public abstract class ContainerInstaller<T> : Singleton<T>, IInstaller where T : ContainerInstaller<T> {
    protected Container container;
    
    protected abstract string ContainerName { get; }

    static void Initialize(Container container) {
      foreach (var initializable in container.All<IInitializable>()) {
        initializable.Initialize();
      }
    }

    void OnContainerBuilt(Container container) {
      this.container = container;
    }
    
    void Start() {
      Initialize(container);
    }

    public virtual void InstallBindings(ContainerBuilder containerBuilder) {
      containerBuilder.SetName(ContainerName);
      containerBuilder.OnContainerBuilt += OnContainerBuilt;
    }
  }
}