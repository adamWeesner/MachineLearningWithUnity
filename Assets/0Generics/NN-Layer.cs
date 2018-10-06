using System.Collections.Generic;

public class Layer {
    public int numNeurons;
    public List<Neuron> neurons = new List<Neuron>();

    public Layer(int numNeurons, int numNeuronInputs) {
        this.numNeurons = numNeurons;
        for (int i = 0; i < numNeurons; i++)
            neurons.Add(new Neuron(numNeuronInputs));
    }
}
