using UnityEngine;
using UnityEngine.Pool;

public static class Raycast2DTool {
  static ContactFilter2D GetFilter(LayerMask layers, float minDepth, float maxDepth) {
    var filter = new ContactFilter2D();
    filter.SetLayerMask(layers);
    filter.SetDepth(minDepth, maxDepth);
    return filter;
  }

  public static GameObject OverlapOne(Vector2 point, LayerMask layers, float minDepth, float maxDepth) {
    ListPool<Collider2D>.Get(out var results);
    var overlapObject = Physics2D.OverlapPoint(point, GetFilter(layers, minDepth, maxDepth), results) > 0 ? results[0].gameObject : null;
    ListPool<Collider2D>.Release(results);
    return overlapObject;
  }
}