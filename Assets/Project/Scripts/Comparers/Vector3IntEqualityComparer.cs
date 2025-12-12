using System.Collections.Generic;
using UnityEngine;

namespace WhaleTee.Comparers {
  public sealed class Vector3IntEqualityComparer : IEqualityComparer<Vector3Int> {
    public bool Equals(Vector3Int x, Vector3Int y) {
      return x.Equals(y);
    }

    public int GetHashCode(Vector3Int obj) {
      return obj.GetHashCode();
    }
  }
}