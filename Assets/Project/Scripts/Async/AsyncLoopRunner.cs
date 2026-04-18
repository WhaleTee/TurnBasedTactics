using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TurnBasedTactics.DI;

namespace WhaleTee.Async {
  public abstract class AsyncLoopRunner : ICancellationTokenProvider, IInitializable, IDisposable {
    CancellationTokenSource cts;
    
    void CreateTokenSource() {
      cts = new CancellationTokenSource();
    }
    
    void CancelToken() {
      cts?.Cancel();
    }

    void DisposeTokenSource() {
      cts?.Dispose();
    }
    
    async UniTaskVoid RunInternal(CancellationToken token) {
      while (!token.IsCancellationRequested) await Run(token);
    }

    protected abstract UniTask Run(CancellationToken token = default);

    public CancellationToken GetToken() {
      return cts.Token;
    }

    public virtual void Initialize() {
      CancelToken();
      DisposeTokenSource();
      CreateTokenSource();
      UniTask.Void(() => RunInternal(GetToken()));
    }

    public void Dispose() {
      CancelToken();
      DisposeTokenSource();
    }
  }
}