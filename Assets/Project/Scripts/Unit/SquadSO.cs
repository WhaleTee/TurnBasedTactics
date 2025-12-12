using UnityEngine;

namespace TurnBasedTactics.Unit {
  [CreateAssetMenu(fileName = "Squad", menuName = "Turn Based Tactics/Squad")]
  public class SquadSO : ScriptableObject {
    [SerializeField] SquadData squadData;

    public SquadData GetSquadData() => squadData.Clone() as SquadData;
  }
}