using UnityEngine.UIElements;
using WhaleTee.MessagePipe.Message;

namespace WhaleTee.Input {
  public struct PointerEnterVisualElementEvent : IEventMessage {
    public VisualElement element;

    public PointerEnterVisualElementEvent(VisualElement element) {
      this.element = element;
    }
  }

  public struct PointerExitVisualElementEvent : IEventMessage {
    public VisualElement element;

    public PointerExitVisualElementEvent(VisualElement element) {
      this.element = element;
    }
  }
}