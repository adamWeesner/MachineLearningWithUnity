using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TrainingSet {
    public double[] input = new double[2];
    public double output;
    [HideInInspector]
    public double actualOutput;
    [HideInInspector]
    public double actualAdjustedOutput;
}

public abstract class GenericPerceptron : MonoBehaviour {
    [Tooltip("The max number of iterations the Perceptron will go through if a Total Error of 0 is not reached sooner")]
    public int timesToTrain = 8;
    public TrainingSet[] trainingSet;

    public double[] weights;
    public double bias = 0;
    [HideInInspector]
    public double totalError = 0;
    [HideInInspector]
    public string name;

    public void InitializeValues() {
        if (weights.Length == 0) weights = new double[trainingSet[0].input.Length];

        for (int i = 0; i < weights.Length; i++)
            if (weights[i] == 0) weights[i] = Random.Range(-1f, 1f);

        if (bias == 0) bias = Random.Range(-1f, 1f);
    }

    public double DotProductBias(double[] weights, double[] inputs) {
        if (weights == null || inputs == null || (weights.Length != inputs.Length)) return -1;

        double dpb = 0;
        for (int index = 0; index < weights.Length; index++)
            dpb += weights[index] * inputs[index];


        dpb += bias;

        return dpb;
    }

    public double CalcOutput(int index) {
        double dpb = DotProductBias(weights, trainingSet[index].input);
        trainingSet[index].actualAdjustedOutput = dpb;
        if (dpb > 0) trainingSet[index].actualOutput = 1;
        else trainingSet[index].actualOutput = 0;

        return trainingSet[index].actualOutput;
    }

    public double CalcOutput(TrainingSet trainingSet) {
        double dpb = DotProductBias(weights, trainingSet.input);
        trainingSet.actualAdjustedOutput = dpb;
        if (dpb > 0) trainingSet.actualOutput = 1;
        else trainingSet.actualOutput = 0;

        return trainingSet.actualOutput;
    }

    public void UpdateWeights(int index) {
        print(trainingSet.GetType());
        double error = trainingSet[index].output - CalcOutput(index);
        totalError += Mathf.Abs((float)error);

        for (int i = 0; i < weights.Length; i++)
            weights[i] = weights[i] + (error * trainingSet[index].input[i]);

        bias += error;
    }

    public void UpdateWeights(TrainingSet trainingSet) {
        double error = trainingSet.output - CalcOutput(trainingSet);
        totalError += Mathf.Abs((float)error);

        for (int i = 0; i < weights.Length; i++)
            weights[i] = weights[i] + (error * trainingSet.input[i]);

        bias += error;
    }

    public virtual void Train() {
        for (int epoch = 0; epoch < timesToTrain; epoch++) {
            if (epoch != 0 && totalError == 0) break;
            totalError = 0;
            for (int set = 0; set < trainingSet.Length; set++) UpdateWeights(set);
            DebugWeightAndTest();
        }
    }

    public abstract void MoreStartBeforeTrain();
    public abstract void MoreStartAfterTrain();

    void Start() {
        SetName();
        MoreStartBeforeTrain();
        InitializeValues();
        Train();
        MoreStartAfterTrain();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            SaveWeights();
        } else if (Input.GetKeyDown(KeyCode.L)) {
            LoadWeights();
        }
    }

    public void LoadWeights() {
        string path = Path.Combine(Application.dataPath, name);
        path = Path.Combine(path, "Weights.txt");
        if (File.Exists(path)) {
            var sr = File.OpenText(path);
            string line = sr.ReadLine();
            string[] weight = line.Split(',');
            for (int index = 0; index < weight.Length - 2; index++)
                weights[index] = System.Convert.ToDouble(weight[index]);

            bias = System.Convert.ToDouble(weight[weight.Length - 1]);
            Debug.Log("Loaded Weights and Bias from file: " + path);
        }
    }

    public void SaveWeights() {
        string path = Path.Combine(Application.dataPath, name);
        path = Path.Combine(path, "Weights.txt");
        var sr = File.CreateText(path);
        string values = "";
        for (int index = 0; index < weights.Length; index++)
            values += weights[index] + ",";

        values += bias;
        sr.WriteLine(values);
        sr.Close();
        Debug.Log("Saved Weight and Bias values to file: " + path);
    }

    public abstract void SetName();

    public void DebugWeightAndTest() {
        string weightInfo = "Testing Weights and Error\n\n";
        for (int weight = 0; weight < weights.Length; weight++)
            weightInfo += "W" + (weight + 1) + ": " + weights[weight] + " ";

        weightInfo += "B: " + bias;

        string testInfo = "\n";
        for (int set = 0; set < trainingSet.Length; set++) {
            for (int input = 0; input < trainingSet[set].input.Length; input++) {
                testInfo += trainingSet[set].input[input];
                testInfo += input == (trainingSet[set].input.Length - 1) ? " " : " and ";
            }

            testInfo += "are " + trainingSet[set].output;
            testInfo += " Got " + trainingSet[set].actualOutput;
            testInfo += " with adjustment of " + trainingSet[set].actualAdjustedOutput;
            testInfo += "\n";
        }

        Debug.Log(weightInfo + testInfo + "Total Error: " + totalError + "\n");
    }
}
