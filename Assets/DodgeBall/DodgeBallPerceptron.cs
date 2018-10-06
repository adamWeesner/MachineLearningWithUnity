using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeBallPerceptron : GenericPerceptron {
    List<TrainingSet> trainingSet = new List<TrainingSet>();
    public GameObject npc;

    public void SendInput(double inputColor, double inputItem, double output) {
        // react
        double result = CalcOutput(inputColor, inputItem);

        // duck for cover
        if (result == 0) {
            npc.GetComponent<Animator>().SetTrigger("Crouch");
            npc.GetComponent<Rigidbody>().isKinematic = false;
        } else {
            npc.GetComponent<Rigidbody>().isKinematic = false;
        }

        TrainingSet set = new TrainingSet();
        set.input = new double[2] { inputColor, inputItem };
        set.output = output;
        trainingSet.Add(set);
        Training();
    }

    double CalcOutput(double i1, double i2) {
        double[] inp = new double[] { i1, i2 };
        double dp = DotProductBias(weights, inp);
        if (dp >= 0) return 1;
        return 0;
    }

    public override void Train() { }

    public override void MoreStartBeforeTrain() { }

    public override void MoreStartAfterTrain() { }

    public override void SetName() {
        print("setname called");
        name = "DodgeBall"; }

    void Training() {
        foreach (TrainingSet set in trainingSet) {
            UpdateWeights(set);
        }
    }
}