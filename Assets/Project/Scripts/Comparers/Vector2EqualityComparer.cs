using System.Collections.Generic;
using UnityEngine;
using WhaleTee.Extensions;

namespace WhaleTee.Comparers {
  public sealed class Vector2EqualityComparer : IEqualityComparer<Vector2> {
    readonly float tolerance;

    public Vector2EqualityComparer(float tolerance) {
      this.tolerance = tolerance;
    }

    public bool Equals(Vector2 x, Vector2 y) {
      return x.Approximately(y, tolerance);
    }

    public int GetHashCode(Vector2 obj) {
      return obj.GetHashCode();
    }
  }
}