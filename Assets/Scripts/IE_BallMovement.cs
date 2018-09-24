using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(IE_BallHealth))]
public class IE_BallMovement : MonoBehaviour
{
    public Vector2 v2_InitialRandomVelocity;
    public Vector2 v2_MinBorders;
    public Vector2 v2_MaxBorders;
    public Transform t_BallSprite;
    private Vector2 v2_CurrentMoveVelocity;
    private Transform t_This;


    private IE_BallHealth _Health;
    public float f_NoSpeedDamage;
    [HeaderAttribute("Casual Line")]
    public float f_Casual_SpeedBoost;
    public float f_Casual_HealthIncrease;

    [HeaderAttribute("Borders")]
    public float f_Border_SpeedBoost;
    private bool b_BorderCollisionActive = true;
    private bool b_BorderCollisionFlag;

    [HeaderAttribute("Obstacles")]
    public float f_RedObstacleDamage;
    public float f_BlueObstacleHealth;
    public float f_ShieldObstacleTime;
    private bool b_CanReceiveDmg = true;
    private Coroutine co_ShieldTimer;
    public AudioClip ac_BlueHit;
    public AudioClip ac_RedHit;
    public AudioClip ac_ShieldHit;
    public Image img_Shield;

    [HeaderAttribute("Audio")]
    public AudioSource as_CasualLine;
    public AudioClip ac_CasualLineClip;
    public AudioSource as_BorderHit;
    public AudioClip ac_BorderHit;
    public AudioClip ac_RandomDirHit;
    public AudioClip ac_BorderHitDMG;

    [HeaderAttribute("Position Tracking")]
    public Vector2 v2_MinBoundry;
    public Vector2 v2_MaxBoundry;
    public float f_MaxVelocityMagnitude;
    private float f_BorderLimitOffset;

    void Awake()
    {
        t_This = transform;
        _Health = GetComponent<IE_BallHealth>();
    }

    void OnEnable()
    {
        int randX = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
        int randY = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
        v2_CurrentMoveVelocity = new Vector2(randX * v2_InitialRandomVelocity.x, randY * v2_InitialRandomVelocity.y);
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("CasualLine"))
        {
            IE_Line hitLine = c.GetComponent<IE_Line>();

            if (hitLine == null || !hitLine.b_ColliderActive)
                return;

            Vector2 difference = new Vector2(hitLine.points[3].x, hitLine.points[3].y) - new Vector2(hitLine.points[2].x, hitLine.points[2].y);
            v2_CurrentMoveVelocity = difference.normalized * v2_CurrentMoveVelocity.magnitude;

            int directionX = 1;
            if (v2_CurrentMoveVelocity.x < 0)
                directionX = -1;

            int directionY = 1;
            if (v2_CurrentMoveVelocity.y < 0)
                directionY = -1;
            v2_CurrentMoveVelocity += new Vector2(directionX * f_Casual_SpeedBoost, directionY * f_Casual_SpeedBoost);

            IE_GameControl.instance.PVO_CasualLineTaken();
            hitLine.PVO_ImmediateDestroy();
            as_CasualLine.PlayOneShot(ac_CasualLineClip);
            _Health.PVO_IncreaseHealth(f_Casual_HealthIncrease);
        }
        else if (b_BorderCollisionActive && c.CompareTag("RightBorder"))
        {

            if (b_CanReceiveDmg)
                _Health.PVO_BorderHit();
            IE_PostProcessManager.instance.PVO_BorderHit();
            as_BorderHit.PlayOneShot(ac_BorderHitDMG);
            v2_CurrentMoveVelocity = Vector2.Reflect(v2_CurrentMoveVelocity, Vector2.right);
            v2_CurrentMoveVelocity *= f_Border_SpeedBoost;

        }

        else if (b_BorderCollisionActive && c.CompareTag("LeftBorder"))
        {

            if (b_CanReceiveDmg)
                _Health.PVO_BorderHit();
            IE_PostProcessManager.instance.PVO_BorderHit();
            as_BorderHit.PlayOneShot(ac_BorderHitDMG);
            v2_CurrentMoveVelocity = Vector2.Reflect(v2_CurrentMoveVelocity, -Vector2.right);
            v2_CurrentMoveVelocity *= f_Border_SpeedBoost;


        }

        else if (b_BorderCollisionActive && c.CompareTag("UpBorder"))
        {

            if (b_CanReceiveDmg)
                _Health.PVO_BorderHit();
            IE_PostProcessManager.instance.PVO_BorderHit();
            as_BorderHit.PlayOneShot(ac_BorderHitDMG);
            v2_CurrentMoveVelocity = Vector2.Reflect(v2_CurrentMoveVelocity, Vector2.up);
            v2_CurrentMoveVelocity *= f_Border_SpeedBoost;
        }

        else if (b_BorderCollisionActive && c.CompareTag("DownBorder"))
        {

            if (b_CanReceiveDmg)
                _Health.PVO_BorderHit();
            IE_PostProcessManager.instance.PVO_BorderHit();
            as_BorderHit.PlayOneShot(ac_BorderHitDMG);
            v2_CurrentMoveVelocity = Vector2.Reflect(v2_CurrentMoveVelocity, -Vector2.up);
            v2_CurrentMoveVelocity *= f_Border_SpeedBoost;
        }

