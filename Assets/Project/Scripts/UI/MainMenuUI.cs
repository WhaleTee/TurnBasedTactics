using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UIElements;
using WhaleTee.Extensions;
using ZLinq;

public class MainMenuUI : MonoBehaviour {
  [SerializeField] [Range(0, 5)] float menuAppearanceDelay;
  [SerializeField] [Range(0, 1)] float buttonAppearanceDelay;
  [SerializeField] [Range(0, 1)] float buttonFadeDelay;
  [SerializeField] [Range(0, 1)] float backgroundAppearanceDelay;
  [SerializeField] [Range(0, 1)] float labelShowTime;
  [SerializeField] [Range(0, 1)] float newGamePanelShowTime;
  UIDocument document;
  VisualElement root;
  VisualElement background;
  VisualElement newGamePanel;
  Button continueButton;
  Button newGameButton;
  Button optionsButton;
  Button creditsButton;
  Button quitButton;
  Button profileButton;
  Button achievementsButton;
  Button statisticsButton;
  Button[] buttons;
  EventCallback<PointerOverEvent>[] buttonPointerOverHandlers;
  EventCallback<PointerOutEvent>[] buttonPointerOutHandlers;
  EventCallback<PointerDownEvent>[] buttonPointerDownHandlers;

  void Awake() {
    document = GetComponent<UIDocument>();
    root = document.rootVisualElement;
    background = root.Q<VisualElement>("Background");
    newGamePanel = root.Q<VisualElement>("NewGamePanel");
    SetupButtonsOnAwake();
  }

  void Start() {
    RegisterButtonCallbacksInternal();
  }

  void SetupButtonsOnAwake() {
    buttons = root.Query("H1").ToList().AsValueEnumerable().SelectMany(h => h.Query<Button>().ToList()).ToArray();
    buttonPointerOverHandlers = new EventCallback<PointerOverEvent>[buttons.Length];
    buttonPointerOutHandlers = new EventCallback<PointerOutEvent>[buttons.Length];
    buttonPointerDownHandlers = new EventCallback<PointerDownEvent>[buttons.Length];
    continueButton = buttons[0];
    newGameButton = buttons[1];
    optionsButton = buttons[2];
    creditsButton = buttons[3];
    quitButton = buttons[4];
    profileButton = buttons[7];
    achievementsButton = buttons[6];
    statisticsButton = buttons[5];
    buttons[5] = profileButton;
    buttons[6] = achievementsButton;
    buttons[7] = statisticsButton;
  }

  public async UniTask SetupButtons() {
    for (var i = 0; i < buttons.Length; i++) {
      var button = buttons[i];
      button.RemoveFromClassList("main-menu-button--hidden");
      buttonPointerOverHandlers[i] = _ => OnButtonHover(button);
      buttonPointerOutHandlers[i] = _ => OnButtonOut(button);
      button.RegisterCallback(buttonPointerOverHandlers[i], TrickleDown.TrickleDown);
      button.RegisterCallback(buttonPointerOutHandlers[i], TrickleDown.TrickleDown);
      await UniTask.WaitForSeconds(buttonAppearanceDelay);
    }
  }

  async UniTask ResetButtons() {
    for (var i = 0; i < buttons.Length; i++) {
      var button = buttons[i];
      OnButtonOut(button);
      button.UnregisterCallback(buttonPointerOverHandlers[i], TrickleDown.TrickleDown);
      button.UnregisterCallback(buttonPointerOutHandlers[i], TrickleDown.TrickleDown);
      buttonPointerOverHandlers[i] = null;
      buttonPointerOutHandlers[i] = null;
      button.AddToClassList("main-menu-button--hidden");
      await UniTask.WaitForSeconds(buttonAppearanceDelay);
    }
  }

  void RegisterButtonCallbacksInternal() {
    RegisterNewGameButtonCallback(() => OpenNewGameMenu().Forget());
    RegisterNewGamePanelBackButtonCallback(() => ReturnFromNewGamePanelToMainMenu().Forget());
    RegisterNewGamePanelStartButtonCallback(() => newGamePanel.visible = false);
  }

