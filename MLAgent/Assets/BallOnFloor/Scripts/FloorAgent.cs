using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class FloorAgent : Agent
{
    [SerializeField] private GameObject ball;
    private Rigidbody ballRb;

    public override void Initialize()
    {
        ballRb = ball.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        transform.Rotate(new Vector3(1f, 0f, 0f), Random.Range(-10f, 10f));
        transform.Rotate(new Vector3(0f, 0f, 1f), Random.Range(-10f, 10f));

        ballRb.velocity = new Vector3(0f, 0f, 0f);
        ball.transform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), 1.5f, Random.Range(-1.5f, 1.5f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.rotation.z); // 1개
        sensor.AddObservation(transform.rotation.x); // 1개
        sensor.AddObservation(ball.transform.position - transform.position); // 3개
        sensor.AddObservation(ballRb.velocity); // 3개
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float z_rotation =  Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float x_rotation =  Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        transform.Rotate(new Vector3(0, 0, 1), z_rotation);
        transform.Rotate(new Vector3(1, 0, 0), x_rotation);

        if(DropBall())
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ContinuousActions = actionsOut.ContinuousActions;
        ContinuousActions[0] = -Input.GetAxis("Horizontal");
        ContinuousActions[1] = -Input.GetAxis("Vertical");
    }

    private bool DropBall()
    {
        return (ball.transform.position.y - transform.position.y) < -2f ||
        Mathf.Abs(ball.transform.position.x - transform.position.x) > 2.5f ||
        Mathf.Abs(ball.transform.position.z - transform.position.z) > 2.5f;
    }
}
