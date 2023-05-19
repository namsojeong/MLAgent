using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public float fishSpeed;
    private float randomizedSpeed = 0f;
    private float nextActionTime = -1f;
    private Vector3 targetPosition;

    private void FixedUpdate()
    {
        if(fishSpeed>0f)
        {
            Swim();
        }
    }

    private void Swim()
    {
        if(Time.fixedTime >= nextActionTime)
        {
            randomizedSpeed = fishSpeed * Random.Range(0.5f, 1.5f);

            targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, 100, 260f, 2f, 13f) + Vector3.up * 0.5f;
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

            float timeToGetThere=Vector3.Distance(transform.position, targetPosition) / randomizedSpeed;
            nextActionTime = Time.deltaTime + timeToGetThere;
        }
        else
        {
            Vector3 moveVector = randomizedSpeed * transform.forward * Time.fixedDeltaTime;
            if(moveVector.magnitude <= Vector3.Distance(transform.position, targetPosition))
            {
                transform.position += moveVector;
            }
            else
            {
                transform.position = targetPosition;

                nextActionTime= Time.fixedDeltaTime;
            }
        }
    }
}
