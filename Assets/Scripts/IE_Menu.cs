/*
Author: Inan Evin
www.inanevin.com
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IE_Menu : MonoBehaviour
{

    public IE_BallMovement _BallMovement;
    public IE_BallHealth _BallHealth;
    public IE_DrawLine _Drawer;
    public IE_RandomObstacleSpawner _Spawner;

    public Text txt_InanEvin;
    public RectTransform rt_InanEvin;
    public Vector2 v2_RandomPos;
    public float f_RandomZ;
    public float f_FadeTime;
    private Coroutine co_InanEvin;
    public AudioSource as_MainMusic;
    public AudioSource as_Buttons;
    public AudioClip ac_GamePlayMusic;
    public AudioClip ac_ButtonHit;
    public Animator anim_Button1;


    private int i_DisableHash = Animator.StringToHash("Disable");
    public float f_WaitToStartTime;
    public Text txt_ScoreText;
    public static bool b_MusicClosed;
    public AudioSource[] as_Sources;
    public float[] f_InitialVolumes;
    private static bool b_SfxClosed;
    public Sprite spr_SfxActive;
    public Sprite spr_SfxDeactive;
    public Sprite spr_MusicClose;
    public Sprite spr_MusicActive;
    public Image img_Sfx;
    public Image img_Music;

    private bool b_GamePaused;
    public Image img_Pause;
    public Sprite spr_Play;
    public Sprite spr_Pause;
    public AudioClip ac_Pause;
    public AudioClip ac_Play;
    public GameObject go_PlayerBallSprite;

    void Awake()
    {
        _BallHealth.enabled = false;
        _BallMovement.enabled = false;
        _Drawer.enabled = false;
        go_PlayerBallSprite.SetActive(false);
    }

    void OnEnable()
    {
        txt_ScoreText.text = "₩ " + PlayerPrefs.GetInt("_HighScore").ToString();

        _Control.f_InitialVolume = as_MainMusic.volume;
        if (b_MusicClosed)
        {
            if (IE_GameControl.instance.co_LerpVolume != null)
                StopCoroutine(IE_GameControl.instance.co_LerpVolume);
            img_Music.sprite = spr_MusicClose;
            as_MainMusic.volume = 0;
        }
        else
        {
            img_Music.sprite = spr_MusicActive;
            as_MainMusic.volume = _Control.f_InitialVolume;
        }

        if (b_SfxClosed)
        {
            as_Sources[0].volume = 0;
            as_Sources[1].volume = 0;
            as_Sources[2].volume = 0;
            as_Sources[3].volume = 0;
            as_Sources[4].volume = 0;
            as_Sources[5].volume = 0;
            as_Sources[6].volume = 0;
            as_Sources[7].volume = 0;
            img_Sfx.sprite = spr_SfxDeactive;
        }
        else
        {
            as_Sources[0].volume = f_InitialVolumes[0];
            as_Sources[1].volume = f_InitialVolumes[1];
            as_Sources[2].volume = f_InitialVolumes[2];
            as_Sources[3].volume = f_InitialVolumes[3];
            as_Sources[4].volume = f_InitialVolumes[4];
            as_Sources[5].volume = f_InitialVolumes[5];
            as_Sources[6].volume = f_InitialVolumes[6];
            as_Sources[7].volume = f_InitialVolumes[7];
            img_Sfx.sprite = spr_SfxActive;
        }
    }

    bool isPaused = false;


    void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;

        if (isPaused)
        {
            b_GamePaused = true;
            img_Pause.sprite = spr_Play;
            as_Buttons.PlayOneShot(ac_Pause);
            Time.timeScale = 0.0f;
        }
        else
        {
            b_GamePaused = false;
            img_Pause.sprite = spr_Pause;
            Time.timeScale = 1.0f;
            as_Buttons.PlayOneShot(ac_Play);

        }
    }


    public void PVO_PauseButton()
    {
        b_GamePaused = !b_GamePaused;

        if (b_GamePaused)
        {
            img_Pause.sprite = spr_Play;
            as_Buttons.PlayOneShot(ac_Pause);
            Time.timeScale = 0.0f;
        }
        else
        {
            img_Pause.sprite = spr_Pause;
            Time.timeScale = 1.0f;
            as_Buttons.PlayOneShot(ac_Play);

        }
    }

    public void PVO_MusicButton()
    {

        b_MusicClosed = !b_MusicClosed;

        if (b_MusicClosed)
        {
            if (IE_GameControl.instance.co_LerpVolume != null)
                StopCoroutine(IE_GameControl.instance.co_LerpVolume);
            img_Music.sprite = spr_MusicClose;
            as_MainMusic.volume = 0;
        }
        else
        {
            img_Music.sprite = spr_MusicActive;
            as_MainMusic.volume = _Control.f_InitialVolume;
        }
    }

    public void PVO_SFXButton()
    {
        b_SfxClosed = !b_SfxClosed;

        if (b_SfxClosed)
        {
            as_Sources[0].volume = 0;
            as_Sources[1].volume = 0;
            as_Sources[2].volume = 0;
            as_Sources[3].volume = 0;
            as_Sources[4].volume = 0;
            as_Sources[5].volume = 0;
            as_Sources[6].volume = 0;
            as_Sources[7].volume = 0;
            img_Sfx.sprite = spr_SfxDeactive;
        }
        else
        {
            as_Sources[0].volume = f_InitialVolumes[0];
            as_Sources[1].volume = f_InitialVolumes[1];
            as_Sources[2].volume = f_InitialVolumes[2];
            as_Sources[3].volume = f_InitialVolumes[3];
            as_Sources[4].volume = f_InitialVolumes[4];
            as_Sources[5].volume = f_InitialVolumes[5];
            as_Sources[6].volume = f_InitialVolumes[6];
            as_Sources[7].volume = f_InitialVolumes[7];
            img_Sfx.sprite = spr_SfxActive;
        }
    }


    public IE_GameControl _Control;
    public void PVO_StartGame()
    {
        Application.targetFrameRate = 30;

        as_Buttons.PlayOneShot(ac_ButtonHit);
        as_MainMusic.clip = ac_GamePlayMusic;
        as_MainMusic.Play();
        anim_Button1.SetTrigger(i_DisableHash);

        _Control.f_StartTimeStamp = Time.timeSinceLevelLoad;
        txt_ScoreText.text = "₩ 0";
        _Spawner.PVO_Init();
        StartCoroutine(CO_WaitToStart());
    }


    public void PVO_Developer()
    {
        Application.OpenURL("http://inanevin.com/");
        /*as_Buttons.PlayOneShot(ac_ButtonHit);

        if (co_InanEvin != null)
            StopCoroutine(co_InanEvin);

        co_InanEvin = StartCoroutine(CO_IE());*/

    }

    public void PVO_Restart()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    IEnumerator CO_WaitToStart()
    {
        yield return new WaitForSeconds(f_WaitToStartTime);
        go_PlayerBallSprite.SetActive(true);
        _BallHealth.enabled = true;
        _BallMovement.enabled = true;
        _Drawer.enabled = true;
    }
    IEnumerator CO_IE()
    {
        float i = 0.0f;
        txt_InanEvin.color = new Color(txt_InanEvin.color.r, txt_InanEvin.color.g, txt_InanEvin.color.b, 1.0f);
        Color currentCOl = txt_InanEvin.color;
        rt_InanEvin.anchoredPosition = new Vector2(Random.Range(-v2_RandomPos.x, v2_RandomPos.x), Random.Range(-v2_RandomPos.y, v2_RandomPos.y));
        rt_InanEvin.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-f_RandomZ, f_RandomZ)));
        while (i < 1.0f)
        {
            i += Time.deltaTime * f_FadeTime;
            txt_InanEvin.color = Color.Lerp(currentCOl, new Color(currentCOl.r, currentCOl.g, currentCOl.b, 0.0f), i);
            yield return null;
        }
    }

    public void PVO_Exit()
    {
        as_Buttons.PlayOneShot(ac_ButtonHit);
        Application.Quit();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
