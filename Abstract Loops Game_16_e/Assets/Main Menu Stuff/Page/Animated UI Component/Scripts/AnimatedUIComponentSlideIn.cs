using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedUIComponentSlideIn : AnimatedUIComponent
{
    #region Variables
    [SerializeField] private SlideInDir_EN m_SlideInDir;
    [SerializeField] private bool ToUseCurve;
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
    
    public override void Enter_F(float time)
    {
        Tweener T_0 = DOTween.To(() => ScreenOutPosCalc_F(), val => GetComponent<RectTransform>().localPosition = val, m_PosInit, time);
        if (ToUseCurve) T_0.SetEase(m_Ease_AnimCurve);
        Debug.Log("<color=green>Enter_F() called</color>");
    }

    public override void ExitInitialize_F()
    {
        GetComponent<RectTransform>().localPosition = m_PosInit;   
    }
    
    public override void Exit_F(float time)
    {
        Tweener T_0 = DOTween.To(() => m_PosInit, val => GetComponent<RectTransform>().localPosition = val, ScreenOutPosCalc_F(), time);
        if(ToUseCurve) T_0.SetEase(m_Ease_AnimCurve);
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
