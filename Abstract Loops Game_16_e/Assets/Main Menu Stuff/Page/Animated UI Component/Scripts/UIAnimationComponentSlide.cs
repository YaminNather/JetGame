using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationComponentSlide : UIAnimationComponent
{
    #region Variables
    [Header("Slide Settings")]
    [SerializeField] private SlideInDir_EN m_SlideInDir;
    [SerializeField] private Ease m_EaseType;
    [SerializeField] private bool m_ToUseCurve;
    [SerializeField] private AnimationCurve m_Ease_AnimCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    private Vector3 m_PosInit;
    #endregion

    public override void OnSceneLoaded_F()
    {
        m_PosInit = GetComponent<RectTransform>().localPosition;
    }    

    public override void EntryInitialize_F()
    {
        GetComponent<RectTransform>().localPosition = ScreenOutPosCalc_F();
    }
    
    public override Tweener EntryTweenMake_F()
    {
        Tweener r = DOTween.To(() => ScreenOutPosCalc_F(), val => GetComponent<RectTransform>().localPosition = val, m_PosInit, Duration);
        EntryTweenSetEase_F(r);
        return r;
    }

    public override void ExitInitialize_F()
    {
        GetComponent<RectTransform>().localPosition = m_PosInit;   
    }
    
    public override Tweener ExitTweenMake_F()
    {
        Tweener r = DOTween.To(() => m_PosInit, val => GetComponent<RectTransform>().localPosition = val, ScreenOutPosCalc_F(), Duration);
        ExitTweenSetEase_F(r);
        return r;
    }

    private Vector3 ScreenOutPosCalc_F()
    {
        switch(m_SlideInDir)
        {
            case SlideInDir_EN.Right:
                return m_PosInit + new Vector3(Screen.width / 2, 0f, 0f);

            case SlideInDir_EN.Left:
                return m_PosInit + new Vector3(-Screen.width / 2, 0f, 0f);

            case SlideInDir_EN.Up:
                return m_PosInit + new Vector3(0f, Screen.height / 2, 0f);

            case SlideInDir_EN.Down:
                return m_PosInit + new Vector3(0f, -Screen.height / 2, 0f);

            default:
                return m_PosInit;

        }
    }
}

public enum SlideInDir_EN { Right, Left, Up, Down }
