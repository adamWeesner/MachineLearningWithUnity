using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Kart {
    public float speed = 50.0f;
    public float rotationSpeed = 100.0f;
    public float visibleDistance = 50.0f;
    public float translation = 0f;
    public float rotation = 0;

    public void Move(Transform t, float translationInput, float rotationInput) {
        translation = translationInput * speed * Time.deltaTime;
        rotation = rotationInput * rotationSpeed * Time.deltaTime;
        t.Translate(0, 0, translation);
        t.Rotate(0, rotation, 0);
    }

    public string GetPath(string data) {
        return Application.dataPath + "/Racing NN/" + data + ".txt";

    }

    public string PerformRayCasts(out float f, out float r, out float l, out float r45, out float l45, Transform t) {
        List<float> rays = new List<float>();
        RaycastHit hit;
        f = r = l = r45 = l45 = 0;

        Debug.DrawRay(t.position, t.forward * visibleDistance, Color.red);
        Debug.DrawRay(t.position, t.right * visibleDistance, Color.red);
        Debug.DrawRay(t.position, -t.right * visibleDistance, Color.red);
        Debug.DrawRay(t.position, Quaternion.AngleAxis(45, Vector3.up) * -t.right * visibleDistance, Color.red);
        Debug.DrawRay(t.position, Quaternion.AngleAxis(-45, Vector3.up) * t.right * visibleDistance, Color.red);

        // forward
        if (Physics.Raycast(t.position, t.forward, out hit, visibleDistance))
            f = 1 - Utils.Round(hit.distance / visibleDistance);

        // left
        if (Physics.Raycast(t.position, -t.right, out hit, visibleDistance))
            l = 1 - Utils.Round(hit.distance / visibleDistance);

        // right
        if (Physics.Raycast(t.position, t.right, out hit, visibleDistance))
            r = 1 - Utils.Round(hit.distance / visibleDistance);

        // left 45
        if (Physics.Raycast(t.position, Quaternion.AngleAxis(45, Vector3.up) * -t.right, out hit, visibleDistance))
            l45 = 1 - Utils.Round(hit.distance / visibleDistance);

        // right 45
        if (Physics.Raycast(t.position, Quaternion.AngleAxis(-45, Vector3.up) * t.right, out hit, visibleDistance))
            r45 = 1 - Utils.Round(hit.distance / visibleDistance);

        rays.Add(f);
        rays.Add(r);
        rays.Add(l);
        rays.Add(r45);
        rays.Add(l45);

        return (f + "," + r + "," + l + "," + r45 + "," + l45);
    }

    public List<float> CalculateRayData(Transform t) {
        List<float> rays = new List<float>();
        RaycastHit hit;
        float fDist = 0, rDist = 0, lDist = 0, r45Dist = 0, l45Dist = 0;

        Debug.DrawRay(t.position, t.forward * visibleDistance, Color.red);
        Debug.DrawRay(t.position, t.right * visibleDistance, Color.red);
        Debug.DrawRay(t.position, -t.right * visibleDistance, Color.red);
        Debug.DrawRay(t.position, Quaternion.AngleAxis(45, Vector3.up) * -t.right * visibleDistance, Color.red);
        Debug.DrawRay(t.position, Quaternion.AngleAxis(-45, Vector3.up) * t.right * visibleDistance, Color.red);

        // forward
        if (Physics.Raycast(t.position, t.forward, out hit, visibleDistance))
            fDist = 1 - Utils.Round(hit.distance / visibleDistance);

        // left
        if (Physics.Raycast(t.position, -t.right, out hit, visibleDistance))
            lDist = 1 - Utils.Round(hit.distance / visibleDistance);

        // right
        if (Physics.Raycast(t.position, t.right, out hit, visibleDistance))
            rDist = 1 - Utils.Round(hit.distance / visibleDistance);

        // left 45
        if (Physics.Raycast(t.position, Quaternion.AngleAxis(45, Vector3.up) * -t.right, out hit, visibleDistance))
            l45Dist = 1 - Utils.Round(hit.distance / visibleDistance);

        // right 45
        if (Physics.Raycast(t.position, Quaternion.AngleAxis(-45, Vector3.up) * t.right, out hit, visibleDistance))
            r45Dist = 1 - Utils.Round(hit.distance / visibleDistance);

        rays.Add(fDist);
        rays.Add(rDist);
        rays.Add(lDist);
        rays.Add(r45Dist);
        rays.Add(l45Dist);

        return rays;
    }

    public void SaveWeightsToFile(ANN ann) {
        string path = GetPath("weights");
        StreamWriter wf = File.CreateText(path);
        wf.WriteLine(ann.PrintWeights());
        wf.Close();
    }

    public void LoadWeightsFromFile(ANN ann) {
        string path = GetPath("weights");
        StreamReader wf = File.OpenText(path);
        if (File.Exists(path)) {
            string line = wf.ReadLine();
            ann.LoadWeights(line);
        }
        wf.Close();
    }
}