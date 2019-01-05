using System.Collections.Generic;
using UnityEngine;
public enum TileType {
    Dirt, Water, Stone,
}
public class Tile {
    public TileType tileType;

    public readonly Vector2 coorcinates;

    public readonly string name;

    public GameObject tileObject;

    public List<Tile> neighbours;

    public Tile(int x, int y, TileType tileType) {
        name = "Tile-" + x + "-" + y;
        coorcinates = new Vector2(x, y);
        this.tileType = tileType;

        neighbours = new List<Tile>();
    }

    public override string ToString() {
        return name;
    }

    public bool isNeighbourTo(Tile tile) {
        if (Mathf.Abs(this.coorcinates.x - tile.coorcinates.x) <= 1 && Mathf.Abs(this.coorcinates.y - tile.coorcinates.y) <= 1) {
            return true;
        }
        return false;
    }

    public void addNeighoursConnection(Tile tile) {
        if (!this.neighbours.Contains(tile) && tile != null) {
            this.neighbours.Add(tile);
        }

        if (!tile.neighbours.Contains(this)) {
            tile.neighbours.Add(this);
        }
    }
}
