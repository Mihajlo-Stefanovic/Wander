using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public float wetness;

    public readonly Vector3Int virtualCoordinates;

    public readonly string name;

    private GameObject tileObject;
    private SpriteRenderer spriteRenderer;

    public List<Tile> neighbours;
    private Vector3Int vector3Int;

    public SpriteRenderer SpriteRenderer {
        private set {

        }
        get {
            return spriteRenderer;
        }
    }
    public GameObject TileObject {
        get {
            return tileObject;
        }

        set {
            tileObject = value;
            spriteRenderer = tileObject.GetComponentInChildren<SpriteRenderer>();
        }
    }

    public Tile(int x, int y, float wetness) {
        name = "Tile_x:" + x + "_y:" + y;
        virtualCoordinates = new Vector3Int(x, y, 0);
        this.wetness = wetness;

        neighbours = new List<Tile>();
    }

    public Tile(Vector2Int position, float wetness) {
        name = "Tile_x:" + position.x + "_y:" + position.y;
        virtualCoordinates = new Vector3Int(position.x, position.y, 0);
        this.wetness = wetness;

        neighbours = new List<Tile>();
    }

    public Tile(Vector2Int position) {
        name = "Tile_x:" + position.x + "_y:" + position.y;
        virtualCoordinates = new Vector3Int(position.x, position.y, 0);

        neighbours = new List<Tile>();
    }

    public Tile(Vector3Int position) {
        name = "Tile_x:" + position.x + "_y:" + position.y;
        virtualCoordinates = new Vector3Int(position.x, position.y, position.z);

        neighbours = new List<Tile>();
    }

    public override string ToString() {
        return name;
    }

    public bool shouldBeNeighbourTo(Tile tile, PlanetGraphType planetGraphType) {
        switch (planetGraphType) {
            case PlanetGraphType.matrix:
                if (Mathf.Abs(this.virtualCoordinates.x - tile.virtualCoordinates.x) <= 1 && Mathf.Abs(this.virtualCoordinates.y - tile.virtualCoordinates.y) <= 1) {
                    return true;
                }
                return false;
            case PlanetGraphType.hexagonal:
                //TODO implement
                break;
        }
        return false;
    }

    public void addNeighoursConnection(Tile tile) {
        if (tile == this) {
            return;
        }

        if (!this.neighbours.Contains(tile)) {
            this.neighbours.Add(tile);
        }

        if (!tile.neighbours.Contains(this)) {
            tile.neighbours.Add(this);
        }
    }

}
