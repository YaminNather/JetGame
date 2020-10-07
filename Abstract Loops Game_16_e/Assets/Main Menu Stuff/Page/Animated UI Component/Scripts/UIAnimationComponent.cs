using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public abstract class UIAnimationComponent : OnSceneLoadedMonoBehaviour
{
    #region Variables
    [Space(20)]
    [SerializeField] private UIAnimationType_EN m_AnimationType;
    public UIAnimationType_EN AnimationType { get => m_AnimationType; }

    [Header("Timings")]
    [SerializeField] private float m_StartTime;
    public float StartTime { get => m_StartTime; }
    [SerializeField] private float m_EndTime;
    public float EndTime { get => m_EndTime; }
    public float Duration { get => EndTime - StartTime; }

    [Header("Easing")]
    [SerializeField] protected bool m_EntryToUseCurve;
    [SerializeField] protected Ease m_EntryEaseType;
    [SerializeField] protected AnimationCurve m_EntryEaseCurve_AnimCurve;
    
    [SerializeField] protected bool m_ExitToUseCurve;
    [SerializeField] protected Ease m_ExitEaseType;
    [SerializeField] protected AnimationCurve m_ExitEaseCurve_AnimCurve;
    #endregion

    public abstract void EntryInitialize_F();
    public abstract Tweener EntryTweenMake_F();

    public abstract void ExitInitialize_F();
    public abstract Tweener ExitTweenMake_F();

    protected void EntryTweenSetEase_F(Tweener T_0) => TweenSetEase_F(T_0, m_EntryToUseCurve, m_EntryEaseType, m_EntryEaseCurve_AnimCurve);
    protected void ExitTweenSetEase_F(Tweener T_0) => TweenSetEase_F(T_0, m_ExitToUseCurve, m_ExitEaseType, m_ExitEaseCurve_AnimCurve);

    private void TweenSetEase_F(Tweener T_0, bool toUseCurve, Ease easeType = Ease.Linear, AnimationCurve animationCurve = null)
    {
        if (T_0.IsActive() == false) throw new System.Exception("Tween is not Active!!!");

        switch (toUseCurve)
        {
            case false:
                T_0.SetEase(easeType);
                return;

            case true:
                T_0.SetEase(animationCurve);
                return;
        }    
    }

}

public enum UIAnimationType_EN { Both, Entry, Exit }