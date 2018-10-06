using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamoGATraining {
    public class DNA : MonoBehaviour {

        // gene colors
        public float r;
        public float g;
        public float b;

        public bool dead = false;
        public float timeToDie = 0;

        public float xMin = -11f;
        public float xMax = 11f;
        public float yMin = -3.5f;
        public float yMax = 5.5f;

        SpriteRenderer sRenderer;
        Collider2D sCollider;

        void OnMouseDown () {
            dead = true;
            timeToDie = PopulationManager.elapsed;
            Debug.Log ("Dead at: " + timeToDie);
            sRenderer.enabled = false;
            sCollider.enabled = false;
        }

        // Use this for initialization
        void Start () {
            sRenderer = GetComponent<SpriteRenderer> ();
            sCollider = GetComponent<Collider2D> ();
            sRenderer.color = new Color (r, g, b);
        }

        public void SetRGB (DNA parent1, DNA parent2) {
            r = Random.Range (0f, 10f) < 5 ? parent1.r : parent2.r;
            g = Random.Range (0f, 10f) < 5 ? parent1.g : parent2.g;
            b = Random.Range (0f, 10f) < 5 ? parent1.b : parent2.b;
        }

        public void SetRGB () {
            r = Random.Range (0f, 1f);
            g = Random.Range (0f, 1f);
            b = Random.Range (0f, 1f);
        }

        public float GetRandom (string value) {
            if (value == "x") {
                return Random.Range (xMin, xMax);
            } else if (value == "y") {
                return Random.Range (yMin, yMax);
            } else {
                return 0;
            }
        }
    }
}