using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using TurnBasedTactics.DI;
using UnityEngine.UIElements;
using WhaleTee.FSM;

namespace TurnBasedTactics.UI {
  public sealed class MainMenuStateUIProcessor : IStateEnterObserver<MainMenuState>, IStateExitObserver<MainMenuState>, IInitializable, IDisposable {
    [Inject]
    readonly StateObserveManager stateObserveManager;

    [Inject]
    readonly MainMenuUI mainMenuUI;

    readonly CancellationTokenSource cts = new();

    void OnNewGameButtonClicked(PointerDownEvent evt) {
      OpenNewGameMenu().Forget();
    }

    void OnNewGamePanelStartButtonClicked(PointerDownEvent evt) {
      mainMenuUI.NewGamePanel.Hide();
    }

    void OnNewGamePanelBackButtonClicked(PointerDownEvent evt) {
      mainMenuUI.NewGamePanel.Hide();
      ShowMainMenu();
    }

    async UniTask OpenNewGameMenu() {
      await HideMainMenu();
      mainMenuUI.NewGamePanel.Show();
    }

    void ShowMainMenu() {
      mainMenuUI.ShowBackground();
      mainMenuUI.ShowGameNameLabel();
      ShowMainMenuButtons();
    }

    async UniTask HideMainMenu() {
      await mainMenuUI.MainMenuButtons.HideButtons();
      mainMenuUI.HideBackground();
      mainMenuUI.HideGameNameLabel();
    }

    void ShowMainMenuButtons() {
      mainMenuUI.MainMenuButtons.ShowButtons().Forget();
    }

    public void OnEnter() {
      ShowMainMenuButtons();
      mainMenuUI.MainMenuButtons.RegisterNewGameButtonClickedCallback(OnNewGameButtonClicked);
      mainMenuUI.NewGamePanel.RegisterStartButtonCallback(OnNewGamePanelStartButtonClicked);
      mainMenuUI.NewGamePanel.RegisterBackButtonCallback(OnNewGamePanelBackButtonClicked);
    }

    public void OnExit() {
      mainMenuUI.MainMenuButtons.UnregisterNewGameButtonClickedCallback(OnNewGameButtonClicked);
      mainMenuUI.NewGamePanel.UnregisterStartButtonCallback(OnNewGamePanelStartButtonClicked);
      mainMenuUI.NewGamePanel.UnregisterBackButtonCallback(OnNewGamePanelBackButtonClicked);
    }

    public void Initialize() {
      stateObserveManager.RegisterStateEnterObserver(this, cts.Token);
      stateObserveManager.RegisterStateExitObserver(this, cts.Token);
    }

    public void Dispose() {
      cts?.Cancel();
      cts?.Dispose();
    }
  }
}