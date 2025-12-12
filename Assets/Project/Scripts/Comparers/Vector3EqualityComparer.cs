using System.Collections.Generic;
using UnityEngine;
using WhaleTee.Extensions;

namespace WhaleTee.Comparers {
  public sealed class Vector3EqualityComparer : IEqualityComparer<Vector3> {
    readonly float tolerance;

    public Vector3EqualityComparer(float tolerance) {
      this.tolerance = tolerance;
    }

    public bool Equals(Vector3 x, Vector3 y) {
      return x.Approximately(y, tolerance);
    }

    public int GetHashCode(Vector3 obj) {
      return obj.GetHashCode();
    }
  }
}