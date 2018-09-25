/*
Author: Inan Evin
www.inanevin.com
*/

using UnityEngine;
using Vectrosity;
using System.Collections;

public class IE_Line : MonoBehaviour
{

    public VectorLine myLine;
    public float f_Time = 3.0f;
    public float f_InterpolateSpeed = 20f;
    public Vector3[] points;
    public Vector3[] dirPoints;
    public float f_MinColliderX;
    public float f_ColliderY;
    public Coroutine co_Count;
    private GameObject go_UsedCollider;
    public bool b_ColliderActive;
    public void PVO_INIT()
    {
        co_Count = StartCoroutine(CO_Count());
    }

    public void PVO_ImmediateDestroy()
    {
        if (co_Count != null)
            StopCoroutine(co_Count);

        co_Destroy = StartCoroutine(CO_Destroy());
    }

    public void PVO_SetLine(VectorLine l)
    {
        myLine = l;
    }

    public Coroutine co_Destroy;
    IEnumerator CO_Count()
    {
        yield return new WaitForSeconds(f_Time);
        co_Destroy = StartCoroutine(CO_Destroy());
    }

    public bool b_DestroyHasStarted;
    IEnumerator CO_Destroy()
    {
        if (b_DestroyHasStarted)
            yield break;

        b_DestroyHasStarted = true;
        Vector3 midPoint = (points[0] + points[1]) / 2;
        float i = 0.0f;
        Vector3 start = points[0];
        Vector3 end = points[1];
        Vector3 start2 = points[3];
        Vector3 end2 = points[2];

        while (i < 1.0f)
        {
            i += Time.deltaTime * f_InterpolateSpeed;
            myLine.points3[0] = Vector3.Lerp(start, midPoint, i);
            myLine.points3[1] = Vector3.Lerp(end, midPoint, i);
            myLine.points3[3] = Vector3.Lerp(start2, end2, i);
            myLine.Draw3D();
            yield return null;
        }
        yield return null;

        //PVO_ClearCollider();
    }

    public void PVO_ClearCollider()
    {
        go_UsedCollider.transform.parent = null;
        go_UsedCollider.SetActive(false);
    }

    public void PVO_AddCollider(IE_ObjectPooler _ColliderPooler)
    {
        // if (go_UsedCollider != null)
        // PVO_ClearCollider();

        Vector3 startPos = points[0];
        Vector3 endPos = points[1];
        go_UsedCollider = _ColliderPooler.GetPooledObject();
        go_UsedCollider.transform.rotation = Quaternion.identity;
        BoxCollider2D col = go_UsedCollider.GetComponent<BoxCollider2D>();

        Vector3 midPoint = (startPos + endPos) / 2;
        col.gameObject.SetActive(true);
        col.isTrigger = false;
        col.transform.parent = transform; // Collider is added as child object of line
        float lineLength = Vector3.Distance(startPos, endPos); // length of line
        if (lineLength < f_MinColliderX)
            lineLength = f_MinColliderX;
        col.size = new Vector3(lineLength, f_ColliderY, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement

        col.transform.position = midPoint; // setting position of collider object
                                           // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        if (!float.IsNaN(angle))
            col.transform.Rotate(0, 0, angle);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

}
