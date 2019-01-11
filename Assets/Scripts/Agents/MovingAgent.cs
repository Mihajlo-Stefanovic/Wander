#undef AGENTMEMORY
#undef CURRENTVISION
#undef MOVING
#undef MOVEDATA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAgent : Agent {
    public int visionDistance;

    public float paceOfMoving;
    private Vector3 idealRealWorldPosition;
    private float realWorldSpeedOfMoving = 10f;

    public MoveData orders;
    public MoveData moveData;

    public float timesWeigth;
    public float ordersWeight;

    public List<Tile> waterTilesFound;
    public float waterSeeingWetLimit = 0.8f;
    public int waterSeeingDepthLimit = 10;

    public int fuelCapacity;
    [System.NonSerialized]
    public int currFuel;

    [System.NonSerialized]
    public bool isReturningToBase;

    [System.NonSerialized]
    public bool isInBase;

    void Awake() {
        agentMemory = new AgentMemory();
        moveData = new MoveData();
        waterTilesFound = new List<Tile>();
        currFuel = fuelCapacity;
        isReturningToBase = false;
        isInBase = false;
    }

    void Start() {
        //StartCoroutine(playTurn());
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position,
            idealRealWorldPosition, realWorldSpeedOfMoving * Time.deltaTime);
    }

    public void initialize() {
        transform.localPosition = idealRealWorldPosition = Base.instance.currentTile.TileObject.transform.position;
        currentTile = Base.instance.currentTile;
        StartCoroutine(playTurn());
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

        isReturningToBase = (currFuel <= fuelCapacity / 2) ? true : false; //TODO change to base offset
        if (!isReturningToBase) {
            moveToNew();
        }
        else {
            returnToBase();
        }

        agentMemory.updateCurentVision(currentTile, visionDistance);
        agentMemory.updateCurentVision(currentTile, waterSeeingDepthLimit, waterSeeingWetLimit, false);
        agentMemory.updateAgentMemory();
        List<Tile> waterTiles = Planet.getTilesInDepth(currentTile, waterSeeingDepthLimit, waterSeeingWetLimit);
        foreach (Tile tile in waterTiles) {
            waterTilesFound.Add(tile);
        }
        Planet.instance.agentMoved(this);

        if(gameObject.activeSelf)
            StartCoroutine(playTurn());
    }

    private void returnToBase() {
        int up; int down; int right; int left;

        if (moveData.numOfDirMoves.TryGetValue(DirectionEnum.up, out up)) { }
        else {
            up = 0;
        }
        if (moveData.numOfDirMoves.TryGetValue(DirectionEnum.down, out down)) { }
        else {
            down = 0;
        }
        if (moveData.numOfDirMoves.TryGetValue(DirectionEnum.right, out right)) { }
        else {
            right = 0;
        }
        if (moveData.numOfDirMoves.TryGetValue(DirectionEnum.left, out left)) { }
        else {
            left = 0;
        }

        int vertical = up - down; int horizontal = right - left;
        DirectionEnum dirr;
        if (vertical > 0) {
            dirr = DirectionEnum.down;
        }
        else if (vertical < 0) {
            dirr = DirectionEnum.up;
        }
        else if (horizontal > 0) {
            dirr = DirectionEnum.left;
        }
        else if (horizontal < 0) {
            dirr = DirectionEnum.right;
        }
        else { //is in base
            isReturningToBase = false;
            Base.instance.agentArrived(this);
            return;
        }

        Tile newTile = currentTile.getNeighbourWDirr(dirr);
        currFuel--;
        moveData.addMove(dirr);

        if (newTile.TileObject != null) {
            moveToTile(newTile);
        }
    }

    private void moveToNew() {
        if (currentTile.Neighbours.Count == 0) {
#if MOVING
            Debug.Log(currentTile.name + " neighbours.Count = 0");
#endif
            return;
        }

        TileMemory tileMemoryResoult;
        int randomNeighbour = Random.Range(0, currentTile.Neighbours.Count);
        if (agentMemory.tileMemories.TryGetValue(currentTile.Neighbours[randomNeighbour].GetHashCode(), out tileMemoryResoult)) {
            ;
        }
        else {
            tileMemoryResoult = new TileMemory(currentTile.Neighbours[randomNeighbour]);
        }

#if MOVING
        Debug.Log("curr" + currentTile.name);
#endif
        foreach (Tile tile in currentTile.Neighbours) { //TODO : change in vision tiles
#if MOVING
            Debug.Log(tile.name);
#endif
            TileMemory tileMemory;
            if (agentMemory.tileMemories.TryGetValue(tile.GetHashCode(), out tileMemory)) {
                ;
            }
            else {
                tileMemory = new TileMemory(tile);
            }

            if ((shouldMoveTo(tileMemoryResoult) <
                shouldMoveTo(tileMemory))) {

                tileMemoryResoult = tileMemory;
            }
        }

        currFuel -= 1;
        moveData.addMove(tileMemoryResoult.tile.dirFrom(currentTile));
#if MOVEDATA
        Debug.Log(moveData.ToString());
#endif
        tileMemoryResoult.visitedTimes += 1;
        if (tileMemoryResoult.tile.TileObject != null) {
            moveToTile(tileMemoryResoult.tile);
        }
    }

    private float shouldMoveTo(TileMemory tileMemory) {
        if (tileMemory.needToVisit) {
            return tileMemory.tile.Wetness - tileMemory.visitedTimes * timesWeigth +
            notObeyingOrders(tileMemory.tile.dirFrom(currentTile)) * ordersWeight;
        }
        return int.MinValue;
    }

    private float notObeyingOrders(DirectionEnum dirr) {
        float order;
        if (orders.dirPercent.TryGetValue(dirr, out order)) {
            ;
        }
        else {
            order = 0;
        }

        float currData;
        if (moveData.dirPercent.TryGetValue(dirr, out currData)) {
            ;
        }
        else {
            currData = 0;
        }

        return -Mathf.Abs(order - currData);
    }

    public void moveToTile(Tile newTile) {
#if MOVING
        Debug.Log("move to: " + newTile.name);
#endif
        this.currentTile = newTile;
        idealRealWorldPosition = newTile.TileObject.transform.position;
    }

    internal void resetAgent() {
        agentMemory = new AgentMemory();
        waterTilesFound = new List<Tile>();
        isInBase = false;
        currFuel = fuelCapacity;
    }
}
