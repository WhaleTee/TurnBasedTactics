using System;
using System.Collections.Generic;
using System.Threading;
using ObservableCollections;
using R3;
using Reflex.Attributes;
using TurnBasedTactics.DI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WhaleTee.Grid {
  public class UnitMovementVisualization : IInitializable, IDisposable {
    [Inject]
    NavigationTilemapContainer navigationTilemapContainer;

    readonly UnitMovementService unitMovementService;
    IDisposable subscriptions;
    CancellationTokenSource cts;

    public UnitMovementVisualization(UnitMovementService unitMovementService) {
      this.unitMovementService = unitMovementService;
    }

    public void Initialize() {
      var disposableBuilder = Disposable.CreateBuilder();
      cts = new CancellationTokenSource();

      unitMovementService.ReachablePositions.ObserveAdd(cts.Token)
                         .Select(e => e.Value.Value)
                         .Subscribe(DrawReachablePositions)
                         .AddTo(ref disposableBuilder);

      unitMovementService.ReachablePositions.ObserveRemove(cts.Token)
                         .Select(e => e.Value.Value)
                         .Subscribe(ClearReachablePositions)
                         .AddTo(ref disposableBuilder);

      unitMovementService.LastPath.ObserveAdd(cts.Token)
                         .Select(e => e.Value)
                         .Subscribe(DrawPath)
                         .AddTo(ref disposableBuilder);

      unitMovementService.LastPath.ObserveRemove(cts.Token)
                         .Select(e => e.Value)
                         .Subscribe(ClearPath)
                         .AddTo(ref disposableBuilder);

      subscriptions = disposableBuilder.Build();
    }

    public void DrawReachablePositions(HashSet<Vector3Int> positions) {
      foreach (var position in positions) {
        navigationTilemapContainer.Tilemap.SetTile(
          new TileChangeData(position, navigationTilemapContainer.NavigationTile, Color.white, Matrix4x4.identity),
          false
        );
      }
    }

    public void ClearReachablePositions(HashSet<Vector3Int> positions) {
      foreach (var position in positions) {
        navigationTilemapContainer.Tilemap.SetTile(new TileChangeData(position, null, Color.white, Matrix4x4.identity), false);
      }
    }

    public void DrawPath(Vector3Int position) {
      navigationTilemapContainer.Tilemap.SetTile(
        new TileChangeData(position, navigationTilemapContainer.StepTile, Color.white, Matrix4x4.identity),
        false
      );
    }

    public void ClearPath(Vector3Int position) {
      navigationTilemapContainer.Tilemap.SetTile(
        new TileChangeData(
          position,
          unitMovementService.IsReachable(position) ? navigationTilemapContainer.NavigationTile : null,
          Color.white,
          Matrix4x4.identity
        ),
        false
      );
    }

    public void Dispose() {
      cts?.Cancel();
      cts?.Dispose();
      subscriptions?.Dispose();
    }
  }
}