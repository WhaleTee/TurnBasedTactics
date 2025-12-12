using System.Collections.Generic;
using UnityEngine;

namespace WhaleTee.Grid {
  public struct AStarNode {
    public Vector3Int position;
    public float g, h, f;
  }

  public class AStarNodeComparer : IComparer<AStarNode> {
    public int Compare(AStarNode x, AStarNode y) {
      var result = x.f.CompareTo(y.f);
      if (result == 0) result = x.h.CompareTo(y.h);
      return result;
    }
  }
}