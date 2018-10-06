using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class WalkerBrain : GenericBrain {
    private ThirdPersonCharacter mCharacter;
    private Vector3 mMove;
    private bool mJump;

    public float timeAlive;

    public WalkerBrain() {
        this.mDNALength = 1;
        this.mDNAMaxLength = 6;
    }

    override public void MoreInit() {
        mCharacter = GetComponent<ThirdPersonCharacter>();
        timeAlive = 0;
    }

    override public void MoreOnCollisionEnter(Collision col) { }

    void FixedUpdate() {
        float h = 0;
        float v = 0;
        bool crouch = false;
        if (dna.GetGene(0) == 0) v = 1;
        else if (dna.GetGene(0) == 1) v = -1;
        else if (dna.GetGene(0) == 2) h = -1;
        else if (dna.GetGene(0) == 3) h = 1;
        else if (dna.GetGene(0) == 4) mJump = true;
        else if (dna.GetGene(0) == 5) crouch = true;

        mMove = v * Vector3.forward + h * Vector3.right;
        mCharacter.Move(mMove, crouch, mJump);
        mJump = false;
        if (alive) timeAlive += Time.deltaTime;
    }
}