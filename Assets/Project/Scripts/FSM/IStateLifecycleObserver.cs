using System;
using WhaleTee.Async;

namespace WhaleTee.FSM {
  public interface IStateLifecycleObserver<T> where T : State {
    Type GetObservableState() {
      return typeof(T);
    }
  }
}