        else if (c.CompareTag("RandomDirection"))
        {
            as_BorderHit.PlayOneShot(ac_RandomDirHit);
            IE_PostProcessManager.instance.PVO_RandomHit();
            int randX = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
            int randY = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
            v2_CurrentMoveVelocity = new Vector2(randX * v2_InitialRandomVelocity.x, randY * v2_InitialRandomVelocity.y);
            c.GetComponent<IE_MultipleArrow>().PVO_Hit();
        }
        else if (c.CompareTag("BlueObstacle"))
        {
            as_BorderHit.PlayOneShot(ac_BlueHit);
            IE_PostProcessManager.instance.PVO_BlueHit();
            _Health.PVO_IncreaseHealth(f_BlueObstacleHealth);
            c.GetComponent<IE_MultipleArrow>().PVO_Hit();

        }
        else if (c.CompareTag("RedObstacle"))
        {
            as_BorderHit.PlayOneShot(ac_RedHit);
            IE_PostProcessManager.instance.PVO_RedHit();
            _Health.PVO_DmgExternal(f_RedObstacleDamage);
            c.GetComponent<IE_MultipleArrow>().PVO_Hit();

        }
        else if (c.CompareTag("ShieldObstacle"))
        {
            IE_GameControl.instance.PVO_ShieldActivation(true);
            as_BorderHit.PlayOneShot(ac_ShieldHit);
            IE_PostProcessManager.instance.PVO_ShieldHit();
            if (co_ShieldTimer != null)
                StopCoroutine(co_ShieldTimer);

            co_ShieldTimer = StartCoroutine(CO_ShieldTimer());
            c.GetComponent<IE_MultipleArrow>().PVO_Hit();
        }
        else if (c.CompareTag("ExtraLifeObstacle"))
        {
            IE_GameControl.instance.PVO_ExtraLifeAdded(1);
            c.GetComponent<IE_MultipleArrow>().PVO_Hit();
        }
        else if (c.CompareTag("ExtraLife2Obstacle"))
        {
            IE_GameControl.instance.PVO_ExtraLifeAdded(2);
            c.GetComponent<IE_MultipleArrow>().PVO_Hit();
        }

    }

    IEnumerator CO_ShieldTimer()
    {
        img_Shield.enabled = true;
        b_CanReceiveDmg = false;

        float i = 0.0f;
        float rate = 1.0f / f_ShieldObstacleTime;

        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            img_Shield.fillAmount = Mathf.Lerp(1.0f, 0.0f, i);
            yield return null;
        }
        IE_GameControl.instance.PVO_ShieldActivation(false);
        b_CanReceiveDmg = true;
        img_Shield.enabled = false;
    }


    void Update()
    {
        Debug.Log(v2_CurrentMoveVelocity);

        t_This.Translate(Vector2.ClampMagnitude(v2_CurrentMoveVelocity, f_MaxVelocityMagnitude) * Time.deltaTime);

        if (t_This.position.x > v2_MaxBoundry.x + f_BorderLimitOffset)
        {
            f_BorderLimitOffset = 3.0f;
            v2_CurrentMoveVelocity = new Vector3(-t_This.position.x, -t_This.position.y);
            b_BorderCollisionActive = false;
        }
        else if (t_This.position.x < v2_MaxBoundry.x)
        {
            f_BorderLimitOffset = 0; b_BorderCollisionActive = true;
        }

        if (t_This.position.y > v2_MaxBoundry.y + f_BorderLimitOffset)
        {
            f_BorderLimitOffset = 3.0f;
            v2_CurrentMoveVelocity = new Vector3(-t_This.position.x, -t_This.position.y);
            b_BorderCollisionActive = false;
        }
        else if (t_This.position.y < v2_MaxBoundry.y)
        {
            f_BorderLimitOffset = 0; b_BorderCollisionActive = true;
        }

        if (t_This.position.x < v2_MinBoundry.x - f_BorderLimitOffset)
        {
            f_BorderLimitOffset = 3.0f;
            v2_CurrentMoveVelocity = new Vector3(-t_This.position.x, -t_This.position.y);
            b_BorderCollisionActive = false;
        }
        else if (t_This.position.x > v2_MinBoundry.x)
        {
            f_BorderLimitOffset = 0; b_BorderCollisionActive = true;
        }

        if (t_This.position.y < v2_MinBoundry.y - f_BorderLimitOffset)
        {
            f_BorderLimitOffset = 3.0f;
            v2_CurrentMoveVelocity = new Vector3(-t_This.position.x, -t_This.position.y);
            b_BorderCollisionActive = false;
        }
        else if (t_This.position.y > v2_MinBoundry.y)
        {
            f_BorderLimitOffset = 0; b_BorderCollisionActive = true;
        }

        float angle = Mathf.Atan2(v2_CurrentMoveVelocity.y, v2_CurrentMoveVelocity.x) * Mathf.Rad2Deg;
        t_BallSprite.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (v2_CurrentMoveVelocity.magnitude < 0.02f)
            _Health.PVO_ReceiveDamage(f_NoSpeedDamage * Time.deltaTime);


    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

}
