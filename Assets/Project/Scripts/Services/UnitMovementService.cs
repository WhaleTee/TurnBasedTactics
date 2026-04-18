using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ObservableCollections;
using R3;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using TurnBasedTactics.Abilities;
using TurnBasedTactics.Unit;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using WhaleTee.Extensions;
using WhaleTee.MessagePipe.Message;
using WhaleTee.Reactive.Input;
using ZLinq;

namespace WhaleTee.Grid {
  public class UnitMovementService : IDisposable {
    [Inject]
    ISubscriber<CharacterSelectedEventMessage> characterSelectedSubscriber;

    [Inject]
    UserInput userInput;

    [Inject]
    GroundTilemapToWorldPositionService tilemapToWorldPositionService;

    [Inject]
    HexGridNavigationService hexGridNavigationService;

    [Inject]
    UnitHexGridMovementService unitHexGridMovementService;

    public ObservableDictionary<int, HashSet<Vector3Int>> ReachablePositions { get; } = new();
    public ObservableList<Vector3Int> LastPath { get; } = new();
    IDisposable subscriptions;
    UnitBehaviour character;
    Vector3Int? target;

    public UnitMovementService() {
      var disposableBuilder = Disposable.CreateBuilder();

      userInput.PointerWorldPosition
               .Where(_ => character != null && !character.SquadUnit.state.movement.isMoving)
               .Subscribe(pointerPosition => {
                            var cellPosition = tilemapToWorldPositionService.GetCellPosition(pointerPosition);

                            if (target == null || target.Value != cellPosition) {
                              target = cellPosition;

                              FindPath(cellPosition);

                              if (LastPath.Count > character.SquadUnit.state.movement.availableSteps) ClearPath();
                            }
                          }
               )
               .AddTo(ref disposableBuilder);

      userInput.LeftClick
               .Where(value => value && character != null && !character.SquadUnit.state.movement.isMoving)
               .Subscribe(_ => UniTask.Void(MoveUnit))
               .AddTo(ref disposableBuilder);

      userInput.RightClick
               .Subscribe(_ => UnselectUnit())
               .AddTo(ref disposableBuilder);

      characterSelectedSubscriber.Subscribe(message => {
                                              if (character != null && character.SquadUnit.state.movement.isMoving) return;
                                              if (message.gameObject.OrNull() == null) return;

                                              var unit = message.gameObject.GetComponent<UnitBehaviour>();
                                              if (unit.OrNull() == null) return;

                                              character = unit;

                                              FindReachablePositions(
                                                tilemapToWorldPositionService.GetCellPosition(message.gameObject.transform.position)
                                              );
                                            }
                                 )
                                 .AddTo(ref disposableBuilder);

      subscriptions = disposableBuilder.Build();
    }

    void FindReachablePositions(Vector3Int startPosition) {
      ClearReachablePositions();
      var positions = new HashSet<Vector3Int>[character.SquadUnit.state.movement.availableSteps];

      hexGridNavigationService.FindReachablePositions(
        startPosition,
        positions
      );

      for (var i = 0; i < positions.Length; i++) ReachablePositions[i + 1] = new HashSet<Vector3Int>(positions[i]);
    }

    void UnselectUnit() {
      if (character == null) return;

      ClearReachablePositions();
      ClearPath();
      character.SquadUnit.state.movement.availableSteps = character.SquadUnit.configuration.movement.steps;
      character = null;
    }

    public bool IsReachable(Vector3Int position) {
      return ReachablePositions.AsValueEnumerable().SelectMany(pair => pair.Value).Contains(position);
    }

    async UniTaskVoid MoveUnit() {
      if (LastPath.Count <= 0) return;

      character.SquadUnit.state.movement.availableSteps -= LastPath.Count;
      character.GetComponent<Animator>().Play("Run");
      await unitHexGridMovementService.MoveUnitAlongGridPathAsync(character.SquadUnit, LastPath.ToList());
      character.GetComponent<Animator>().Play("Idle");
      FindReachablePositions(LastPath[^1]);
      ClearPath();
    }

    void ClearReachablePositions() {
      foreach (var key in ReachablePositions.AsValueEnumerable().Select(pair => pair.Key).ToArray()) {
        ReachablePositions.Remove(key);
      }

      ReachablePositions.Clear();
    }

    void FindPath(Vector3Int targetPosition) {
      ClearPath();
      var positions = ListPool<Vector3Int>.Get();

      hexGridNavigationService.FindPathPositions(
        tilemapToWorldPositionService.GetCellPosition(character.transform.position),
        targetPosition,
        false,
        positions
      );

      foreach (var position in positions) {
        LastPath.Add(position);
      }

      ListPool<Vector3Int>.Release(positions);
    }

    void ClearPath() {
      foreach (var position in LastPath.AsValueEnumerable().ToArray()) {
        LastPath.Remove(position);
      }

      LastPath.Clear();
    }

    public void Dispose() {
      subscriptions?.Dispose();
    }
  }
}