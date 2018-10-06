using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBrain : GenericBrain {

    public float timeAlive;
    public float timeWalking;
    public GameObject eyes;
    bool seeGround = true;

    public GameObject ethanPrefab;
    GameObject ethan;

    public PlatformBrain() {
        this.mDNALength = 2;
        this.mDNAMaxLength = 3;
    }

    void OnDestroy() {
        Destroy(ethan);
    }

    public override void MoreInit() {
        timeAlive = 0;
        ethan = Instantiate(ethanPrefab, this.transform.position, this.transform.rotation);
        ethan.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target = this.transform;
    }

    void Update() {
        if (!alive) return;

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red, 10);
        seeGround = false;
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit)) {
            if (hit.collider.gameObject.tag == "platform") seeGround = true;
        }

        timeAlive = PlatformPopManager.elapsed;

        float move = 0;
        float turn = 0;
        if (seeGround) {
            if (dna.GetGene(0) == 0) {
                move = 1;
                timeWalking += 1;
            } else if (dna.GetGene(0) == 1) turn = -90;
            else if (dna.GetGene(0) == 2) turn = 90;
        } else {
            if (dna.GetGene(1) == 0) {
                move = 1;
                timeWalking += 1;
            } else if (dna.GetGene(1) == 1) turn = -90;
            else if (dna.GetGene(1) == 2) turn = 90;
        }

        this.transform.Translate(0, 0, move * 0.1f);
        this.transform.Rotate(0, turn, 0);
    }

    public override void MoreOnCollisionEnter(Collision col) { }
}