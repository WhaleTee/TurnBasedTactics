using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using ZLinq;

namespace TurnBasedTactics.UI {
  [Serializable]
  public sealed class MainMenuButtons {
    [SerializeField]
    [Range(0, 1)]
    float buttonAppearanceDelay;

    [SerializeField]
    [Range(0, 1)]
    float buttonFadeDelay;

    VisualElement root;
    Button[] buttons;
    Button continueButton;
    Button newGameButton;
    Button optionsButton;
    Button creditsButton;
    Button quitButton;
    Button profileButton;
    Button achievementsButton;
    Button statisticsButton;

    CancellationTokenSource buttonAppearanceTokenSource;

    void SetupButtons() {
      buttons = root.Query("H1").ToList().AsValueEnumerable().SelectMany(h => h.Query<Button>().ToList()).ToArray();
      continueButton = buttons[0];
      newGameButton = buttons[1];
      optionsButton = buttons[2];
      creditsButton = buttons[3];
      quitButton = buttons[4];
      profileButton = buttons[5];
      achievementsButton = buttons[6];
      statisticsButton = buttons[7];
    }

    void RefreshButtonAppearanceTokenSource() {
      buttonAppearanceTokenSource?.Cancel();
      buttonAppearanceTokenSource?.Dispose();
      buttonAppearanceTokenSource = new CancellationTokenSource();
    }

    public void Setup(VisualElement root) {
      this.root = root;
      SetupButtons();
    }

    public async UniTask ShowButtons() {
      RefreshButtonAppearanceTokenSource();

      foreach (var button in buttons) {
        button.RemoveFromClassList("main-menu-button--hidden");
        await UniTask.WaitForSeconds(buttonAppearanceDelay, cancellationToken: buttonAppearanceTokenSource.Token, cancelImmediately: true);
      }
    }

    public async UniTask HideButtons() {
      RefreshButtonAppearanceTokenSource();

      foreach (var button in buttons) {
        button.AddToClassList("main-menu-button--hidden");
        await UniTask.WaitForSeconds(buttonFadeDelay, cancellationToken: buttonAppearanceTokenSource.Token, cancelImmediately: true);
      }
    }

    public void RegisterNewGameButtonClickedCallback(EventCallback<PointerDownEvent> action) {
      newGameButton.RegisterCallback(action, TrickleDown.TrickleDown);
    }

    public void UnregisterNewGameButtonClickedCallback(EventCallback<PointerDownEvent> action) {
      newGameButton.UnregisterCallback(action, TrickleDown.TrickleDown);
    }
  }
}