using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paddle {
    public GameObject it;
    public float minY = 8.5f;
    public float maxY = 17.7f;
    public float maxSpeed = 15;
    public float numSaved = 0;
    public float numMissed = 0;
}

public class PongBrain : MonoBehaviour {
    public Paddle paddle;
    public GameObject ball;
    Rigidbody2D ballRB;
    float yVel;

    public ANNBuilder aNNBuilder;
    public ANN ann;


    // Use this for initialization
    void Start() {
        ann = new ANN(aNNBuilder.inputs, aNNBuilder.hidden, aNNBuilder.outputs, aNNBuilder.neuronsPerHidden, aNNBuilder.alpha, aNNBuilder.hiddenFunction, aNNBuilder.outputFunction);
        ballRB = ball.GetComponent<Rigidbody2D>();
    }

    List<double> Run(double ballX, double ballY, double ballVelX, double ballVelY, double paddleX, double paddleY, double paddleVel, bool train) {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        inputs.Add(ballX);
        inputs.Add(ballY);
        inputs.Add(ballVelX);
        inputs.Add(ballVelY);
        inputs.Add(paddleX);
        inputs.Add(paddleY);
        outputs.Add(paddleVel);

        if (train) return ann.Train(inputs, outputs);
        else return ann.CalcOutput(inputs, outputs);
    }

    void Update() {
        float paddleY = Mathf.Clamp(paddle.it.transform.position.y + (yVel * Time.deltaTime * paddle.maxSpeed), paddle.minY, paddle.maxY);
        paddle.it.transform.position = new Vector3(paddle.it.transform.position.x, paddleY, paddle.it.transform.position.z);

        List<double> output = new List<double>();
        int layerMask = 1 << 9;
        RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, ballRB.velocity, 1000, layerMask);
        if (hit.collider != null) {
            if (hit.collider.gameObject.tag == "tops") {
                Vector3 reflection = Vector3.Reflect(ballRB.velocity, hit.normal);
                hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
            }

            if (hit.collider != null && hit.collider.gameObject.tag == "backwall") {
                float dy = hit.point.y - paddle.it.transform.position.y;
                output = Run(
                        ball.transform.position.x,
                        ball.transform.position.y,
                        ballRB.velocity.x,
                        ballRB.velocity.y,
                        paddle.it.transform.position.x,
                        paddle.it.transform.position.y,
                        dy,
                        true
                    );
                yVel = (float)output[0];
            }
        } else {
            yVel = 0;
        }
    }
}
