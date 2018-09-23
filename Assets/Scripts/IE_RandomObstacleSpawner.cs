/*
Author: Inan Evin
www.inanevin.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IE_RandomObstacleSpawner : MonoBehaviour
{

    public float f_MinWaitForSeq;
    public float f_MaxWaitForSeq;

    public int i_MinSequenceCount;
    public int i_MaxSequenceCount;
    public float f_MinSeqInterval;
    public float f_MaxSeqInterval;

    public IE_ObjectPooler[] _Poolers;
    [RangeAttribute(0.0f, 1.0f)]
    public float[] f_Chances;
    public Vector2 v2_MinPositions;
    public Vector2 v2_MaxPositions;

    public bool b_InitAtStart;

    void Start()
    {
        if (b_InitAtStart)
            PVO_Init();
    }
    public void PVO_Init()
    {
        StartCoroutine(CO_Seq());
    }

    IEnumerator CO_Seq()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(f_MinWaitForSeq, f_MaxWaitForSeq));

            int random = Random.Range(i_MinSequenceCount, i_MaxSequenceCount);

            int i = 0;

            while (i < random)
            {
                int randomP = 0;
                float randomValue = Random.Range(0.0f, 1.0f);

                if (randomValue < f_Chances[5])
                    randomP = 5;
                else if (randomValue < f_Chances[4])
                    randomP = 4;
                else if (randomValue < f_Chances[3])
                    randomP = 3;
                else if (randomValue < f_Chances[2])
                    randomP = 2;
                else if (randomValue < f_Chances[1])
                    randomP = 1;
                else if (randomValue < f_Chances[0])
                    randomP = 0;

                GameObject pooled = _Poolers[randomP].GetPooledObject();
                if (pooled != null)
                {
                    pooled.transform.position = new Vector3(Random.Range(v2_MinPositions.x, v2_MaxPositions.x), Random.Range(v2_MinPositions.y, v2_MaxPositions.y), 20.0f);
                    pooled.SetActive(true);
                }
                i++;
                yield return new WaitForSeconds(Random.Range(f_MinSeqInterval, f_MaxSeqInterval));
            }

            yield return null;
        }
    }


    void OnDisable()
    {
        StopAllCoroutines();
    }
}
