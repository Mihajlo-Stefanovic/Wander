#undef AGENTMEMORY
#undef CURRENTVISION
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAgent : Agent {
    public int visionDistance = 1;
    public float paceOfMoving = 1f;

    public Vector3Int currentAgentToBasePosition;

    private Vector3Int destination;

    private Vector3 idealRealWorldPosition;
    private float realWorldSpeedOfMoving = 0.2f;
    void Awake() {
        agentMemory = new List<TileMemory>();
        currentVision = new List<Tile>();
    }

    void Start() {
        //position is set in base
        StartCoroutine(playTurn());
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, idealRealWorldPosition, realWorldSpeedOfMoving);
    }
    public void setInitialPosition() {
        transform.localPosition = idealRealWorldPosition = currentAgentToBasePosition;
        currentTile = Planet.instance.planetGraphInfo.currPlanetTiles[0]; //first tile
    }

    private IEnumerator playTurn() {
#if AGENTMEMORY
        foreach(Tile tile in agentMemory)
            print(tile);
        print("----");
#endif
#if CURRENTVISION
        foreach(Tile tile in currentVision)
            print(tile);
        print("----");
#endif
        yield return new WaitForSeconds(paceOfMoving);

        Move();
        updateCurentVision();
        updateAgentMemory();
        Planet.instance.agentMoved(this);

        StartCoroutine(playTurn());
    }

    private void updateAgentMemory() {

        List<Tile> currentVision = new List<Tile>();
        addPlanetTilesWithDFS(currentVision);

        foreach (Tile tile in currentVision) {
            TileMemory tileMemory = new TileMemory(tile);
            if (!agentMemory.Contains(tileMemory)) {
                agentMemory.Add(tileMemory);
            }
        }
    }

    private void updateCurentVision() {
        currentVision.Clear();

        addPlanetTilesWithDFS(currentVision);

        foreach (Tile tile in currentVision) {
            TileMemory tileMemory = new TileMemory(tile);
            if (!agentMemory.Contains(tileMemory)) {
                agentMemory.Add(tileMemory);
            }
        }
    }

    private void addPlanetTilesWithDFS(List<Tile> listToAdd) {
        Stack<KeyValuePair<Tile, int>> tilesToAdd = new Stack<KeyValuePair<Tile, int>>();

        tilesToAdd.Push(new KeyValuePair<Tile, int>(currentTile, 0));

        while (tilesToAdd.Count != 0) {
            KeyValuePair<Tile, int> currTileAndDepth = tilesToAdd.Pop();

            listToAdd.Add(currTileAndDepth.Key);

            if (currTileAndDepth.Value < visionDistance) {
                foreach (Tile neighour in currTileAndDepth.Key.Neighbours) {
                    if (!listToAdd.Contains(neighour)) {
                        tilesToAdd.Push(new KeyValuePair<Tile, int>(neighour, currTileAndDepth.Value + 1));
                    }
                }
            }

        }
    }

    private void Move() {
        if (currentTile.Neighbours.Count==0) {
            Debug.Log(currentTile.name + " neighbours.Count = 0");
            return;
        }
        Tile newTile = currentTile.Neighbours[0];

        foreach (Tile tile in currentTile.Neighbours) {
            //TODO DECIDE WHERE TO GO
            //TileMemory tileMemory = agentMemory.Find(new TileMemory(tile));
            if (tile.Wetness > newTile.Wetness) {
                newTile = tile;
            }
        }

        if (newTile.TileObject != null) {
            moveToTile(newTile);
        }
    }

    private void moveToTile(Tile newTile) {
        this.currentTile = newTile;
        idealRealWorldPosition = newTile.TileObject.transform.position;
    }
}
