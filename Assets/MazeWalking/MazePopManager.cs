using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazePopManager : GenericPopManager {
    public GameObject startingPosition;
    public MazePopManager() { this.startingPos = startingPosition; }

    public override void MoreBreedPop() {
        elapsed = 0;
    }

    public override float BreedSortCondition(GameObject o) {
        return GetBrain<MazeBrain>(o).distanceTravelled;
    }
}