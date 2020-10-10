using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIAnimationComponent : OnSceneLoadedMonoBehaviour
{
    #region Variables
    [SerializeField] protected Type_EN m_Type;
    public Type_EN Type { get => m_Type; }

    [Header("Timings")]
    [SerializeField] protected float m_StartTime;
    public float StartTime { get => m_StartTime; }
    [SerializeField] protected float m_EndTime;
    public float EndTime { get => m_EndTime; }
    public float Duration { get => m_EndTime - m_StartTime; }

    [Header("Entry Ease Settings")]
    [SerializeField] protected Ease m_EntryEaseType; 
    [SerializeField] protected bool m_EntryToUseCurve;
    [SerializeField] protected AnimationCurve m_EntryAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Exit Ease Settings")]
    [SerializeField] protected Ease m_ExitEaseType;
    [SerializeField] protected bool m_ExitToUseCurve;
    [SerializeField] protected AnimationCurve m_ExitAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    #endregion

    public abstract void EntryInitialize_F();
    public abstract Tweener EntryTween_F();

    public abstract void ExitInitialize_F();
    public abstract Tweener ExitTween_F();

    public enum Type_EN { Both, Entry, Exit }

    protected void EntryTweenSetEase_F(Tweener T_0) => TweenSetEase_F(T_0, m_EntryEaseType, m_EntryToUseCurve, m_EntryAnimationCurve);
    
    protected void ExitTweenSetEase_F(Tweener T_0) => TweenSetEase_F(T_0, m_ExitEaseType, m_ExitToUseCurve, m_ExitAnimationCurve);

    private void TweenSetEase_F(Tweener T_0, Ease easeType, bool toUseCurve, AnimationCurve animationCurve)
    {
        if (T_0.IsActive() == false) throw new System.Exception("Cannot set Ease cuz Tween null!!!");
        switch(toUseCurve)
        {
            case false:
                T_0.SetEase(easeType);
                break;

            case true:
                T_0.SetEase(animationCurve);
                break;
        }
    }
}
