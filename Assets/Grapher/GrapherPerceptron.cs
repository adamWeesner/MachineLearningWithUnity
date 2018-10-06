using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapherPerceptron : GenericPerceptron {
    public SimpleGrapher sg;

    double CalcOutput(double i1, double i2) {
        double[] inp = new double[] { i1, i2 };
        double dp = DotProductBias(weights, inp);
        if (dp >= 0) return 1;
        return 0;
    }

    void DrawAllPoints() {
        for (int set = 0; set < trainingSet.Length; set++) {
            if (trainingSet[set].output == 0) sg.DrawPoint((float)trainingSet[set].input[0], (float)trainingSet[set].input[1], Color.magenta);
            else sg.DrawPoint((float)trainingSet[set].input[0], (float)trainingSet[set].input[1], Color.green);
        }
    }

    public override void MoreStartBeforeTrain() {
        totalError = -1;
        DrawAllPoints();
    }

    public override void MoreStartAfterTrain() {
        sg.DrawRay((float)(-(bias / weights[1]) / (bias / weights[0])), (float)(-bias / weights[1]), Color.red);

        if (CalcOutput(0.3, 0.9) == 0) sg.DrawPoint(0.3f, 0.9f, Color.red);
        else sg.DrawPoint(0.3f, 0.9f, Color.yellow);

        if (CalcOutput(0.8, 0.1) == 0) sg.DrawPoint(0.8f, 0.1f, Color.red);
        else sg.DrawPoint(0.8f, 0.1f, Color.yellow);
    }

    public override void SetName() { name = "Grapher"; }
}
