using System.Collections.Generic;
using MessagePipe;
using Reflex.Attributes;
using UnityEngine;
using WhaleTee.MessagePipe.Message;
using ZLinq;

namespace WhaleTee.Grid {
  public class HexGridNavigationService {
    [Inject]
    readonly ISubscriber<CellPlacedEventMessage> subscriber;

    readonly HashSet<Vector3Int> blockedCells = new();
    readonly Dictionary<Vector3Int, int> cellsCosts = new();

    public HexGridNavigationService(ISubscriber<CellPlacedEventMessage> subscriber) {
      this.subscriber = subscriber;
      SubscribeEvents();
    }

    static readonly Vector3Int[] directionsOdd = {
      new(0, -1, 0),
      new(-1, 0, 0),
      new(0, 1, 0),
      new(1, 1, 0),
      new(1, 0, 0),
      new(1, -1, 0)
    };

    static readonly Vector3Int[] directionsEven = {
      new(-1, -1, 0),
      new(-1, 0, 0),
      new(-1, 1, 0),
      new(0, 1, 0),
      new(1, 0, 0),
      new(0, -1, 0)
    };

    static IEnumerable<Vector3Int> GetNeighbors(Vector3Int position) {
      return (IsEven(position.y) ? directionsEven : directionsOdd)
             .AsValueEnumerable()
             .Select(direction => position + direction)
             .ToArray();
    }

    static float Heuristic(Vector3Int from, Vector3Int to) {
      return (from - to).magnitude;
    }

    static bool IsEven(int value) {
      return (value & 1) == 0;
    }

    void SubscribeEvents() {
      subscriber.Subscribe(OnCellPlaced);
    }

    void OnCellPlaced(CellPlacedEventMessage message) {
      if (message.blockingMovement) blockedCells.Add(message.cellPosition);
      else blockedCells.Remove(message.cellPosition);

      cellsCosts[message.cellPosition] = message.movementCost;
    }

    public void FindReachablePositions(Vector3Int start, HashSet<Vector3Int>[] stepPositions) {
      // BFS
      var visited = new HashSet<Vector3Int>();

      for (var step = 0; step < stepPositions.Length; step++)
        foreach (var neighbor in step == 0 ? GetNeighbors(start) : stepPositions[step - 1].AsValueEnumerable().SelectMany(GetNeighbors).ToArray()) {
          if (blockedCells.Contains(neighbor) || neighbor == start) continue;

          stepPositions[step] ??= new HashSet<Vector3Int>();
          if (!visited.Contains(neighbor)) stepPositions[step].Add(neighbor);
          visited.Add(neighbor);
        }
    }

    public void FindPathPositions(Vector3Int start, Vector3Int target, bool applyCost, List<Vector3Int> path) {
      path.Clear();
      // A*
      if (start == target || blockedCells.Contains(target)) return;

      var open = new SortedSet<AStarNode>(new AStarNodeComparer());
      var closed = new HashSet<Vector3Int>();
      var toFrom = new Dictionary<Vector3Int, Vector3Int>();

      open.Add(new AStarNode { position = start });

      while (open.Count > 0) {
        var cell = open.Min;
        open.Remove(cell);
        closed.Add(cell.position);

        if (cell.position == target) {
          path.Add(cell.position);

          for (var position = cell.position; toFrom.TryGetValue(position, out var from) && from != start; position = from) path.Add(from);

          path.Reverse();

          return;
        }

        foreach (var neighbor in GetNeighbors(cell.position)) {
          if (closed.Contains(neighbor) || blockedCells.Contains(neighbor)) continue;

          var neighborCell = open.AsValueEnumerable()
                                 .FirstOrDefault(o => o.position == neighbor, new AStarNode { position = neighbor });

          var g = neighborCell.g + Heuristic(start, neighbor) + (applyCost ? cellsCosts.GetValueOrDefault(neighbor) : 0);

          if (!open.Contains(neighborCell) || g < neighborCell.g) {
            open.Remove(neighborCell);
            neighborCell.g = g;
            neighborCell.h = Heuristic(neighbor, target);
            neighborCell.f = neighborCell.g + neighborCell.h;
            open.Add(neighborCell);
            toFrom[neighbor] = cell.position;
          }
        }
      }
    }

    public int Distance(Vector3Int from, Vector3Int to) {
      var penalty = (IsEven(from.y) && !IsEven(to.y) && from.x < to.x) || (IsEven(to.y) && !IsEven(from.y) && to.x < from.x) ? 1 : 0;
      var dy = Mathf.Abs(from.y - to.y);
      var dx = Mathf.Abs(from.x - to.x);
      return Mathf.Max(dy, dx + dy / 2 + penalty);
    }

    public Vector3Int FindNeighbor(Vector3Int cell, Vector3 direction, int depth = 1) {
      return Vector3Int.zero;
    }

    public Vector3Int PushCells(Vector3Int cell, Vector3Int target, int depth = 1) {
      var result = target;
      Vector3 direction = target - cell;
      direction.Normalize();
      Debug.Log(direction);

      for (var i = 0; i <= depth; i++) {
        var nextPosition = target + direction * i;
        Debug.Log(nextPosition);
        result = GetNeighbors(result).AsValueEnumerable().MinBy(n => Mathf.Abs(Vector3.Distance(n, nextPosition)));
      }

      return result;
    }

    public bool IsCellBlocked(Vector3Int cell) {
      return blockedCells.Contains(cell);
    }
  }
}