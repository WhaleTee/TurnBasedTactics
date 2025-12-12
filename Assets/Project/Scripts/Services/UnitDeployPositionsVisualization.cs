using System;
using System.Collections.Generic;
using System.Threading;
using ObservableCollections;
using R3;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WhaleTee.Grid {
  public class UnitDeployPositionsVisualization : IDisposable {
    [Inject] NavigationTilemapContainer navigationTilemapContainer;
    IDisposable subscriptions;
    CancellationTokenSource cts;

    public UnitDeployPositionsVisualization(ObservableDictionary<int, HashSet<Vector3Int>> reachablePositions) {
      var disposableBuilder = Disposable.CreateBuilder();
      cts = new CancellationTokenSource();

      reachablePositions.ObserveAdd(cts.Token)
                        .Select(e => e.Value.Value)
                        .Subscribe(DrawReachablePositions)
                        .AddTo(ref disposableBuilder);

      reachablePositions.ObserveRemove(cts.Token)
                        .Select(e => e.Value.Value)
                        .Subscribe(ClearReachablePositions)
                        .AddTo(ref disposableBuilder);

      subscriptions = disposableBuilder.Build();
    }

    public void DrawReachablePositions(HashSet<Vector3Int> positions) {
      foreach (var position in positions)
        navigationTilemapContainer.Tilemap.SetTile(
          new TileChangeData(position, navigationTilemapContainer.NavigationTile, Color.white, Matrix4x4.identity),
          false
        );
    }

    public void ClearReachablePositions(HashSet<Vector3Int> positions) {
      foreach (var position in positions)
        navigationTilemapContainer.Tilemap.SetTile(new TileChangeData(position, null, Color.white, Matrix4x4.identity), false);
    }

    public void Dispose() {
      cts?.Cancel();
      cts?.Dispose();
      subscriptions?.Dispose();
    }
  }
}