namespace WhaleTee.Factory {
  public interface IFactory<out TOut> {
    TOut Create();
  }
  
  public interface IFactory<in TIn, out TOut> {
    TOut Create(TIn ctx);
  }
}