using Reflex.Core;
using TurnBasedTactics.Game;
using WhaleTee.Lifecycle;

namespace TurnBasedTactics.DI {
  public class ProjectContainerInstaller : ContainerInstaller<ProjectContainerInstaller> {
    protected override string ContainerName => "Root Container";

    static void RegisterGameDriver(ContainerBuilder containerBuilder) {
      containerBuilder.AddSingleton(typeof(GameDriver), typeof(IInitializable), typeof(IUpdateable), typeof(IFixedUpdateable));
    }

    public override void InstallBindings(ContainerBuilder containerBuilder) {
      base.InstallBindings(containerBuilder);
      RegisterGameDriver(containerBuilder);
    }
  }
}