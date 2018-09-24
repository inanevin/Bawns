/*	Author: Inan Evin
	www.inanevin.com	
*/

using UnityEngine;
using System.Collections;
using Colorful;

public class IE_PostProcessManager : MonoBehaviour
{

    private static IE_PostProcessManager _This;

    public static IE_PostProcessManager instance
    {
        get
        {
            if (_This == null)
            {
                _This = GameObject.FindObjectOfType<IE_PostProcessManager>();

                if (_This == null)
                {
                    GameObject thisInstance = new GameObject("SCRIPTNAME");
                    thisInstance.hideFlags = HideFlags.HideAndDontSave;
                    _This = thisInstance.AddComponent<IE_PostProcessManager>();
                }
            }

            return _This;
        }
    }

    public Technicolor cc_Technicolor;
    public enum TechnicolorLerpType { LerpToCasual };
    private Coroutine co_TechnicolorLerp;

    public Animator anim_RedTarget;
    public Animator anim_PurpleTarget;
    public Animator anim_ShieldTarget;
    private int i_HitHash = Animator.StringToHash("Hit");
    [HeaderAttribute("Casual Line Technicolor")]
    public Animator anim_CasualLineTarget;
    public bool b_CasualLineAnimation;
    public string s_CasualLineTrigger;
    private int i_CasualLineHash;


    [HeaderAttribute("Border Hit")]
    public Animator anim_BorderTarget;

    public bool b_BorderHitAnimation;
    public string s_BorderHitTrigger;
    private int i_BorderHitHash;
    [HeaderAttribute("RandomDir Hit")]
    public Animator anim_RandomDirTarget;
    private int i_RandomDirHash = Animator.StringToHash("RandomDirTaken");

    void Awake()
    {
        if (b_CasualLineAnimation)
            i_CasualLineHash = Animator.StringToHash(s_CasualLineTrigger);

        if (b_BorderHitAnimation)
            i_BorderHitHash = Animator.StringToHash(s_BorderHitTrigger);
    }

    public void PVO_BorderHit()
    {
        if (b_BorderHitAnimation)
            anim_BorderTarget.SetTrigger(i_BorderHitHash);
    }

    public void PVO_RandomHit()
    {
        anim_RandomDirTarget.SetTrigger(i_RandomDirHash);
    }
    public void PVO_BlueHit()
    {
        anim_PurpleTarget.SetTrigger(i_HitHash);
    }
    public void PVO_RedHit()
    {
        anim_RedTarget.SetTrigger(i_HitHash);
    }
    public void PVO_ShieldHit()
    {
        anim_ShieldTarget.SetTrigger(i_HitHash);
    }


    public void PVO_CasualLineTaken(TechnicolorLerpType lerpType)
    {
        if (b_CasualLineAnimation)
            anim_CasualLineTarget.SetTrigger(i_CasualLineHash);
    }


}
