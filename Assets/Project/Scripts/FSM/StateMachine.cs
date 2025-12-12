using System;
using System.Collections.Generic;
using MessagePipe;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhaleTee.Reflex.Extensions;

namespace WhaleTee.FSM {
  public sealed class StateMachine : IDisposable {
    readonly Dictionary<Type, State> states = new();
    readonly StateTransitionAction stateTransitionAction;
    State current;

    public event Action<Type, Type> stateChangedEvent = delegate { };
    [Inject] readonly IPublisher<StateLifecycleChangedEvent> stateLifecycleEnteredEventPublisher;

    StateMachine() => stateTransitionAction = new StateTransitionAction(OnStateTransition);
    
    void OnStateTransition(Type stateType) {
      if (states.TryGetValue(stateType, out var state)) {
        var oldState = current;
        ChangeState(state);

        stateChangedEvent.Invoke(oldState?.GetType(), current?.GetType());
      }
    }

    void AddState(State state) {
      states.TryAdd(state.GetType(), state);
    }

    void SetState(State state) {
      current = states[state.GetType()];
      current?.Enter();
    }

    void ChangeState(State state) {
      if (state == null || state == current) return;

      var previousState = current;
      var nextState = states[state.GetType()];

      previousState.Exit();
      stateLifecycleEnteredEventPublisher.Publish(new StateLifecycleChangedEvent(previousState.GetType(), (byte)StateLifecycle.Exit));
      nextState?.Enter();
      stateLifecycleEnteredEventPublisher.Publish(new StateLifecycleChangedEvent(nextState?.GetType(), (byte)StateLifecycle.Enter));
      current = nextState;
    }

    public void Update() {
      current?.Update();
      stateLifecycleEnteredEventPublisher.Publish(new StateLifecycleChangedEvent(current?.GetType(), (byte)StateLifecycle.Update));
    }

    public void FixedUpdate() {
      current?.FixedUpdate();
    }

    public void Dispose() {
      current?.Exit();
    }

    public sealed class StateMachineBuilder {
      StateMachine stateMachine;
      State initialState;
      List<State> registeredStates = new();

      State AddStateInternal(State state) {
        if (state != null) {
          stateMachine.AddState(state);
          registeredStates.Add(state);
          state.StateTransitionAction = stateMachine.stateTransitionAction;
        }

        return state;
      }

      public static StateMachineBuilder Create() {
        var builder = new StateMachineBuilder { stateMachine = new StateMachine(), initialState = null, registeredStates = new List<State>() };
        return builder;
      }

      public StateMachineBuilder AddInitialState(State state) {
        initialState = AddStateInternal(state);
        return this;
      }

      public StateMachineBuilder AddState(State state) {
        AddStateInternal(state);
        return this;
      }

      public StateMachine Build() {
        if (initialState == null) {
          var firstRegisteredState = registeredStates[0];

          if (firstRegisteredState == null) {
            Debug.LogError($"Initial state has not been registered! The state machine would not be built.");
            return null;
          }

          Debug.Log($"The first registered state [{firstRegisteredState.GetType().Name}] would be the initial.");

          initialState = firstRegisteredState;
        }

        stateMachine.SetState(initialState);
        return stateMachine.InjectAttributes();
      }
    }
  }
}