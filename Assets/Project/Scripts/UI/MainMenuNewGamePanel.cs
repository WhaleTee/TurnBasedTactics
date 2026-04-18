using System;
using UnityEngine.UIElements;

namespace TurnBasedTactics.UI {
  [Serializable]
  public sealed class MainMenuNewGamePanel {
    VisualElement root;
    Button startButton;
    Button backButton;

    public void Setup(VisualElement root) {
      this.root = root.Q<VisualElement>("NewGamePanel");
      startButton = root.Q<Button>("Start");
      backButton = root.Q<Button>("Back");
    }

    public void Show() {
      root.visible = true;
      root.pickingMode = PickingMode.Position;
    }

    public void Hide() {
      root.visible = false;
      root.pickingMode = PickingMode.Ignore;
    }

    public void RegisterStartButtonCallback(EventCallback<PointerDownEvent> action) {
      startButton.RegisterCallback(action, TrickleDown.TrickleDown);
    }

    public void UnregisterStartButtonCallback(EventCallback<PointerDownEvent> action) {
      startButton.UnregisterCallback(action, TrickleDown.TrickleDown);
    }

    public void RegisterBackButtonCallback(EventCallback<PointerDownEvent> action) {
      backButton.RegisterCallback(action, TrickleDown.TrickleDown);
    }

    public void UnregisterBackButtonCallback(EventCallback<PointerDownEvent> action) {
      backButton.UnregisterCallback(action, TrickleDown.TrickleDown);
    }
  }
}