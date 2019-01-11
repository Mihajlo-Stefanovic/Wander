using System.Collections.Generic;
using System.Linq;

public class MoveData {
    public Dictionary<DirectionEnum, float> dirPercent;
    public Dictionary<DirectionEnum, int> numOfDirMoves;

    public int numOfMoves;
    public MoveData() {
        dirPercent = new Dictionary<DirectionEnum, float>(); //0...1
        numOfDirMoves = new Dictionary<DirectionEnum, int>();
        numOfMoves = 0;
    }

    public void addMove(DirectionEnum direction) {
        numOfMoves += 1;

        if (numOfDirMoves.ContainsKey(direction)) {
            numOfDirMoves[direction] += 1;
        }
        else {
            numOfDirMoves.Add(direction, 1);
            dirPercent.Add(direction, -1);
        };


        foreach (var key in dirPercent.Keys.ToList()) {
            dirPercent[key] = ((float)numOfDirMoves[key]) / numOfMoves;
        }

    }

    public override string ToString() {
        string s = "Dir percent: ";
        foreach (var el in dirPercent) {
            s += ((DirectionEnum)el.Key).ToString() + " " + el.Value + " | ";
        }

        s += "NumOfDirMoves: ";
        foreach (var el in numOfDirMoves) {
            s += ((DirectionEnum)el.Key).ToString() + " " + el.Value +" | ";
        }

        return s;
    }
}
