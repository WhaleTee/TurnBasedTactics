using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using WhaleTee.Comparers;
using WhaleTee.Extensions;
using WhaleTee.Input;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace WhaleTee.Reactive.Input {
  public sealed class UserInput : IDisposable {
    const float EQUITY_TOLERANCE = 0.1f;
    readonly InputActions inputActions;
    DisposableBag subscriptions;

    static Camera MainCamera => Camera.main;

    public ReactiveProperty<bool> LeftClick { get; }
    public ReactiveProperty<bool> RightClick { get; }
    public ReactiveProperty<bool> MiddleClick { get; }
    public ReactiveProperty<Vector2> PointerPosition { get; }
    public ReactiveProperty<Vector3> PointerWorldPosition { get; }
    public ReactiveProperty<Vector2> ScrollWheel { get; }
    public ReactiveProperty<bool> AnyKey { get; }

    public UserInput() {
      LeftClick = new ReactiveProperty<bool>();
      RightClick = new ReactiveProperty<bool>();
      MiddleClick = new ReactiveProperty<bool>();
      PointerPosition = new ReactiveProperty<Vector2>(Vector2.zero, EqualityComparers.Vector2(EQUITY_TOLERANCE));
      PointerWorldPosition = new ReactiveProperty<Vector3>(Vector3.zero, EqualityComparers.Vector3(EQUITY_TOLERANCE));
      ScrollWheel = new ReactiveProperty<Vector2>(Vector2.zero, EqualityComparers.Vector2(EQUITY_TOLERANCE));
      AnyKey = new ReactiveProperty<bool>();

      inputActions = new InputActions();
      inputActions.Enable();
      HandleInput();
    }

    static Vector3 ScreenToWorldPoint(Camera camera, Vector3 point) {
      return camera.ScreenToWorldPoint(point);
    }

    void HandleInput() {
      InputSystem.onEvent += HandleAnyKey;

      Observable.EveryUpdate(UnityFrameProvider.EarlyUpdate)
                .Subscribe(_ => {
                             LeftClick.Value = inputActions.UI.Click.IsPressed();
                             RightClick.Value = inputActions.UI.RightClick.IsPressed() && inputActions.UI.RightClick.WasPressedThisFrame();
                             MiddleClick.Value = inputActions.UI.MiddleClick.IsPressed() && inputActions.UI.MiddleClick.WasPressedThisFrame();
                           }
                )
                .AddTo(ref subscriptions);

      Observable.FromEvent<InputAction.CallbackContext>(
                  handler => inputActions.UI.ScrollWheel.performed += handler,
                  handler => inputActions.UI.ScrollWheel.performed -= handler
                )
                .Subscribe(ctx => ScrollWheel.Value += ctx.ReadValue<Vector2>())
                .AddTo(ref subscriptions);

      Observable.EveryUpdate(UnityFrameProvider.PreLateUpdate)
                .Subscribe(_ => {
                             ScrollWheel.Value = Vector2.zero;
                             AnyKey.Value = false;
                           }
                )
                .AddTo(ref subscriptions);

      Observable.FromEvent<InputAction.CallbackContext>(
                  handler => inputActions.UI.Point.performed += handler,
                  handler => inputActions.UI.Point.performed -= handler
                )
                .Subscribe(ctx => {
                             PointerPosition.Value = ctx.ReadValue<Vector2>();
                             PointerWorldPosition.Value = GetPointerPositionWorld();
                           }
                )
                .AddTo(ref subscriptions);
    }

    void HandleAnyKey(InputEventPtr evt, InputDevice device) {
      AnyKey.Value = evt.GetFirstButtonPressOrNull() != null;
    }

    public static void WrapCursorPosition(Vector2 position) {
      Mouse.current.WarpCursorPosition(position);
    }

    public Vector3 GetPointerPositionWorld() {
      return ScreenToWorldPoint(MainCamera, PointerPosition.Value.With(z: MainCamera.transform.position.z));
    }

    public Vector3 GetPointerPositionWorldSaveZ(Vector3 point) {
      return ScreenToWorldPoint(MainCamera, PointerPosition.Value.With(z: MainCamera.transform.InverseTransformPoint(point).z));
    }

    public Ray GetRay() {
      return MainCamera.ScreenPointToRay(PointerPosition.Value);
    }

    public Vector2 GetPointerPositionInvertY(float value) {
      var pointerPosition = PointerPosition.Value;
      pointerPosition.y = value - pointerPosition.y;
      return pointerPosition;
    }

    public void SetCursorVisible(bool visible, Vector2 atPoint) {
      Cursor.lockState = !visible ? CursorLockMode.Locked : CursorLockMode.None;
      Cursor.visible = visible;
      WrapCursorPosition(atPoint);
    }

    public void Dispose() {
      InputSystem.onEvent -= HandleAnyKey;
      subscriptions.Dispose();
      inputActions?.Disable();
      inputActions?.Dispose();
    }
  }
}