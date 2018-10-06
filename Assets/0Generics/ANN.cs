using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ActivationFunctions { step, sigmoid, tanh, reLU, leakyReLU, sinusoid, arcTan, softsign }

[System.Serializable]
public class ANNBuilder {
    public int inputs;
    public int hidden;
    public int outputs;
    public int neuronsPerHidden;
    public double alpha;
    public ActivationFunctions hiddenFunction;
    public ActivationFunctions outputFunction;
}

public class ANN {
    public int numInputs;
    public int numOutputs;
    public int numHidden;
    public int numNeuronsPerHidden;
    public double alpha;
    List<Layer> layers = new List<Layer>();
    public ActivationFunctions hiddenFunction;
    public ActivationFunctions outputFunction;

    public ANN(int numInputs, int numHidden, int numOutputs, int numNeuronsPerHidden, double alpha, ActivationFunctions hidden, ActivationFunctions output) {
        this.numInputs = numInputs;
        this.numHidden = numHidden;
        this.numOutputs = numOutputs;
        this.numNeuronsPerHidden = numNeuronsPerHidden;
        this.alpha = alpha;
        this.hiddenFunction = hidden;
        this.outputFunction = output;

        if (numHidden > 0) {
            // creates input layer
            layers.Add(new Layer(numNeuronsPerHidden, numInputs));
            // creates all hidden layers
            for (int i = 0; i < numHidden - 1; i++)
                layers.Add(new Layer(numNeuronsPerHidden, numNeuronsPerHidden));
            // creates output layer
            layers.Add(new Layer(numOutputs, numNeuronsPerHidden));
        } else {
            // creates input and output layers only
            layers.Add(new Layer(numOutputs, numInputs));
        }
    }

    public List<double> Train(List<double> inputValues, List<double> desiredOutput) {
        List<double> outputValues = new List<double>();
        outputValues = CalcOutput(inputValues, desiredOutput);
        UpdateWeights(outputValues, desiredOutput);
        return outputValues;
    }

    public List<double> CalcOutput(List<double> inputValues, List<double> desiredOutput) {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        int currentInput = 0;

        if (inputValues.Count != numInputs) {
            Debug.Log("Error! Number of inputs must be " + numInputs);
            return outputs;
        }

        inputs = new List<double>(inputValues);
        for (int layer = 0; layer < numHidden + 1; layer++) {
            if (layer > 0) inputs = new List<double>(outputs);
            outputs.Clear();

            for (int neuron = 0; neuron < layers[layer].numNeurons; neuron++) {
                double N = 0;
                layers[layer].neurons[neuron].inputs.Clear();

                for (int input = 0; input < layers[layer].neurons[neuron].numInputs; input++) {
                    layers[layer].neurons[neuron].inputs.Add(inputs[input]);
                    N += layers[layer].neurons[neuron].weights[input] * inputs[input];
                    currentInput++;
                }

                N -= layers[layer].neurons[neuron].bias;
                if (layer == numHidden)
                    layers[layer].neurons[neuron].output = ActivationFunctionO(N);
                else
                    layers[layer].neurons[neuron].output = ActivationFunction(N);

                outputs.Add(layers[layer].neurons[neuron].output);
                currentInput = 0;
            }
        }

        return outputs;
    }

    public string PrintWeights() {
        string weights = "";
        foreach (Layer layer in layers) {
            foreach (Neuron neuron in layer.neurons) {
                foreach (double weight in neuron.weights) {
                    weights += weight + ",";
                }
            }
        }
        return weights;
    }

    public void LoadWeights(string weights) {
        if (weights == "") return;

        string[] weightValues = weights.Split(',');
        int weightValue = 0;
        foreach (Layer layer in layers) {
            foreach (Neuron neuron in layer.neurons) {
                for (int weight = 0; weight < neuron.weights.Count; weight++) {
                    neuron.weights[weight] = System.Convert.ToDouble(weightValues[weightValue]);
                }
            }
        }
    }

