namespace WhaleTee.FSM {
  public interface IStateEnterObserver<T> : IStateLifecycleObserver<T> where T : State {
    void OnEnter();
  }
}