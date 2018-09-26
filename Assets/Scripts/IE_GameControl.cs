/*	Author: Inan Evin
	www.inanevin.com	
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IE_GameControl : MonoBehaviour
{

    private static IE_GameControl _This;

    public static IE_GameControl instance
    {
        get
        {
            if (_This == null)
            {
                _This = GameObject.FindObjectOfType<IE_GameControl>();

                if (_This == null)
                {
                    GameObject thisInstance = new GameObject("SCRIPTNAME");
                    thisInstance.hideFlags = HideFlags.HideAndDontSave;
                    _This = thisInstance.AddComponent<IE_GameControl>();
                }
            }

            return _This;
        }
    }

    public IE_BallMovement _BallMovement;
    public IE_DrawLine _DrawLine;
    public Animator anim_Camera;
    private int i_DieHash = Animator.StringToHash("Die");
    public IE_EndGame _EndGame;
    public AudioSource as_MainMusic;
    public AudioClip ac_EndClip;
    public float f_StartTimeStamp;
    public float f_LerpVolumeTo;
    public float f_LerpVolumeSpeed;
    public IE_RandomObstacleSpawner _Spawner;
    public IE_BallHealth _Health;

    [HeaderAttribute("Shield")]
    public Color col_ShieldActive;
    public Color col_ShieldDeactive;
    public Image img_Shield;

    [HeaderAttribute("Extra Life")]
    [System.NonSerializedAttribute]
    public int i_ExtraLife;
    public int i_MaxExtraLife;
    public Image[] img_ExtraLives;
    public Color col_ActiveExtraLife;
    public Color col_DeactiveExtraLife;
    [System.NonSerializedAttribute]
    public int i_TotalPoints = 0;

    [HeaderAttribute("New Point Logic")]
    public Text txt_TotalPoints;
    public RectTransform rt_AdditionalPoints;
    public Text txt_AdditionalPoints;
    public Text txt_ComboCount;
    public int i_BigPointLimit;
    public Vector2 v2_SmallAdditionalAppear;
    public Vector2 v2_BigAdditionalAppear;
    public Vector3 v3_SmallAddLocalScale;
    public Vector3 v3_BigAddLocalScale;
    public float f_AdditionalMinZ;
    public float f_AdditionalMaxZ;
    public float f_AdditionalFadeTime;
    private int i_CurrentCombo;
    public float f_ComboIntervalTime;
    public int i_CasualLinePoints;
    private Coroutine co_PointsTaken;
    private Coroutine co_ComboCounter;
    [HeaderAttribute("Casual Line")]
    public float f_CasualUIShakeAmount;
    public float f_CasualUIShakeTime;
    public float f_CasualUIShakeSpeed;
    public float f_ComboInterval;
    public float f_InitialVolume;

    public Coroutine co_LerpVolume;

    [HeaderAttribute("Notification")]
    public string[] s_RandomDefaultNotifications;
    private Coroutine co_Notification;
    public RectTransform rt_Notification;
    public Text txt_Notification;
    public float f_NotMinZ;
    public float f_NotMaxZ;
    public float f_NotShakeAmount;
    public float f_NotShakeSpeed;
    public float f_NotShakeTime;
    public float f_NotFadeSpeed;
    private float f_NotInitialY;
    public string s_ComboNotification;
    public int i_NotificationLimitCoef;
    private int i_NotificationLimitCounter = 1;
    public AudioClip ac_Notification;
    public AudioClip ac_ComboNotification;
    public Color col_NotComboColor;
    public Color col_NotDefaultColor;
    public AudioSource as_NotSource;

    private static bool b_ResolutionSet;
    void Awake()
    {
        v2_SmallAdditionalAppear = rt_AdditionalPoints.anchoredPosition;
        f_NotInitialY = rt_Notification.anchoredPosition.y;
        Application.targetFrameRate = 25;

        if(!b_ResolutionSet)
        {
            b_ResolutionSet = true;
            Debug.Log("Resolution was set!");
            //Screen.SetResolution(600, 488, true, 60);
        }
    }

    public void PVO_ExtraLifeAdded(int amount)
    {
        if (i_ExtraLife + amount > i_MaxExtraLife)
        {
            amount = i_MaxExtraLife - i_ExtraLife;
        }

        i_ExtraLife += amount;

        if (amount == 1)
            img_ExtraLives[i_ExtraLife - 1].color = col_ActiveExtraLife;
        else if (amount == 2)
        {
            img_ExtraLives[i_ExtraLife - 1].color = col_ActiveExtraLife;
            img_ExtraLives[i_ExtraLife - 2].color = col_ActiveExtraLife;
        }

        // Other Effects Maybe

    }

    public void PVO_HealthFinished()
    {
        if (i_ExtraLife == 0)
        {
            PVO_Die();
            return;
        }

        img_ExtraLives[i_ExtraLife - 1].color = col_DeactiveExtraLife;
        _Health.PVO_FullHealth();
        i_ExtraLife--;

        // Effects
    }

    public void PVO_CasualLineTaken()
    {
        IE_PostProcessManager.instance.PVO_CasualLineTaken(IE_PostProcessManager.TechnicolorLerpType.LerpToCasual);

        i_CurrentCombo++;
        int points = i_CasualLinePoints;
        bool showComboUI = false;

        if (i_CurrentCombo > 1)
            showComboUI = true;

        points *= i_CurrentCombo;


        i_TotalPoints += points;
        txt_TotalPoints.text = "₩ " + i_TotalPoints.ToString();
        if (i_CurrentCombo > 1)
        {
            if (co_Notification != null)
                StopCoroutine(co_Notification);

            co_Notification = StartCoroutine(CO_Notification(true));
        }
        else
        {
            if (i_TotalPoints > i_NotificationLimitCoef * i_NotificationLimitCounter)
            {
                i_NotificationLimitCounter++;
                if (co_Notification != null)
                    StopCoroutine(co_Notification);

                co_Notification = StartCoroutine(CO_Notification(false));
            }
        }


        if (co_PointsTaken != null)
            StopCoroutine(co_PointsTaken);


        co_PointsTaken = StartCoroutine(CO_PointsTaken(points, showComboUI, i_CurrentCombo));



        if (co_ComboCounter != null)
            StopCoroutine(co_ComboCounter);

        co_ComboCounter = StartCoroutine(CO_CountCombo());
    }

    public void PVO_ShieldActivation(bool isActive)
    {
        Color targetColor = col_ShieldActive;

        if (!isActive)
            targetColor = col_ShieldDeactive;

        img_Shield.color = targetColor;
    }

    IEnumerator CO_Notification(bool isCombo)
    {

        if (isCombo)
        {
            as_NotSource.PlayOneShot(ac_ComboNotification);
            txt_Notification.text = s_ComboNotification;
            txt_Notification.color = col_NotComboColor;
        }
        else
        {
            as_NotSource.PlayOneShot(ac_Notification);
            txt_Notification.text = s_RandomDefaultNotifications[Random.Range(0, s_RandomDefaultNotifications.Length)];
            txt_Notification.color = col_NotDefaultColor;
        }

        rt_Notification.anchoredPosition = new Vector2(rt_Notification.anchoredPosition.x, f_NotInitialY);
        txt_Notification.color = new Color(txt_Notification.color.r, txt_Notification.color.g, txt_Notification.color.b, 1.0f);
        rt_Notification.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(f_NotMinZ, f_NotMaxZ)));

        float timer = 0.0f;

        while (timer < f_NotShakeTime)
        {
            timer += Time.deltaTime;

            rt_Notification.anchoredPosition = new Vector2(rt_Notification.anchoredPosition.x, f_NotInitialY + Mathf.Sin(Time.time * f_NotShakeSpeed) * f_NotShakeAmount);
            yield return null;
        }

        Color currentCol = txt_Notification.color;

        timer = 0.0f;

        while (timer < 1.0f)
        {
            timer += Time.deltaTime * f_NotFadeSpeed;
            txt_Notification.color = Color.Lerp(currentCol, new Color(currentCol.r, currentCol.g, currentCol.b, 0.0f), timer);
            yield return null;
        }
    }
    IEnumerator CO_CountCombo()
    {
        yield return new WaitForSeconds(f_ComboIntervalTime);
        i_CurrentCombo = 0;
    }

    IEnumerator CO_PointsTaken(int points, bool showComboUI, int combo)
    {
        Vector2 appearPosition = v2_SmallAdditionalAppear;
        Vector3 appearScale = v3_SmallAddLocalScale;

        if (points > i_BigPointLimit)
        {
            appearPosition = v2_BigAdditionalAppear;
            appearScale = v3_BigAddLocalScale;
        }


        txt_AdditionalPoints.color = new Color(txt_AdditionalPoints.color.r, txt_AdditionalPoints.color.g, txt_AdditionalPoints.color.b, 1.0f);
        txt_AdditionalPoints.text = "+₩ " + points.ToString();

        if (showComboUI)
        {
            txt_ComboCount.text = " X" + combo;
            txt_ComboCount.color = new Color(txt_ComboCount.color.r, txt_ComboCount.color.g, txt_ComboCount.color.b, 1.0f);
            txt_ComboCount.enabled = true;
        }
        else
            txt_ComboCount.enabled = false;

        Color currentComboColor = txt_ComboCount.color;
        Color currentColor = txt_AdditionalPoints.color;

        rt_AdditionalPoints.anchoredPosition = appearPosition;
        rt_AdditionalPoints.localScale = appearScale;

        rt_AdditionalPoints.localEulerAngles = new Vector3(rt_AdditionalPoints.localEulerAngles.x, rt_AdditionalPoints.localEulerAngles.y, Random.Range(f_AdditionalMinZ, f_AdditionalMaxZ));

        float i = 0.0f;

        while (i < 1.0f)
        {
            i += Time.deltaTime * f_AdditionalFadeTime;
            txt_AdditionalPoints.color = Color.Lerp(currentColor, new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), i);
            if (showComboUI)
                txt_ComboCount.color = Color.Lerp(currentComboColor, new Color(txt_ComboCount.color.r, txt_ComboCount.color.g, txt_ComboCount.color.b, 0.0f), i);
            yield return null;
        }

    }

    public void PVO_Die()
    {
        _Spawner.enabled = false;
        as_MainMusic.clip = ac_EndClip;
        as_MainMusic.Play();
        _BallMovement.enabled = false;
        _DrawLine.enabled = false;

        int highScore = PlayerPrefs.GetInt("_HighScore");
        Debug.Log(PlayerPrefs.GetInt("_HighScore"));
        bool isHighScore = false;
        if (highScore < (i_TotalPoints * (Time.timeSinceLevelLoad - f_StartTimeStamp)))
        {
            PlayerPrefs.SetInt("_HighScore", (int)(i_TotalPoints * (Time.timeSinceLevelLoad - f_StartTimeStamp)));
            isHighScore = true;
        }
        _EndGame.PVO_Initiate(i_TotalPoints, (Time.timeSinceLevelLoad - f_StartTimeStamp), isHighScore);

        if (!IE_Menu.b_MusicClosed)
            co_LerpVolume = StartCoroutine(CO_LerpVolume());
    }

    IEnumerator CO_LerpVolume()
    {
        float i = 0.0f;

        float current = as_MainMusic.volume;

        while (i < 1.0f)
        {
            i += Time.deltaTime * f_LerpVolumeSpeed;
            as_MainMusic.volume = Mathf.Lerp(current, f_LerpVolumeTo, i);
            yield return null;
        }
        f_InitialVolume = as_MainMusic.volume;

    }



    void OnDisable()
    {
        StopAllCoroutines();
    }
}
