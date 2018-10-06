using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformPopManager : GenericPopManager {
    public override float BreedSortCondition(GameObject o) {
        return GetBrain<PlatformBrain>(o).timeWalking + GetBrain<PlatformBrain>(o).timeAlive;
    }

    public override void MoreBreedPop() { }
}