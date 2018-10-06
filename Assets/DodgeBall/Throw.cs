using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public Material green;
    public Material red;

    GameObject itemCreated;

    DodgeBallPerceptron perceptron;

    void Start() {
        perceptron = GetComponent<DodgeBallPerceptron>();
    }

    // Update is called once per frame
    void Update() {
        System.Random random = new System.Random();
        int itemToThrow = random.Next(2);
        int colorOfItem = random.Next(2);
        int canHit = 1;
        if (itemToThrow == 0 && colorOfItem == 0) canHit = 0;

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (itemToThrow == 0 && colorOfItem == 0) { // red sphere
                itemCreated = setUpItem(spherePrefab, red);
            } else if (itemToThrow == 0 && colorOfItem == 1) { // green sphere
                itemCreated = setUpItem(spherePrefab, green);
            } else if (itemToThrow == 1 && colorOfItem == 0) { // red cube
                itemCreated = setUpItem(cubePrefab, red);
            } else if (itemToThrow == 1 && colorOfItem == 1) { // green cube 
                itemCreated = setUpItem(cubePrefab, green);
            }
            perceptron.SendInput(itemToThrow, colorOfItem, canHit);
        }
    }

    GameObject setUpItem(GameObject prefab, Material color) {
        GameObject item = Instantiate(prefab, Camera.main.transform.position, Camera.main.transform.rotation);
        item.GetComponent<Renderer>().material = color;
        item.GetComponent<Rigidbody>().AddForce(0, 0, 500);
        return item;
    }
}
