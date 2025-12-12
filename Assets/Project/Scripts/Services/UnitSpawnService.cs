using MessagePipe;
using Reflex.Attributes;
using TurnBasedTactics.Unit;
using UnityEngine;
using WhaleTee.Grid;
using WhaleTee.MessagePipe.Message;
using WhaleTee.Pooling;

public class UnitSpawnService {
  [Inject] ObjectPool objectPool;
  [Inject] IPublisher<UnitSpawnedEventMessage> unitSpawnedEvent;
  [Inject] UnitHexGridMovementService unitHexGridMovementService;

  public void SpawnUnit(SquadUnit unit, Vector2 position) {
    unit.InjectDependencies();
    var prefab = unit.configuration.prefab;
    var gameObject = objectPool.SpawnObject(prefab, position, Quaternion.identity);
    gameObject.AddComponent<UnitBehaviour>().SquadUnit = unit;
    unit.state.gameObject = gameObject;
    unitHexGridMovementService.SetPosition(unit, position);
    unitSpawnedEvent.Publish(new UnitSpawnedEventMessage { gameObject = gameObject });
    unit.state.movement.canMove = true;
    foreach (var ability in unit.state.abilities) ability.owner = gameObject;
  }
}