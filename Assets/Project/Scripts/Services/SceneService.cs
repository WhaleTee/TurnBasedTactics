using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneService {
  CancellationTokenSource cts;

  public SceneService() {
    cts = new CancellationTokenSource();
  }

  public async UniTaskVoid LoadScene(string sceneName, Action callback) {
    if (SceneManager.GetActiveScene().name == sceneName) callback.Invoke();

    var loadSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

    await UniTask.WaitUntil(() => loadSceneOperation == null || loadSceneOperation.isDone, PlayerLoopTiming.Update, cts.Token);

    callback.Invoke();
  }

  ~SceneService() {
    cts.Cancel();
  }
}