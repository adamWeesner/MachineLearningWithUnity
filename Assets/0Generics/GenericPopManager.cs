using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GenericPopManager : MonoBehaviour {
    public GameObject botPrefab;
    protected GameObject startingPos;
    public int populationSize = 50;
    protected List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public float trialTime = 5;
    protected int generation = 1;
    protected int populationSplit = 2;

    public GenericPopManager() { }

    public GenericPopManager(GameObject startingPos) {
        this.startingPos = startingPos;
    }

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI() {
        guiStyle.fontSize = 18;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + populationSize, guiStyle);
        GUI.EndGroup();
    }

    void Start() {
        for (int i = 0; i < populationSize; i++) {
            GameObject bot = CreateBot();
            GetBrain<GenericBrain>(bot).Init();
            population.Add(bot);
        }
    }

    void Update() {
        elapsed += Time.deltaTime;
        if (elapsed >= trialTime) {
            BreedNewPopulation();
            elapsed = 0;
            return;
        }

        bool stillSomeLeft = false;
        for (int i = 0; i < population.Count; i++) {
            if (GetBrain<GenericBrain>(population[i]).alive) {
                stillSomeLeft = true;
                break;
            }
        }

        if (!stillSomeLeft) {
            BreedNewPopulation();
            elapsed = 0;
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2) {
        GameObject offspring = CreateBot();
        GenericBrain b = GetBrain<GenericBrain>(offspring);
        b.Init();

        if (Random.Range(0, 100) == 1) b.dna.Mutate();
        else b.dna.Combine(GetBrain<GenericBrain>(parent1).dna, GetBrain<GenericBrain>(parent2).dna);

        return offspring;
    }

    public abstract void MoreBreedPop();
    public abstract float BreedSortCondition(GameObject o);

    void BreedNewPopulation() {
        List<GameObject> sortedList = population.OrderBy(o => BreedSortCondition(o)).ToList();
        population.Clear();

        for (int i = (int)(sortedList.Count / populationSplit) - 1; i < sortedList.Count - 1; i++)
            for (int j = 0; j < populationSplit; j++)
                population.Add(Breed(sortedList[i], sortedList[i + 1]));

        for (int i = 0; i < sortedList.Count; i++) Destroy(sortedList[i]);
        generation++;
        MoreBreedPop();
    }

    public Vector3 RandomPos() {
        return new Vector3(
            this.transform.position.x + Random.Range(-2, 2),
            this.transform.position.y,
            this.transform.position.z + Random.Range(-2, 2));
    }

    public GameObject CreateBot() {
        if (startingPos != null) return Instantiate(botPrefab, startingPos.transform.position, this.transform.rotation);
         else return Instantiate(botPrefab, RandomPos(), this.transform.rotation);
    }

    public T GetBrain<T>(GameObject bot) {
        return (T)bot.GetComponent<T>();
    }
}