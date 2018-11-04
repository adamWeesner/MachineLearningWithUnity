using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class ANNDrive : MonoBehaviour {
    public ANN ann;
    public ANNBuilder aNNBuilder;
    public Kart kart;

    public int epochs = 1000;
    bool trainingDone = false;
    float trainingProgress = 0;
    double sse = 0;
    double lastSSE = 1;

    private void OnGUI() {
        GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE);
        GUI.Label(new Rect(25, 40, 250, 30), "Aplha: " + ann.alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + trainingProgress + "%");
    }

    void Start() {
        ann = new ANN(aNNBuilder.inputs, aNNBuilder.hidden, aNNBuilder.outputs, aNNBuilder.neuronsPerHidden, aNNBuilder.alpha, aNNBuilder.hiddenFunction, aNNBuilder.outputFunction, aNNBuilder.useWeightsFromFile, aNNBuilder.folder);
        if (ann.useFileWeights) {
            ann.LoadWeightsFromFile();
            trainingDone = true;
        } else {
            StartCoroutine(LoadTrainingSet());
        }
    }

    void Update() {
        if (!trainingDone) return;

        List<double> calcOutputs = new List<double>();
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        float fDist = 0, rDist = 0, lDist = 0, r45Dist = 0, l45Dist = 0;

        kart.PerformRayCasts(out fDist, out rDist, out lDist, out r45Dist, out l45Dist, this.transform);

        inputs.Add(fDist);
        inputs.Add(rDist);
        inputs.Add(lDist);
        inputs.Add(r45Dist);
        inputs.Add(l45Dist);
        outputs.Add(0);
        outputs.Add(0);
        calcOutputs = ann.CalcOutput(inputs, outputs);

        float translationInput = Utils.Map(-1, 1, 0, 1, (float)calcOutputs[0]);
        float rotationInput = Utils.Map(-1, 1, 0, 1, (float)calcOutputs[1]);
        kart.Move(this.transform, translationInput, rotationInput);
    }

    IEnumerator LoadTrainingSet() {
        string path = ann.GetPath("trainingData");
        string line;
        if (File.Exists(path)) {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader tdf = File.OpenText(path);

            List<double> calcOutputs = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            for (int i = 0; i < epochs; i++) {
                // set file pointer to beginning of file
                sse = 0;
                tdf.BaseStream.Position = 0;
                string currentWeights = ann.PrintWeights();
                while ((line = tdf.ReadLine()) != null) {
                    string[] data = line.Split(',');
                    // if nothing to be learned ignore this line
                    float thisError = 0;
                    if (System.Convert.ToDouble(data[5]) != 0 && System.Convert.ToDouble(data[6]) != 0) {
                        inputs.Clear();
                        outputs.Clear();
                        inputs.Add(System.Convert.ToDouble(data[0]));
                        inputs.Add(System.Convert.ToDouble(data[1]));
                        inputs.Add(System.Convert.ToDouble(data[2]));
                        inputs.Add(System.Convert.ToDouble(data[3]));
                        inputs.Add(System.Convert.ToDouble(data[4]));

                        double out1 = Utils.Map(0, 1, -1, 1, System.Convert.ToSingle(data[5]));
                        double out2 = Utils.Map(0, 1, -1, 1, System.Convert.ToSingle(data[6]));
                        outputs.Add(out1);
                        outputs.Add(out2);

                        calcOutputs = ann.Train(inputs, outputs);
                        thisError = ((Mathf.Pow((float)(outputs[0] - calcOutputs[0]), 2) +
                            Mathf.Pow((float)(outputs[1] - calcOutputs[1]), 2))) / 2.0f;
                    }
                    sse += thisError;
                }
                trainingProgress = ((float)i / (float)epochs) * 100;
                sse /= lineCount;

                if (lastSSE < sse) { // if sse isnt better reload old one and decrease alpha
                    ann.LoadWeights(currentWeights);
                    ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f, 0.01f, 0.9f);
                } else { // increase alpha
                    ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f, 0.01f, 0.9f);
                    lastSSE = sse;
                }
                yield return null;
            }
            tdf.Close();
        }
        trainingDone = true;
        ann.SaveWeightsToFile();
    }
}
