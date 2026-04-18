using System.Threading;

namespace WhaleTee.Async {
  public interface ICancellationTokenProvider {
    CancellationToken GetToken();
  }
}