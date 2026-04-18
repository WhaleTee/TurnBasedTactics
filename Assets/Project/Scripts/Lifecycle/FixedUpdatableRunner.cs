using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reflex.Extensions;
using UnityEngine.SceneManagement;
using WhaleTee.Async;

namespace WhaleTee.Lifecycle {
  public class FixedUpdatableRunner : AsyncLoopRunner {
    readonly IEnumerable<IFixedUpdateable> updateables;

    public FixedUpdatableRunner(IEnumerable<IFixedUpdateable> updateables) {
      this.updateables = updateables;
    }

    protected override async UniTask Run(CancellationToken token = default) {
      await UniTask.WaitForFixedUpdate(token, true);
      
      foreach (var updateable in updateables) {
        updateable.FixedUpdate();
      }
    }
  }
}