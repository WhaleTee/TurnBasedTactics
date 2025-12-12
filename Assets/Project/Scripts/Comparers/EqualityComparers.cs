using UnityEngine;

namespace WhaleTee.Comparers {
  public static class EqualityComparers {
    public static Vector2EqualityComparer Vector2(float tolerance) {
      return new Vector2EqualityComparer(Mathf.Abs(tolerance));
    }

    public static Vector3EqualityComparer Vector3(float tolerance) {
      return new Vector3EqualityComparer(Mathf.Abs(tolerance));
    }

    public static Vector3IntEqualityComparer Vector3Int() {
      return new Vector3IntEqualityComparer();
    }
  }
}