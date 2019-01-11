using System.Collections.Generic;
using UnityEngine;

public enum DirectionEnum {
    up, down, left, right
}

class Direction : MonoBehaviour {
    static public Dictionary<Vector3Int, DirectionEnum> directions = new Dictionary<Vector3Int, DirectionEnum> {
        { Vector3Int.up, DirectionEnum.up },
        { Vector3Int.down, DirectionEnum.down },
        { Vector3Int.left, DirectionEnum.left },
        { Vector3Int.right, DirectionEnum.right }
    };
}
