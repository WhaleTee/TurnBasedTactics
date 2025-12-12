using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WhaleTee.Grid {
  [Serializable]
  public class NavigationTilemapContainer {
    [field: SerializeField] public Tilemap Tilemap { get; private set; }
    [field: SerializeField] public TileBase StepTile { get; private set; }
    [field: SerializeField] public TileBase NavigationTile { get; private set; }
  }
}