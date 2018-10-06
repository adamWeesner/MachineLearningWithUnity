using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPerceptron : GenericPerceptron {
    public override void MoreStartAfterTrain() { }

    public override void MoreStartBeforeTrain() { }

    public override void SetName() { name = "Perceptron"; }
}
