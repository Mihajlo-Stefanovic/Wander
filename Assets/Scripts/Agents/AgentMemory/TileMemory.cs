using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMemory
{
    public Tile tile;
    public int visitedTimes;
    public bool needToVisit;

    public TileMemory(Tile tile, bool needToVisit = true) {
        this.tile = tile;
        visitedTimes = 0;
        this.needToVisit = needToVisit;
    }

    public override bool Equals(object other) {
        if (other.GetType() == typeof(TileMemory)) {
            TileMemory oth = (TileMemory) other;
            if (oth.tile == tile)
                return true;
        }
        return false;
    }

    public override int GetHashCode() {
        return tile.GetHashCode();
    }
}
