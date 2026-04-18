using UnityEngine.UIElements;

namespace WhaleTee.Input {
  public struct PointerEnterVisualElementEvent {
    public VisualElement element;

    public PointerEnterVisualElementEvent(VisualElement element) {
      this.element = element;
    }
  }

  public struct PointerExitVisualElementEvent {
    public VisualElement element;

    public PointerExitVisualElementEvent(VisualElement element) {
      this.element = element;
    }
  }
}