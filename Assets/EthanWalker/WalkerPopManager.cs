using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WalkerPopManager : GenericPopManager {
    public override float BreedSortCondition(GameObject o) {
        return GetBrain<WalkerBrain>(o).timeAlive;
    }

    public override void MoreBreedPop() { }
}