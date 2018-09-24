/*
Author: Inan Evin
www.inanevin.com
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class IE_AnimationEvents : MonoBehaviour
{

    private AudioSource as_This;
    public Animator anim_MainCamera;
    public GameObject go_ToEnable;
    public float f_CountTime;
    public Text txt_TimeCount;
    public Text txt_PointCount;
    private int i_RGBHash = Animator.StringToHash("RGB");
    private int i_CountHash = Animator.StringToHash("Count");
    public IE_EndGame _EndGame;

    void Awake()
    {
        as_This = GetComponent<AudioSource>();
    }


    public void PVO_PlayAudio(AudioClip clipToPlay)
    {
        anim_MainCamera.SetTrigger(i_RGBHash);
        as_This.PlayOneShot(clipToPlay);
    }

    public void PVO_EnableObject()
    {

        go_ToEnable.SetActive(true);
    }

    public void PVO_CountTime()
    {
        anim_MainCamera.SetTrigger(i_CountHash);


        StartCoroutine(CO_CountTime());
    }

    public void PVO_CountPoints()
    {
        anim_MainCamera.SetTrigger(i_CountHash);

        StartCoroutine(CO_CountPoints());
    }

    public void PVO_CountTotalPoints()
    {
        anim_MainCamera.SetTrigger(i_CountHash);

        StartCoroutine(CO_CountTotalPoints());
    }



    IEnumerator CO_CountPoints()
    {

        int target = IE_GameControl.instance.i_TotalPoints;
        if (target == 0)
        {
            yield return new WaitForSeconds(.6f);
            go_ToEnable.SetActive(true);
            yield break;
        }
        float i = 0.0f;
        f_CountTime *= target;
        if (f_CountTime > 1.25f)
            f_CountTime = 1.25f;
        else if (f_CountTime < 0.25f)
            f_CountTime = 0.25f;
        float rate = 1.0f / f_CountTime;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            txt_PointCount.text = "POINTS: " + ((int)Mathf.Lerp(0, target, i)).ToString();
            yield return null;
        }
        yield return new WaitForSeconds(.6f);

        go_ToEnable.SetActive(true);
    }

    IEnumerator CO_CountTotalPoints()
    {

        int target = (int)_EndGame.f_CurrentTotal;
        if (target == 0)
        {
            yield return new WaitForSeconds(.6f);
            _EndGame.PVO_AfterTotalPoints();

            yield break;
        }
        float i = 0.0f;
        f_CountTime *= target;
        if (f_CountTime > 1.25f)
            f_CountTime = 1.25f;
        else if (f_CountTime < 0.25f)
            f_CountTime = 0.25f;
        float rate = 1.0f / f_CountTime;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            txt_PointCount.text = "TOTAL POINTS: " + ((int)Mathf.Lerp(0, target, i)).ToString();
            yield return null;
        }
        yield return new WaitForSeconds(.6f);

        _EndGame.PVO_AfterTotalPoints();
    }


    IEnumerator CO_CountTime()
    {
        float target = _EndGame.i_CurrentTime;
        f_CountTime *= target;
        if (f_CountTime > 1.25f)
            f_CountTime = 1.25f;
        else if (f_CountTime < 0.25f)
            f_CountTime = 0.25f;
        float rate = 1.0f / f_CountTime;

        float i = 0.0f;

        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            txt_TimeCount.text = "TIME: " + ((int)Mathf.Lerp(0, target, i)).ToString();
            yield return null;
        }
        yield return new WaitForSeconds(.6f);
        go_ToEnable.SetActive(true);

    }
}
