using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drive : MonoBehaviour {
    public Kart kart;
    List<string> collectedTrainingData = new List<string>();
    StreamWriter tdf;

    private void Start() {
        tdf = File.CreateText(Application.dataPath + "/Racing NN/trainingData.txt");
    }

    private void OnApplicationQuit() {
        foreach (string td in collectedTrainingData) {
            tdf.WriteLine(td);
        }

        tdf.Close();
    }

    void Update() {
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");
        kart.Move(this.transform, translationInput, rotationInput);

        List<float> rays = kart.CalculateRayData(this.transform);

        string td = rays[0] + "," + rays[1] + "," + rays[2] + "," + rays[3] + "," + rays[4] + "," + Utils.Round(translationInput) + "," + Utils.Round(rotationInput);
        if (!collectedTrainingData.Contains(td))
            collectedTrainingData.Add(td);
    }
}
