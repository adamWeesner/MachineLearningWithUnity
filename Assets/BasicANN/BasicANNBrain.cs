using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BianryTrainingSet {
    public int input1;
    public int input2;
    public int desiredOutput;
}

[System.Serializable]
public class Trainer {
    public int timesToTrain;
    public int perLoop;
    public int soFar;
}

public class BasicANNBrain : MonoBehaviour {
    ANN ann;
    double sumSquareError = 0;

    public List<BianryTrainingSet> trainingSet;
    public ANNBuilder aNNBuilder;
    public Trainer trainer = new Trainer();

    void Start() {
        ann = new ANN(aNNBuilder.inputs, aNNBuilder.hidden, aNNBuilder.outputs, aNNBuilder.neuronsPerHidden, aNNBuilder.alpha, aNNBuilder.hiddenFunction, aNNBuilder.outputFunction);
    }

    private void Update() {
        List<double> result;
        if (trainer.soFar < trainer.timesToTrain) {
            for (int i = 0; i < trainer.perLoop; i++) {
                sumSquareError = 0;
                foreach (var set in trainingSet) {
                    result = Train(set.input1, set.input2, set.desiredOutput);
                    sumSquareError += Mathf.Pow((float)result[0] - set.desiredOutput, 2); ;
                }
                trainer.soFar++;
            }
            Debug.Log("trained " + trainer.perLoop + " times...");
        } else {
            Debug.Log("Sum square error: " + sumSquareError);
            string debugAfter = "";
            foreach (var set in trainingSet) {
                result = Train(set.input1, set.input2, set.desiredOutput, false);
                debugAfter += "After Training desired result for " + set.input1 + " and " + set.input2 + " is " + set.desiredOutput + " and got " + result[0] + "\n";
            }
            Debug.Log(debugAfter);
            enabled = false;
        }
    }

    List<double> Train(int input1, int input2, int desiredOutput, bool updateWeights = true) {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        inputs.Add(input1);
        inputs.Add(input2);
        outputs.Add(desiredOutput);
        if (updateWeights) return ann.Train(inputs, outputs);
        else return ann.CalcOutput(inputs, outputs);
    }
}