  async UniTask HideMainMenu() {
    await ResetButtons();
    await HideBackground();
  }

  async UniTask OpenNewGameMenu() {
    await HideMainMenu();
    newGamePanel.visible = true;
  }

  async UniTask ReturnFromNewGamePanelToMainMenu() {
    newGamePanel.visible = false;
    await SetupButtons();
  }

  async UniTask TweenOpacityAsync(VisualElement element, float to) {
    var transition = GetTransitionProperty(element, "opacity");

    if (transition.duration == 0) {
      element.style.opacity = to;
      return;
    }

    await Tween.VisualElementOpacity(
      element,
      element.resolvedStyle.opacity,
      to,
      transition.duration,
      transition.ease,
      startDelay: transition.delay
    );
  }

  public struct Transition {
    public string property;
    public float delay;
    public float duration;
    public Ease ease;
  }

  public Transition GetTransitionProperty(VisualElement element, string property) {
    var target = TransitionPropertyExists(element, property) ? new StylePropertyName(property) : new StylePropertyName("all");

    return element.resolvedStyle.transitionProperty.AsValueEnumerable()
                  .Index()
                  .Where(t => t.Item == target)
                  .Select(t => new Transition {
                      property = property,
                      delay = element.resolvedStyle.transitionDelay.AsValueEnumerable().ElementAtOrDefault(t.Index).value,
                      duration = element.resolvedStyle.transitionDuration.AsValueEnumerable().ElementAtOrDefault(t.Index).value,
                      ease = element.resolvedStyle.transitionTimingFunction.AsValueEnumerable().ElementAtOrDefault(t.Index).mode.ToPrimeTweenEase()
                    }
                  )
                  .FirstOrDefault();
  }

  bool TransitionPropertyExists(VisualElement element, string property) {
    return element.resolvedStyle.transitionProperty.AsValueEnumerable().Any(p => p == new StylePropertyName(property));
  }

  int GetIndexOfTransitionProperty(VisualElement element, string name) {
    var allProperty = new StylePropertyName("all");

    return element.resolvedStyle.transitionProperty.AsValueEnumerable()
                  .Index()
                  .Where(t => t.Item == new StylePropertyName(name) || t.Item == allProperty)
                  .Select(t => t.Index)
                  .FirstOrDefault(-1);
  }

  public async UniTask HideBackground() {
    await TweenOpacityAsync(background, 0);
    background.visible = false;
  }

  public async UniTask ShowBackground() {
    root.Q<Label>("PressAnyKeyLabel").style.opacity = 1;
    await TweenOpacityAsync(background, 1);
  }

  void OnButtonHover(Button button) {
    button.Q<VisualElement>("ButtonBackground").AddToClassList("button-background--hover");
  }

  void OnButtonOut(Button button) {
    button.Q<VisualElement>("ButtonBackground").RemoveFromClassList("button-background--hover");
  }

  public async UniTaskVoid HidePressAnyKeyLabel() {
    await TweenOpacityAsync(root.Q<Label>("PressAnyKeyLabel"), 0);
  }

  public void RegisterContinueButtonCallback(Action action) { }

  public void RegisterNewGameButtonCallback(Action action) {
    newGameButton.RegisterCallback<PointerDownEvent>(_ => action.Invoke(), TrickleDown.TrickleDown);
  }

  public void RegisterNewGamePanelBackButtonCallback(Action action) {
    newGamePanel.Q<Button>("Back").RegisterCallback<PointerDownEvent>(_ => action.Invoke(), TrickleDown.TrickleDown);
  }

  public void RegisterNewGamePanelStartButtonCallback(Action action) {
    newGamePanel.Q<Button>("Start").RegisterCallback<PointerDownEvent>(_ => action.Invoke(), TrickleDown.TrickleDown);
  }
}