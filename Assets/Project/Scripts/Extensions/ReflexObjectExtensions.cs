using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine.SceneManagement;

namespace WhaleTee.Reflex.Extensions {
  public static class ReflexObjectExtensions {
    public static T InjectAttributes<T>(this T obj) {
      AttributeInjector.Inject(obj, SceneManager.GetActiveScene().GetSceneContainer());
      return obj;
    }

    public static T Construct<T>(this Container container) where T : class {
      var obj = ConstructorInjector.Construct(typeof(T), container).InjectAttributes();
      return obj as T;
    }
  }
}