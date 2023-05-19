using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PenguinAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;

    public GameObject heartPrefab;
    public GameObject regurgitatedFishPrefab;

    private PenguinArea penguinArea;
    private Rigidbody rigidBody;
    private GameObject baby;

    private bool isFull;

    public override void Initialize()
    {
        penguinArea = GetComponentInParent<PenguinArea>();
        rigidBody = GetComponent<Rigidbody>();
        baby = penguinArea.penguinBaby;
    }

    public override void OnEpisodeBegin()
    {
        isFull = false;
        penguinArea.ResetArea();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 8��
        sensor.AddObservation(isFull); // 1��
        sensor.AddObservation(Vector3.Distance(baby.transform.position, transform.position)); // 1��
        sensor.AddObservation((baby.transform.position - transform.position).normalized); // 3��
        sensor.AddObservation(transform.forward); // 3��
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        forwardAmount = actions.DiscreteActions[0];
        float turnAmount = 0f;

        if (actions.DiscreteActions[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (actions.DiscreteActions[1]== 2f)
        {
            turnAmount = 1f;
        }

        rigidBody.MovePosition(transform.position + transform.forward * forwardAmount);
        transform.Rotate(transform.up * turnAmount*turnSpeed*Time.fixedDeltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        int turnAction = 0;

        if(Input.GetKey(KeyCode.W))
        {
            forwardAction = 1;
        }

        if(Input.GetKey(KeyCode.A))
        {
            turnAction = 1;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            turnAction = 2;
        }

        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
    }


}