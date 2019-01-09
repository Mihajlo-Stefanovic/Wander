using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMemory
{
    public Tile tile;
    public float visitedTimes;

    public TileMemory(Tile tile) {
        this.tile = tile;
        visitedTimes = 0;
    }

    public override bool Equals(object other) {
        if (tile == other)
            return true;
        else if (other.GetType() == typeof(TileMemory)) {
            TileMemory oth = (TileMemory) other;
            if (oth.tile == tile)
                return true;
        }
        return false;
    }
}
