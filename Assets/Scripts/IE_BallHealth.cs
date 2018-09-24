/*
Author: Inan Evin
www.inanevin.com
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(IE_BallMovement))]
public class IE_BallHealth : MonoBehaviour
{

    public float f_InitialHealth;

    public Image img_Health;

    private Color col_EmissionColor;
    private float f_Health;

    [HeaderAttribute("Damages")]
    public float f_BorderDamage;




    void Awake()
    {
        f_Health = f_InitialHealth;
    }


    public void PVO_IncreaseHealth(float amount)
    {
        f_Health += amount;
        if (f_Health > f_InitialHealth)
            f_Health = f_InitialHealth;

        img_Health.fillAmount = f_Health / f_InitialHealth;

    }

    /*IEnumerator CO_HealthIncrease(float amount)
    {
        float amountX = amount;
        if (f_InitialHealth - f_Health < amountX)
            amountX = f_InitialHealth - f_Health;

        rt_HealthIncrease.sizeDelta = new Vector2(f_HealthIncreaseMaxWidth * amountX / 100.0f, f_InitialHealthIncreaseY);
        rt_HealthIncrease.anchoredPosition = new Vector2(((img_Health.fillAmount - 0.5f) * f_HealthIncreaseMaxWidth) - rt_HealthIncrease.sizeDelta.x / 2, 0);

        Vector2 sizeDelta = rt_HealthIncrease.sizeDelta;
        float i = 0.0f;
        while (i < 1.0f)
        {
            i += Time.deltaTime * f_HealthIncreaseFadeSpeed;
            rt_HealthIncrease.sizeDelta = Vector2.Lerp(sizeDelta, new Vector2(0, sizeDelta.y), i);
            yield return null;
        }
    }*/

    public void PVO_BorderHit()
    {
        PVO_ReceiveDamage(f_BorderDamage);
    }

    public void PVO_DmgExternal(float dmg)
    {
        PVO_ReceiveDamage(dmg);
    }

    public void PVO_FullHealth()
    {
        f_Health = f_InitialHealth;
        img_Health.fillAmount = f_Health / f_InitialHealth;
        b_Dead = false;
    }
    private bool b_Dead;
    public void PVO_ReceiveDamage(float dmg)
    {
        f_Health -= dmg;
        img_Health.fillAmount = f_Health / f_InitialHealth;

        if (f_Health < 0 && !b_Dead)
        {
            b_Dead = true;
            IE_GameControl.instance.PVO_HealthFinished();
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
