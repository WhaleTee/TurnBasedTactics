using UnityEngine;
using UnityEngine.UIElements;

namespace TurnBasedTactics.UI {
  public sealed class MainMenuUI : MonoBehaviour {
    UIDocument document;
    VisualElement root;
    VisualElement background;
    VisualElement pressAnyKeyLabel;
    VisualElement gameNameLabel;

    [field: SerializeField] public MainMenuButtons MainMenuButtons { get; private set; }
    [field: SerializeField] public MainMenuNewGamePanel NewGamePanel { get; private set; }

    void Awake() {
      document = GetComponent<UIDocument>();
      root = document.rootVisualElement;
      background = root.Q<VisualElement>("Background");
      pressAnyKeyLabel = root.Q<Label>("PressAnyKeyLabel");
      gameNameLabel = root.Q<Label>("GameNameLabel");
      MainMenuButtons.Setup(root);
      NewGamePanel.Setup(root);
    }

    public void ShowBackground() {
      background.RemoveFromClassList("background--hidden");
    }

    public void HideBackground() {
      background.AddToClassList("background--hidden");
    }

    public void ShowPressAnyKeyLabel() {
      pressAnyKeyLabel.RemoveFromClassList("press-any-key-label--hidden");
    }

    public void HidePressAnyKeyLabel() {
      pressAnyKeyLabel.AddToClassList("press-any-key-label--hidden");
    }

    public void ShowGameNameLabel() {
      gameNameLabel.AddToClassList("game-name-label--shown");
      gameNameLabel.RemoveFromClassList("game-name-label--hidden");
    }

    public void HideGameNameLabel() {
      gameNameLabel.AddToClassList("game-name-label--hidden");
      gameNameLabel.RemoveFromClassList("game-name-label--shown");
    }
  }
}