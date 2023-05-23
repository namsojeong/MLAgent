using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MummyAgent : Agent
{
    private Rigidbody rigidBody;
    private Transform targetTransform;

    public Material goodMaterial;
    public Material badMaterial;
    private Material originMaterial;
    private Renderer floorRenderer;

    public override void Initialize()
    {
        rigidBody = GetComponent<Rigidbody>();
        targetTransform = transform.parent.Find("Target");

        floorRenderer = transform.parent.Find("Floor").GetComponent<Renderer>();
        originMaterial = floorRenderer.material;
    }

    public override void OnEpisodeBegin()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        transform.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.05f, Random.Range(-4.0f, 4.0f));
        targetTransform.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.55f, Random.Range(-4.0f, 4.0f));

        StartCoroutine(ReverMaterial());
    }

    IEnumerator ReverMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        floorRenderer.material = originMaterial;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ÃÑ 8°³ °üÃø
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rigidBody.velocity.x);
        sensor.AddObservation(rigidBody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.ContinuousActions;

        Vector3 dir = (Vector3.forward * action[0]) + (Vector3.right * action[1]);
        rigidBody.AddForce(dir.normalized * 50.0f);

        SetReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;
        action[0] = Input.GetAxis("Vertical");
        action[1] = Input.GetAxis("Horizontal");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("DeadZone"))
        {
            floorRenderer.material = badMaterial;
            SetReward(-1.0f);
            EndEpisode();
        }

        if (collision.collider.CompareTag("Target"))
        {
            floorRenderer.material = goodMaterial;
            SetReward(1.0f);
            EndEpisode();
        }
    }
}
