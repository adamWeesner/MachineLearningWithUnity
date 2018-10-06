using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CamoGATraining {
    public class PopulationManager : MonoBehaviour {

        public GameObject personPrefab;
        public int populationSize = 10;
        List<GameObject> population = new List<GameObject> ();
        public static float elapsed = 0;
        int trialTime = 10;
        int generation = 1;

        GUIStyle guiStyle = new GUIStyle ();
        void OnGUI () {
            guiStyle.fontSize = 18;
            guiStyle.normal.textColor = Color.white;
            GUI.Label (new Rect (10, 10, 100, 20), "Generation: " + generation, guiStyle);
            GUI.Label (new Rect (10, 65, 100, 20), "Trial Time: " + (int) elapsed, guiStyle);
        }

        // Use this for initialization
        void Start () {
            for (int i = 0; i < populationSize; i++) {
                Vector3 pos = new Vector3 (GetDNA (personPrefab).GetRandom ("x"), GetDNA (personPrefab).GetRandom ("y"), 0);
                GameObject go = Instantiate (personPrefab, pos, Quaternion.identity);
                GetDNA (go).SetRGB ();
                population.Add (go);
            }
        }

        // Update is called once per frame
        void Update () {
            elapsed += Time.deltaTime;
            if (elapsed > trialTime) {
                BreedNewPopulation ();
                elapsed = 0;
            }

            bool stillSomeLeft = false;
            for (int i = 0; i < population.Count; i++) {
                if (GetDNA (population[i]).dead == false) {
                    stillSomeLeft = true;
                    break;
                }
            }

            if (!stillSomeLeft) {
                BreedNewPopulation ();
                elapsed = 0;
            }
        }

        GameObject Breed (GameObject parent1, GameObject parent2) {
            Vector3 pos = new Vector3 (GetDNA (personPrefab).GetRandom ("x"), GetDNA (personPrefab).GetRandom ("y"), 0);
            GameObject offspring = Instantiate (personPrefab, pos, Quaternion.identity);
            DNA dna1 = GetDNA (parent1);
            DNA dna2 = GetDNA (parent2);
            if (Random.Range (0, 10) < 9.9) {
                GetDNA (offspring).SetRGB (dna1, dna2);
            } else {
                GetDNA (offspring).SetRGB ();
            }
            return offspring;
        }

        void BreedNewPopulation () {
            List<GameObject> newPopulation = new List<GameObject> ();
            List<GameObject> sortedList = population.OrderBy (o => GetDNA (o).timeToDie).ToList ();

            population.Clear ();

            for (int i = (int) (sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++) {
                population.Add (Breed (sortedList[i], sortedList[i + 1]));
                population.Add (Breed (sortedList[i + 1], sortedList[i]));
            }

            for (int i = 0; i < sortedList.Count; i++) {
                Destroy (sortedList[i]);
            }
            generation++;
        }

        DNA GetDNA (GameObject person) {
            return person.GetComponent<DNA> ();
        }
    }
}