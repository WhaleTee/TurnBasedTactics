using UnityEngine;

namespace Project.Scripts.Miscellaneous {
  public class Singleton<T> : MonoBehaviour where T : Component {
    static T instance;

    public static bool HasInstance => instance != null;

    public static void TryGetInstance(out T i) {
      i = HasInstance ? instance : null;
    }

    public static T Instance {
      get {
        if (instance != null) return instance;

        instance = FindAnyObjectByType<T>();
        if (instance != null) return instance;

        var go = new GameObject(typeof(T).Name + " Generated");
        instance = go.AddComponent<T>();
        return instance;
      }
    }

    protected virtual void Awake() {
      InitializeSingleton();
    }

    protected virtual void InitializeSingleton() {
      if (!Application.isPlaying) return;

      if (instance == null) {
        instance = this as T;
        DontDestroyOnLoad(gameObject);
      } else {
        if (instance != this) {
          Destroy(gameObject);
        }
      }
    }
  }
}