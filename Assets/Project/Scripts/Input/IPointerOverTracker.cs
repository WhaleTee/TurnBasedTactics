namespace WhaleTee.Input {
  public interface IPointerOverTracker<in T> {
    void Track(T ui);
    void Untrack(T ui);
    bool IsTracked(T ui);
    bool IsPointerOverUI();
  }
}