using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    private float wetness;

    public readonly Vector3Int virtualCoordinates;

    public readonly string name;

    private GameObject tileObject;
    private SpriteRenderer spriteRenderer;

    private List<Tile> neighbours;
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

    public float Wetness {
        get {
            return wetness;
        }

        set {
            if (value > 1) {
                wetness = 1;
            }
            else {
                wetness = value;
            }
        }
    }

    public List<Tile> Neighbours {
        get {
            return neighbours;
        }

        private set {
            neighbours = value;
        }
    }

    public Tile(int x, int y, float wetness) {
        name = "Tile_x:" + x + "_y:" + y;
        virtualCoordinates = new Vector3Int(x, y, 0);
        this.Wetness = wetness;

        Neighbours = new List<Tile>();
    }

    public Tile(Vector2Int position, float wetness) {
        name = "Tile_x:" + position.x + "_y:" + position.y;
        virtualCoordinates = new Vector3Int(position.x, position.y, 0);
        this.Wetness = wetness;

        Neighbours = new List<Tile>();
    }

    public Tile(Vector2Int position) {
        name = "Tile_x:" + position.x + "_y:" + position.y;
        virtualCoordinates = new Vector3Int(position.x, position.y, 0);

        Neighbours = new List<Tile>();
    }

    public Tile(Vector3Int position) {
        name = "Tile_x:" + position.x + "_y:" + position.y;
        virtualCoordinates = new Vector3Int(position.x, position.y, position.z);

        Neighbours = new List<Tile>();
    }

    public override string ToString() {
        return name;
    }

    public bool shouldBeNeighbourTo(Tile tile, PlanetGraphType planetGraphType) {
        switch (planetGraphType) {
            case PlanetGraphType.matrix:
                int offX = Mathf.Abs(this.virtualCoordinates.x - tile.virtualCoordinates.x);
                int offY = Mathf.Abs(this.virtualCoordinates.y - tile.virtualCoordinates.y);
                if (offX <= 1 && offY <= 1) {
                    if (offX != offY) { //removes diagonal and center
                        return true;
                    }
                }
                return false;
            case PlanetGraphType.hexagonal:
                //TODO implement
                break;
        }
        return false;
    }

    internal Tile getNeighbourWDirr(DirectionEnum dirr) {
        foreach(Tile tile in neighbours) {
            if (tile.dirFrom(this) == dirr)
                return tile;
        }
        Debug.Log("No such neighbour for " + name + " " + dirr);
        return null;
    }

    public DirectionEnum dirFrom(Tile parentTile) {
        Vector3Int diff = this.virtualCoordinates - parentTile.virtualCoordinates;
        return Direction.directions[diff];
    }

    public void addNeighoursConnection(Tile tile) {
        if (tile == this) {
            return;
        }

        if (!this.Neighbours.Contains(tile)) {
            this.Neighbours.Add(tile);
        }

        if (!tile.Neighbours.Contains(this)) {
            tile.Neighbours.Add(this);
        }
    }

    public override int GetHashCode() { //HACKED to all be unique!
        string s = virtualCoordinates.x + " " + virtualCoordinates.y + " " + virtualCoordinates.z;
        return s.GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj.GetType() == typeof(Tile)) {
            Tile oth = (Tile)obj;
            return virtualCoordinates.Equals(oth.virtualCoordinates);
        }
        return false;
    }
}
