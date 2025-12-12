using WhaleTee.FSM;
using WhaleTee.Grid;
using WhaleTee.Reflex.Extensions;

public sealed class GameplayState : State {
  UnitMovementService unitMovementService;
  UnitMovementVisualization unitMovementVisualization;

  protected override void OnEnter() {
    // todo выбор способности и проверка работоспособности
    // unitMovementService = new UnitMovementService().InjectAttributes();
    // unitMovementVisualization = new UnitMovementVisualization(unitMovementService).InjectAttributes();
  }

  protected override void OnExit() {
    unitMovementService?.Dispose();
    unitMovementVisualization?.Dispose();
  }
}