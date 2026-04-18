using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;
using Reflex.Attributes;
using TurnBasedTactics.Unit;
using UnityEngine;
using WhaleTee.FSM;
using WhaleTee.Grid;
using WhaleTee.Reactive.Input;
using WhaleTee.Reflex.Extensions;
using ZLinq;

public sealed class DeployState : State {
  readonly StateMachine stateMachine;
  bool isFinished;

  public DeployState() {
    stateMachine = StateMachine.StateMachineBuilder.Create()
                               .AddInitialState(new EnemyEmergeState())
                               .AddState(new SquadDeploymentState())
                               .AddState(new EnemyMoveState())
                               .AddState(new SquadActionState())
                               .AddState(new EndTurnState())
                               .Build();
  }

  void OnStateChanged(Type oldState, Type newState) {
    if (newState == typeof(EndTurnState)) isFinished = true;
  }

  protected override Type GetTransition() {
    return isFinished ? typeof(GameplayState) : null;
  }

  protected override void OnEnter() {
    stateMachine.stateChangedEvent += OnStateChanged;
    isFinished = false;
  }

  protected override void OnUpdate() {
    stateMachine.Update();
  }

  protected override void OnFixedUpdate() {
    stateMachine.FixedUpdate();
  }

  protected override void OnExit() {
    stateMachine.stateChangedEvent -= OnStateChanged;
  }
}

public sealed class EnemyEmergeState : State {
  protected override Type GetTransition() {
    return typeof(SquadDeploymentState);
  }
}

public sealed class SquadDeploymentState : State {
  [Inject]
  UserInput userInput;

  [Inject]
  HexGridNavigationService navigationService;

  [Inject]
  NavigationTilemapContainer navigationTilemapContainer;

  [Inject]
  GroundTilemapToWorldPositionService toWorldPositionService;

  [Inject]
  SquadData squad;

  [Inject]
  UnitSpawnService unitSpawnService;

  [Inject]
  UnitHexGridMovementService hexGridMovementService;

  UnitDeployPositionsVisualization unitDeployVisualization;
  readonly ObservableDictionary<int, HashSet<Vector3Int>> reachablePositions = new();
  readonly Vector3Int[] squadDeployPositions = new Vector3Int[3];
  IDisposable subscriptions;
  bool confirmButtonIsClicked; // todo subscribe to real button
  SquadUnit deployUnit;
  Vector3Int deployCell;

  protected override Type GetTransition() {
    var all = Array.TrueForAll(squadDeployPositions, v => v != squad.deployInitialPoint) && confirmButtonIsClicked;
    if (all) squad.firstUnit.state.abilities[0].Target();
    return all ? typeof(EnemyMoveState) : null;
  }

  protected override void OnEnter() {
    for (var i = 0; i < squadDeployPositions.Length; i++) squadDeployPositions[i] = squad.deployInitialPoint;

    unitDeployVisualization = new UnitDeployPositionsVisualization(reachablePositions).Inject();

    var bag = Disposable.CreateBuilder();

    userInput.LeftClick
             .Where(value => value)
             .Subscribe(_ => {
                          var cellPosition = toWorldPositionService.GetCellPosition(userInput.PointerWorldPosition.Value);

                          if (deployCell != squad.deployInitialPoint && deployUnit != null && deployCell == cellPosition)
                            for (var i = 0; i < squadDeployPositions.Length; i++) {
                              if (squadDeployPositions[i] != squad.deployInitialPoint) continue;

                              squadDeployPositions[i] = cellPosition;
                              hexGridMovementService.SetPosition(deployUnit, toWorldPositionService.GetWorldPosition(cellPosition));

                              switch (i) {
                                case 0:
                                  unitSpawnService.SpawnUnit(squad.secondUnit, toWorldPositionService.GetWorldPosition(cellPosition));
                                  deployUnit = squad.secondUnit;
                                  break;
                                case 1:
                                  unitSpawnService.SpawnUnit(squad.thirdUnit, toWorldPositionService.GetWorldPosition(cellPosition));
                                  deployUnit = squad.thirdUnit;
                                  break;
                                case 2:
                                  confirmButtonIsClicked = true;
                                  break;
                              }

                              break;
                            }
                        }
             )
             .AddTo(ref bag);

    userInput.PointerWorldPosition
             .Subscribe(position => {
                          var cellPosition = toWorldPositionService.GetCellPosition(position);

                          if (!reachablePositions.AsValueEnumerable().SelectMany(p => p.Value).Contains(cellPosition)) return;

                          if (cellPosition != deployCell) {
                            deployCell = cellPosition;

                            hexGridMovementService.SetPosition(
                              deployUnit,
                              toWorldPositionService.GetWorldPosition(cellPosition) + deployUnit.configuration.position.deployCellOffset
                            );
                          }
                        }
             )
             .AddTo(ref bag);

    subscriptions = bag.Build();
    FindReachablePositions();
    unitSpawnService.SpawnUnit(squad.firstUnit, toWorldPositionService.GetWorldPosition(squad.deployInitialPoint));
    deployUnit = squad.firstUnit;
    deployCell = squad.deployInitialPoint;
  }

  protected override void OnExit() {
    ClearReachablePositions();
    unitDeployVisualization?.Dispose();
    subscriptions?.Dispose();
  }

  void FindReachablePositions() {
    ClearReachablePositions();
    var positions = new HashSet<Vector3Int>[squad.maxDeployDistance];

    navigationService.FindReachablePositions(squad.deployInitialPoint, positions);
    positions[0].Add(squad.deployInitialPoint);

    for (var i = 0; i < positions.Length; i++) reachablePositions[i + 1] = new HashSet<Vector3Int>(positions[i]);
  }

  void ClearReachablePositions() {
    foreach (var key in reachablePositions.AsValueEnumerable().Select(pair => pair.Key).ToArray()) {
      reachablePositions.Remove(key);
    }

    reachablePositions.Clear();
  }
}

public sealed class EnemyMoveState : State {
  protected override Type GetTransition() {
    return typeof(SquadActionState);
  }
}

public sealed class EnemyActionState : State { }

public sealed class SquadActionState : State {
  protected override Type GetTransition() {
    return typeof(EndTurnState);
  }
}

public sealed class EndTurnState : ParametrizedState<Type> {
  public EndTurnState(Type parameter = null) : base(parameter) { }

  protected override Type GetTransition() {
    return parameter;
  }
}

public sealed class NewTurnState : State { }