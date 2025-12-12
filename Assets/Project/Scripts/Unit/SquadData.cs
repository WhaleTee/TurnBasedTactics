using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace TurnBasedTactics.Unit {
  [Serializable]
  public class SquadData : ICloneable {
    public SquadUnit firstUnit, secondUnit, thirdUnit;
    public Vector3Int deployInitialPoint;
    public int maxDeployDistance;

    T[] CloneArray<T>(IReadOnlyList<T> array) where T : class {
      var newArray = new T[array.Count];

      for (var i = 0; i < newArray.Length; i++) newArray[i] = SerializationUtility.CreateCopy(array[i]) as T;
      return newArray;
    }
    
    public object Clone() {
      return new SquadData {
        firstUnit =
        new SquadUnit {
          configuration = firstUnit.configuration,
          state =
          new UnitState {
            movement = new UnitMovementState { availableSteps = firstUnit.configuration.movement.steps, canMove = true },
            position = new UnitPositionState(),
            abilities = CloneArray(firstUnit.configuration.abilities)
          }
        },
        secondUnit = new SquadUnit {
          configuration = secondUnit.configuration,
          state = new UnitState {
            movement = new UnitMovementState { availableSteps = secondUnit.configuration.movement.steps, canMove = true },
            position = new UnitPositionState(),
            abilities = CloneArray(secondUnit.configuration.abilities)
          }
        },
        thirdUnit = new SquadUnit {
          configuration = thirdUnit.configuration,
          state = new UnitState {
            movement = new UnitMovementState { availableSteps = thirdUnit.configuration.movement.steps, canMove = true },
            position = new UnitPositionState(),
            abilities = CloneArray(thirdUnit.configuration.abilities)
          }
        },
        deployInitialPoint = deployInitialPoint,
        maxDeployDistance = maxDeployDistance
      };
    }
  }
}