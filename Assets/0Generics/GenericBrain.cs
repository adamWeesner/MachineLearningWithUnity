using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericBrain : MonoBehaviour {
    protected int mDNALength { get; set; }
    public GenericDNA dna { get; set; }
    public bool alive { get; set; }
    protected int mDNAMaxLength { get; set; }

    public GenericBrain() {
        this.mDNALength = -1;
        this.alive = true;
    }

    public GenericBrain(int DNALength, int dnaMaxLength) {
        this.mDNALength = DNALength;
        this.mDNAMaxLength = dnaMaxLength;
        this.alive = true;
    }

    public abstract void MoreInit();

    public void Init() {
        dna = new GenericDNA(mDNALength, mDNAMaxLength);
        MoreInit();
    }

    public abstract void MoreOnCollisionEnter(Collision col);

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "dead") {
            alive = false;
            MoreOnCollisionEnter(col);
        }
    }
}