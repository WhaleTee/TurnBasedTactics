using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reflex.Extensions;
using UnityEngine.SceneManagement;
using WhaleTee.Async;

namespace WhaleTee.Lifecycle {
  public class UpdatableRunner : AsyncLoopRunner {
    readonly IEnumerable<IUpdateable> updateables;

    public UpdatableRunner(IEnumerable<IUpdateable> updateables) {
      this.updateables = updateables;
    }

    protected override async UniTask Run(CancellationToken token = default) {
      await UniTask.Yield(PlayerLoopTiming.LastUpdate, token, true);
      
      foreach (var updateable in updateables) {
        updateable.Update();
      }
    }
  }
}