using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Replay {
    public List<double> states;
    public double reward;

    public Replay(double xRotation, double ballZ, double ballVelocityX, double reward) {
        states = new List<double>();
        states.Add(xRotation);
        states.Add(ballZ);
        states.Add(ballVelocityX);
        this.reward = reward;
    }
}

public class BalanceBrain : MonoBehaviour {
    public float timeScale = 5.0f;
    public GameObject ball;                         // object to monitor
    public ANN ann;
    public ANNBuilder aNNBuilder;

    float reward = 0.0f;                            // reward to associate with actions
    List<Replay> replayMemory = new List<Replay>(); // memory - list of past actions and rewards
    int memCap = 10000;                             // memory capacity

    float discount = 0.99f;                         // how much future states affect rewards
    float exploreRate = 100.0f;                     // chance of picking random action
    float maxExploreRate = 100.0f;                  // max chance value
    float minExploreRate = 0.01f;                   // min chance value
    float exploreDecay = 0.0001f;                   // chance decay amount for each update

    Vector3 ballStartPos;                           // record start position of object
    int failCount = 0;                              // count when the ball is dropped
    float tiltSpeed = 0.5f;                         /* max angle to apply to tilting each update, make sure this is large enough
                                                       so that the Q value multiplied by it is large enough to recover balance 
                                                       when the ball gets a good speed up
                                                    */
    float timer = 0;                                // timer to keep track of balancing
    float maxBalanceTime = 0;                       // record time ball is kept balanced

    GUIStyle guiStyle = new GUIStyle();
    private void OnGUI() {
        int itemLoc = 0;
        int locStep = 25;
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + exploreRate, guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best Balance: " + maxBalanceTime, guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This Balance: " + timer, guiStyle);
        GUI.EndGroup();
    }

    // Use this for initialization
    void Start() {
        ann = new ANN(aNNBuilder.inputs, aNNBuilder.hidden, aNNBuilder.outputs, aNNBuilder.neuronsPerHidden, aNNBuilder.alpha, aNNBuilder.hiddenFunction, aNNBuilder.outputFunction, aNNBuilder.useWeightsFromFile, aNNBuilder.folder);
        ballStartPos = ball.transform.position;
        Time.timeScale = timeScale;

        if (aNNBuilder.useWeightsFromFile)
            ann.LoadWeightsFromFile();

        Debug.Log(ann.PrintWeights());
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            ResetBall();

        if (Input.GetKeyDown(KeyCode.S))
            ann.SaveWeightsToFile();

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FixedUpdate() {
        timer += Time.deltaTime;
        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        states.Add(this.transform.rotation.x);
        states.Add(ball.transform.position.z);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);

        qs = ann.SoftMax(ann.CalcOutput(states));
        double maxQ = qs.Max();
        Debug.Log("quality: " + maxQ);

        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        //if (Random.Range(0, 100) < exploreRate)
        //    maxQIndex = Random.Range(0, 2);

        if (maxQIndex == 0)
            this.transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
        else if (maxQIndex == 1)
            this.transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);

        if (ball.GetComponent<BallState>().dropped)
            reward = -1f;
        else
            reward = 0.1f;

        Replay lastMemory = new Replay(
                this.transform.rotation.x,
                ball.transform.position.z,
                ball.GetComponent<Rigidbody>().angularVelocity.x,
                reward
            );

        if (replayMemory.Count > memCap)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);

        if (ball.GetComponent<BallState>().dropped) {
            for (int i = replayMemory.Count - 1; i >= 0; i--) {
                List<double> outputsOld = new List<double>();
                List<double> outputsNew = new List<double>();

                outputsOld = ann.SoftMax(ann.CalcOutput(replayMemory[i].states));

                double maxQOld = outputsOld.Max();
                int action = outputsOld.ToList().IndexOf(maxQOld);
                double feedback;

                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1) {
                    feedback = replayMemory[i].reward;
                } else {
                    outputsNew = ann.SoftMax(ann.CalcOutput(replayMemory[i + 1].states));
                    maxQ = outputsNew.Max();
                    feedback = replayMemory[i].reward + discount * maxQ;
                }

                outputsOld[action] = feedback;
                ann.Train(replayMemory[i].states, outputsOld);
            }
            ResetBall();
            failCount++;
        }

    }

    void ResetBall() {
        ball.transform.position = ballStartPos;
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        ball.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);

        ball.GetComponent<BallState>().dropped = false;
        this.transform.rotation = Quaternion.identity;
        replayMemory.Clear();

        if (timer > maxBalanceTime)
            maxBalanceTime = timer;

        timer = 0;
    }
}
