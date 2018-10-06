using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBrain : GenericBrain {
    public GameObject eyes;
    bool seeWall = true;
    Vector3 startPosition;
    public float distanceTravelled = 0;

    public MazeBrain() {
        this.mDNALength = 2;
        this.mDNAMaxLength = 360;
    }

    public override void MoreInit() {
        startPosition = this.transform.position;
    }

    public override void MoreOnCollisionEnter(Collision col) {
        //GameObject.FindObjectOfType<PopulationManager>().aliveCount--;
        distanceTravelled = 0;
    }

    void Update() {
        if (!alive) return;

        seeWall = false;
        RaycastHit hit;
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * .5f, Color.red);
        if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward, out hit, 0.5f)) {
            if (hit.collider.gameObject.tag == "wall") {
                seeWall = true;
            }
        }
    }

    void FixedUpdate() {
        if (!alive) return;

        float h = 0;
        float v = dna.GetGene(0);

        if (seeWall) {
            h = dna.GetGene(1);
        }

        this.transform.Translate(0, 0, v * 0.001f);
        this.transform.Rotate(0, h, 0);
        distanceTravelled = Vector3.Distance(startPosition, this.transform.position);
    }
}