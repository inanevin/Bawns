/*
Author: Inan Evin
www.inanevin.com
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IE_EndGame : MonoBehaviour
{
    public GameObject go_GameOver;
    public GameObject go_HS;
    public GameObject go_Skip;

    public Text txt_TotalPoints;
    public float f_PanelTime;
    public Text txt_GameOver;
    public float f_RSpeed;
    public float f_GSpeed;
    public float f_BSpeed;
    public GameObject go_HighScore;
    public float f_HSWaitTime;
    public float f_CurrentTotal;
    private bool b_IsCurrentHS;
    public int i_CurrentTime;
    public void PVO_Initiate(int total, float time, bool isHighScore)
    {

        b_IsCurrentHS = isHighScore;
        i_CurrentTime = (int)time;
        f_CurrentTotal = (int)(total * time);
        go_GameOver.SetActive(true);

        StartCoroutine(CO_GameOver());
    }

    public void PVO_AfterTotalPoints()
    {
        go_Skip.SetActive(true);

        if (b_IsCurrentHS)
            go_HS.SetActive(true);

    }

    IEnumerator CO_GameOver()
    {
        float r, g, b;
        while (true)
        {
            r = Mathf.Sin(Time.time * f_RSpeed);


            g = Mathf.Sin(Time.time * f_GSpeed);

            b = Mathf.Sin(Time.time * f_BSpeed);



            txt_GameOver.color = new Color(r, g, b, .7f);
            yield return null;
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
