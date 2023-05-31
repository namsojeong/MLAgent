using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class MummyRayAgent : Agent
{
    int goodItemCount = 0;

    private Rigidbody rigidBody;
    private StageManager stageManager;

    public float moveSpeed = 1.5f;
    public float turnSpeed = 200f;

    private Renderer floorRenderer;
    public Material goodMaterial, badMaterial;
    private Material originMaterial;

    public override void Initialize()
    {
        MaxStep = 5000;
        rigidBody = GetComponent<Rigidbody>();
        stageManager = transform.parent.GetComponent<StageManager>();

        floorRenderer = transform.parent.Find("Floor").GetComponent<Renderer>();
        originMaterial = floorRenderer.material;

    }
    public override void OnEpisodeBegin()
    {
        goodItemCount = 0;
        stageManager.SetStageObject();

        rigidBody.velocity = rigidBody.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-22.0f, 22.0f), 0.05f, Random.Range(-22.0f, 22.0f));
        transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        // Branch 0 : 정지 / 전진 / 후진
        switch (action[0])
        {
            case 1: dir = transform.forward; break;
            case 2: dir = -transform.forward; break;
        }

        // Branch 1 : 정지 / 좌회전 / 우회전
        switch (action[1])
        {
            case 1: rot = -transform.up; break;
            case 2: rot = transform.up; break;
        }

        transform.Rotate(rot, Time.fixedDeltaTime * turnSpeed);
        rigidBody.AddForce(dir * moveSpeed, ForceMode.VelocityChange);

        // 마이너스 페널티를 적용
        AddReward(-1 / (float)MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        actionsOut.Clear();

        // Branch 0 - 이동로직에 쓸 키 맵핑
        // Branch 0의 Size : 3
        //  정지    /   전진    /   후진
        //  Non     /   W       /   S
        //  0       /   1       /   2
        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2;
        }

        // Branch 1 - 회전로직에 쓸 키 맵핑
        // Branch 1의 Size : 3
        //  정지    /   좌회전  /   우회전
        //  Non     /   A       /   D
        //  0       /   1       /   2
        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("GOOD_ITEM"))
        {
            // GoodItem과 충돌하면 아이템을 먹고 다음 스텝으로 진행함.
            // 가속도가 붙을 수 있기때문에 물리력 초기화 필요.
            goodItemCount++;
            rigidBody.velocity = rigidBody.angularVelocity = Vector3.zero;
            Destroy(collision.gameObject);
            AddReward(1.0f);

            if(goodItemCount >= 30)
            {
            StartCoroutine(RevertMaterial(goodMaterial));
                AddReward(5.0f);
                EndEpisode();
            }
        }

        if (collision.collider.CompareTag("BAD_ITEM"))
        {
            AddReward(-1.0f);
            EndEpisode();

            StartCoroutine(RevertMaterial(badMaterial));
        }

        if (collision.collider.CompareTag("WALL"))
        {
            AddReward(-0.1f);
        }
    }

    private IEnumerator RevertMaterial(Material changeMaterial)
    {
        floorRenderer.material = changeMaterial;
        yield return new WaitForSeconds(0.2f);
        floorRenderer.material = originMaterial;
    }
}
