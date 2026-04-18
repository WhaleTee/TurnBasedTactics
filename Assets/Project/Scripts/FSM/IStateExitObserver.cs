namespace WhaleTee.FSM {
  public interface IStateExitObserver<T> : IStateLifecycleObserver<T> where T : State {
    void OnExit();
  }
}