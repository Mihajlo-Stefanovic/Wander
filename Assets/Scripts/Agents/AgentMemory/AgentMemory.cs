#undef DEBUG
using System.Collections.Generic;

public class AgentMemory {
    public Dictionary<int, TileMemory> tileMemories; //TODO : shouldn't use hash code as key
    public List<TileMemory> currentVision;

    public AgentMemory() {
        tileMemories = new Dictionary<int, TileMemory>();
        currentVision = new List<TileMemory>();
    }

    public void updateAgentMemory() {
        foreach (TileMemory tileMemory in currentVision) {
            addMemory(tileMemory);
        }
    }

    public void updateCurentVision(Tile currentTile, int visionDistance, float wetnessLimit = -1, bool reset=true) {
        if(reset)
            currentVision = new List<TileMemory>();

        List<Tile> tiles = Planet.getTilesInDepth(currentTile, visionDistance, wetnessLimit);

        foreach (Tile tile in tiles) {
            TileMemory tileMemory;
            if (tileMemories.TryGetValue(tile.GetHashCode(), out tileMemory)) {
                ;
            }
            else {
                if (wetnessLimit > -1 && tile.Wetness > wetnessLimit) {
                    tileMemory = new TileMemory(tile, false); //as if visited
                }
                else {
                    tileMemory = new TileMemory(tile);
                }
            }
            currentVision.Add(tileMemory);
        }
    }

    private void addMemory(TileMemory tileMemory) {
        if (tileMemories.ContainsKey(tileMemory.GetHashCode())) {
#if DEBUG
            Debug.Log("Replaced old: " + tileMemories[tileMemory.GetHashCode()].tile.name);
#endif
            tileMemories[tileMemory.GetHashCode()] = tileMemory; //replace old
        }
        else {
            tileMemories.Add(tileMemory.GetHashCode(), tileMemory);
        }
    }

    public List<Tile> getTilesFromMemoryTiles(List<TileMemory> tileMemoriesToConvert) {
        List<Tile> tiles = new List<Tile>();
        foreach (TileMemory tileMemory in tileMemoriesToConvert) {
            tiles.Add(tileMemory.tile);
        }
        return tiles;
    }

    public List<Tile> getTilesFromMemoryTiles(Dictionary<int, TileMemory> tileMemoriesF) {
        List<Tile> tiles = new List<Tile>();
        foreach (KeyValuePair<int, TileMemory> pair in tileMemoriesF) {
            tiles.Add(pair.Value.tile);
        }
        return tiles;
    }
}
