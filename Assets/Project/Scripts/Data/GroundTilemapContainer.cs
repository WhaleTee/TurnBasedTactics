using UnityEngine.Tilemaps;

namespace WhaleTee.Grid {
  public class GroundTilemapContainer {
    public readonly Tilemap tilemap;

    public GroundTilemapContainer(Tilemap tilemap) {
      this.tilemap = tilemap;
    }
  }
}