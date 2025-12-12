using WhaleTee.FSM;

public abstract class ParametrizedState<T> : State {
  protected T parameter;

  protected ParametrizedState(T parameter) {
    this.parameter = parameter;
  }
}