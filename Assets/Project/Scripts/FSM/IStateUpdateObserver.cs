namespace WhaleTee.FSM {
  public interface IStateUpdateObserver<T> : IStateLifecycleObserver<T> where T : State {
    void OnUpdate();
  }
}