#undef AGENTMEMORY
#undef CURRENTVISION
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAgent : Agent {
    public int visionDistance = 1;
    public float paceOfMoving = 1f;

    public Vector2 currentAgentToBasePosition;

    private Vector2 destination;
    void Awake() {
        agentMemory = new List<Tile>();
        currentVision = new List<Tile>();
    }

    void Start() {
        //position is set in base
        StartCoroutine(playTurn());
    }

    public void setInitialPosition() {//uses real world matrix positions, to improve maybe
        transform.localPosition = currentAgentToBasePosition;

        currentTile = Planet.instance.planetGraphInfo.getTileWithPos
            (Base.instance.position + currentAgentToBasePosition);
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

        Move(calculateWhereToMoveMatrix());
        updateCurentVision();
        updateAgentMemory();
        Planet.instance.agentMoved(this);

        StartCoroutine(playTurn());
    }

    private void updateAgentMemory() {

        List<Tile> currentVision = new List<Tile>();
        addPlanetTilesWithBFS(currentVision);

        foreach (Tile tile in currentVision) {
            if (!agentMemory.Contains(tile)) {
                agentMemory.Add(tile);
            }
        }
    }

    private void updateCurentVision() {
        currentVision.Clear();

        addPlanetTilesWithBFS(currentVision);

        foreach (Tile tile in currentVision) {
            if (!agentMemory.Contains(tile)) {
                agentMemory.Add(tile);
            }
        }
    }

    private void addPlanetTilesWithBFS(List<Tile> listToAdd) {
        Stack<KeyValuePair<Tile, int>> tilesToAdd = new Stack<KeyValuePair<Tile, int>>();

        tilesToAdd.Push(new KeyValuePair<Tile, int>(currentTile, 0));

        while (tilesToAdd.Count != 0) {
            KeyValuePair<Tile, int> currTileAndDepth = tilesToAdd.Pop();

            listToAdd.Add(currTileAndDepth.Key);
            
            if (currTileAndDepth.Value < visionDistance) {
                foreach (Tile neighour in currTileAndDepth.Key.neighbours) {
                    if (!listToAdd.Contains(neighour)) {
                        tilesToAdd.Push(new KeyValuePair<Tile, int>(neighour, currTileAndDepth.Value + 1));
                    }
                }
            }

        }
    }

    //TODO adapt to graph
    private Vector2 calculateWhereToMoveMatrix() {
        int maxX = 1; int maxY = 1;
        return new Vector2(UnityEngine.Random.Range(0, maxX)+1,
            UnityEngine.Random.Range(0, maxY)+1); //temp
    }

    private void Move(Vector2 dir) {
        this.currentTile = currentTile.
            neighbours[UnityEngine.Random.Range(0, currentTile.neighbours.Count)];

        moveToTile(currentTile);
    }

    private void moveToTile(Tile tile) {
        transform.position = new Vector3(tile.coorcinates.x,0, tile.coorcinates.y);
    }
}