    void UpdateWeights(List<double> outputs, List<double> desiredOutput) {
        double error;
        // back propegation
        for (int layer = numHidden; layer >= 0; layer--) {
            for (int neuron = 0; neuron < layers[layer].numNeurons; neuron++) {
                if (layer == numHidden) {
                    error = desiredOutput[neuron] - outputs[neuron];
                    // this is the error gradient calculated with the Delta Rule: http://en.wikipedia.org/wiki/Delta_rule
                    layers[layer].neurons[neuron].errorGradient = outputs[neuron] * (1 - outputs[neuron]) * error;
                } else {
                    layers[layer].neurons[neuron].errorGradient = layers[layer].neurons[neuron].output * (1 - layers[layer].neurons[neuron].output);
                    double errorGradSum = 0;
                    for (int nextLayerNeuron = 0; nextLayerNeuron < layers[layer + 1].numNeurons; nextLayerNeuron++)
                        errorGradSum += layers[layer + 1].neurons[nextLayerNeuron].errorGradient * layers[layer + 1].neurons[nextLayerNeuron].weights[neuron];

                    layers[layer].neurons[neuron].errorGradient *= errorGradSum;
                }

                for (int input = 0; input < layers[layer].neurons[neuron].numInputs; input++) {
                    if (layer == numHidden) {
                        error = desiredOutput[neuron] - outputs[neuron];
                        layers[layer].neurons[neuron].weights[input] += alpha * layers[layer].neurons[neuron].inputs[input] * error;
                    } else {
                        layers[layer].neurons[neuron].weights[input] += alpha * layers[layer].neurons[neuron].inputs[input] * layers[layer].neurons[neuron].errorGradient;
                    }
                }

                layers[layer].neurons[neuron].bias += alpha * -1 * layers[layer].neurons[neuron].errorGradient;
            }
        }
    }

    // more activation functions http://en.wikipedia.org/wiki/Activation_function
    double ActivationFunction(double value) {
        return ActivationFunctionGet(hiddenFunction, value);
    }

    double ActivationFunctionO(double value) {
        return ActivationFunctionGet(outputFunction, value);
    }

    double ActivationFunctionGet(ActivationFunctions function, double value) {
        if (function == ActivationFunctions.step) return Step(value);
        else if (function == ActivationFunctions.sigmoid) return Sigmoid(value);
        else if (function == ActivationFunctions.tanh) return TanH(value);
        else if (function == ActivationFunctions.reLU) return ReLU(value);
        else if (function == ActivationFunctions.leakyReLU) return LeakyReLu(value);
        else if (function == ActivationFunctions.sinusoid) return Sinusoid(value);
        else if (function == ActivationFunctions.arcTan) return ArcTan(value);
        else if (function == ActivationFunctions.softsign) return Softsign(value);
        else throw new System.ArgumentException("Activation function is not valid");
    }

    // binary step activation function
    double Step(double value) {
        if (value < 0) return 0;
        else return 1;
    }

    // logistic softstep activation function
    double Sigmoid(double value) {
        double k = (double)System.Math.Exp(value);
        return k / (1.0f + k);
    }

    // tanH activation function
    double TanH(double value) {
        return 2 * (Sigmoid(2 * value)) - 1;
    }

    // rectified linear unit activation function
    double ReLU(double value) {
        if (value > 0) return value;
        else return 0;
    }

    // leaky rectified linear activation function
    double LeakyReLu(double value) {
        if (value < 0) return 0.01 * value;
        else return value;
    }

    // sinusoid activation function
    double Sinusoid(double value) {
        return Mathf.Sin((float)value);
    }

    // arc tan activation function
    double ArcTan(double value) {
        return Mathf.Pow(Mathf.Tan((float)value), -1);
    }

    // softsign activation function
    double Softsign(double value) {
        return value / (1 + Mathf.Abs((float)value));
    }
}