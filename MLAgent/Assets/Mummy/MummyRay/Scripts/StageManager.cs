using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject goodItem;
    public GameObject badItem;

    public int goodItemCount = 30;
    public int badItemCount = 10;

    public List<GameObject> goodList = new List<GameObject>();
    public List<GameObject> badList = new List<GameObject>();


    public void SetStageObject()
    {
        // Set �ϱ� ���� ����Ʈ�� �ѹ� ����
        foreach (var obj in goodList)
        {
            Destroy(obj);
        }
        foreach (var obj in badList)
        {
            Destroy(obj);
        }

        // List �ʱ�ȭ
        goodList.Clear();
        badList.Clear();

        // Good Item ����
        for (int i = 0; i < goodItemCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-23.0f, 23.0f), 0.05f, Random.Range(-23.0f, 23.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));

            goodList.Add(Instantiate(goodItem, transform.position + pos, rot, transform));
        }

        // Bad Item ����
        for (int i = 0; i < badItemCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-23.0f, 23.0f), 0.05f, Random.Range(-23.0f, 23.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));

            badList.Add(Instantiate(badItem, transform.position + pos, rot, transform));
        }
    }
}
