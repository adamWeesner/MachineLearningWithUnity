using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDNA {
    List<int> genes = new List<int>();
    int dnaLength = 0;
    int maxValues = 0;

    public GenericDNA(int l, int v) {
        dnaLength = l;
        maxValues = v;
        SetRandom();
    }

    public void SetRandom() {
        genes.Clear();
        for (int i = 0; i < dnaLength; i++)
            genes.Add(Random.Range(0, maxValues));
    }

    public void SetInt(int pos, int value) {
        genes[pos] = value;
    }

    public void Combine(GenericDNA dna1, GenericDNA dna2) {
        for (int i = 0; i < dnaLength; i++) {
            int c = 0;

            if (i < dnaLength / 2.0) c = dna1.genes[i];
            else c = dna2.genes[i];

            genes[i] = c;
        }
    }

    public void Mutate() {
        genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValues);
    }

    public int GetGene(int pos) {
        return genes[pos];
    }
}
