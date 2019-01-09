using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour {
    public Tile currentTile;

    public List<TileMemory> agentMemory;
    public List<Tile> currentVision;
}
