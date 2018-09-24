using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class IE_DrawLine : MonoBehaviour
{
    private VectorLine line;
    private VectorLine line2;
    public float f_SetPositionRate;
    private float f_LastSet;
    public float f_LineWidth = 2;
    public float f_MaxLineMagnitude;
    public float f_MagnitudeOffset;
    public float f_DrawRate;
    private float f_Timer;
    private bool b_Drawn;
    private bool b_CanCheckOthers;
    Vector3 startPosition;
    Vector3 endPosition;
    public IE_ObjectPooler _ColliderPooler;
    public float f_MinColliderX;
    public float f_ColliderY;
    [Header("Casual Line")]
    public float f_CasualLineLifeTime;
    public float f_CasualLineDestroySpeed;
    public string s_CasualLineTag;
    public Material mat_Uncompleted;
    public Material mat_Completed;
    public float f_MidArrowLength;
    public Transform t_Ball;
    public List<Vector3> linePoints = new List<Vector3>();
    public List<Vector3> line2Points = new List<Vector3>();
    public Texture2D lineTex;
    public Texture2D frontTex;
    public Texture2D backTex;
    private int i_LineCounter = 0;
    private VectorLine usedLine;
    private List<Vector3> usedLinePoints = new List<Vector3>();
    private int i_InitialDrawCount = 0;
    private bool b_CanDrawNewLine = true;
    private IE_Line lineScript;
    private IE_Line lineScript2;
    private IE_Line usedLineScript;
    private bool b_CanUpdateLine;
    void Start()
    {
        //VectorLine.SetEndCap ("Arrow4", EndCap.Both, 0, 0, 2.0f, 1.0f, lineTex, frontTex); 
        line = new VectorLine("Line", linePoints, f_LineWidth, LineType.Discrete);
        line2 = new VectorLine("Line2", line2Points, f_LineWidth, LineType.Discrete);
        line.collider = true;
        line2.collider = true;
        line.Draw3DAuto();
        line2.Draw3DAuto();
        line.rectTransform.gameObject.tag = "CasualLine";
        line2.rectTransform.gameObject.tag = "CasualLine";
        lineScript = line.rectTransform.gameObject.AddComponent<IE_Line>();
        lineScript.f_Time = f_CasualLineLifeTime;
        lineScript.f_InterpolateSpeed = f_CasualLineDestroySpeed;
        //lineScript.f_ColliderY = f_ColliderY;
        //  lineScript.f_MinColliderX = f_MinColliderX;
        lineScript2 = line2.rectTransform.gameObject.AddComponent<IE_Line>();
        lineScript2.f_Time = f_CasualLineLifeTime;
        lineScript2.f_InterpolateSpeed = f_CasualLineDestroySpeed;
        // lineScript2.f_ColliderY = f_ColliderY;
        //  lineScript2.f_MinColliderX = f_MinColliderX;

        linePoints.Add(new Vector3());
        linePoints.Add(new Vector3());
        linePoints.Add(new Vector3());
        linePoints.Add(new Vector3());
        line2Points.Add(new Vector3());
        line2Points.Add(new Vector3());
        line2Points.Add(new Vector3());
        line2Points.Add(new Vector3());
    }

    void Update()
    {
        if (!b_Drawn)
        {
            if (b_CanDrawNewLine && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                b_CanUpdateLine = true;
                b_CanDrawNewLine = false;
                i_LineCounter++;

                if (i_LineCounter == 3)
                    i_LineCounter = 1;

                if (i_LineCounter == 1)
                {
                    usedLine = line;
                    usedLinePoints = linePoints;
                    usedLineScript = lineScript;

                    if (usedLineScript.co_Count != null)
                        usedLineScript.StopCoroutine(usedLineScript.co_Count);
                    if (usedLineScript.co_Destroy != null)
                        usedLineScript.StopCoroutine(usedLineScript.co_Destroy);

                    linePoints.Clear();
                    linePoints.Add(new Vector3());
                    linePoints.Add(new Vector3());
                    linePoints.Add(new Vector3());
                    linePoints.Add(new Vector3());
                }
                else
                {
                    usedLine = line2;
                    usedLinePoints = line2Points;
                    usedLineScript = lineScript2;

                    if (usedLineScript.co_Count != null)
                        usedLineScript.StopCoroutine(usedLineScript.co_Count);
                    if (usedLineScript.co_Destroy != null)
                        usedLineScript.StopCoroutine(usedLineScript.co_Destroy);
                        
                    line2Points.Clear();
                    line2Points.Add(new Vector3());
                    line2Points.Add(new Vector3());
                    line2Points.Add(new Vector3());
                    line2Points.Add(new Vector3());
                }
                usedLineScript.b_ColliderActive = false;

                //line.endCap = "Arrow4";

                // MAY CHANGE ACCORDING TO THE LINE TYPE
                usedLine.material = mat_Uncompleted;

                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 20;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                startPosition = mousePos;
                usedLinePoints[0] = mousePos;

            }
            else if (b_CanUpdateLine && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && Time.time > f_LastSet + f_SetPositionRate)
            {
                f_LastSet = Time.time;
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 20;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);


                if ((mousePos - startPosition).magnitude < f_MaxLineMagnitude)
                {
                    usedLinePoints[1] = mousePos;
                }
                else
                    usedLinePoints[1] = startPosition + (mousePos - startPosition).normalized * f_MaxLineMagnitude;

                float currentMag = (usedLinePoints[1] - startPosition).magnitude;
                usedLinePoints[2] = (usedLinePoints[0] + usedLinePoints[1]) / 2;
                Vector3 cross = Vector3.Cross(usedLinePoints[0], usedLinePoints[1]).normalized * currentMag * f_MidArrowLength;
                float dot = Vector3.Dot(cross, t_Ball.position - usedLinePoints[2]);

                if (dot > 0)
                    cross *= -1;

                usedLinePoints[3] = usedLinePoints[2] - cross;


                usedLine.Draw3D();

            }
            else if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                b_CanUpdateLine = false;
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 20;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);


                // WILL CHANGE ACCORDING TO THE LINE TYPE
                usedLineScript.myLine = usedLine;
                usedLineScript.points = usedLinePoints.ToArray();
                usedLineScript.b_DestroyHasStarted = false;
                if (usedLineScript.co_Count != null)
                    usedLineScript.StopCoroutine(usedLineScript.co_Count);
                usedLine.material = mat_Completed;
                usedLine.rectTransform.gameObject.tag = s_CasualLineTag;
                //lineScript.PVO_AddCollider(_ColliderPooler);
                usedLineScript.PVO_INIT();

                if (i_InitialDrawCount < 2)
                    i_InitialDrawCount++;
                usedLineScript.b_ColliderActive = true;

                b_CanDrawNewLine = true;
                b_Drawn = true;
                f_Timer = 0.0f;
            }
        }
        else
        {
            f_Timer += Time.deltaTime;
            if (f_Timer > f_DrawRate)
                b_Drawn = false;

        }

    }
}
