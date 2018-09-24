/*
Author: Inan Evin
www.inanevin.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IE_MultipleArrow : MonoBehaviour
{

    public float f_ActiveTime;
    public float f_RotateSpeed;

    private Transform t_This;
    private Coroutine co_Count;
    public bool b_DoesRotate;
    void Awake()
    {
        t_This = transform;
    }

    void OnEnable()
    {
        StartCoroutine(CO_Count());

    }
    void Update()
    {
        if (b_DoesRotate)
            t_This.Rotate(new Vector3(0, 0, f_RotateSpeed) * Time.deltaTime);
    }

    public void PVO_Hit()
    {
        if (co_Count != null)
            StopCoroutine(co_Count);

        gameObject.SetActive(false);
    }

    IEnumerator CO_Count()
    {
        yield return new WaitForSeconds(f_ActiveTime);
        gameObject.SetActive(false);
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
}
