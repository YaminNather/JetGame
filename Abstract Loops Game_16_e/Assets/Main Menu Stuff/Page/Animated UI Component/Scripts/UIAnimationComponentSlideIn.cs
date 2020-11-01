using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI Animation/Slide")]
public class UIAnimationComponentSlideIn : UIAnimationComponent
{
    #region Variables
    [Header("Sliding Settings")]
    [SerializeField] private SlideInDir_EN m_SlideInDir;   
    private Vector3 m_AnchoredPosInit;
    #endregion

    public override void OnSceneLoaded_F()
    {
        m_AnchoredPosInit = GetComponent<RectTransform>().anchoredPosition;
    }    

    public override void EntryInitialize_F()
    {
        GetComponent<RectTransform>().anchoredPosition = ScreenOutPosCalc_F();
    }
    
    public override Tweener EntryTween_F()
    {
        Tweener T_0 = DOTween.To(() => ScreenOutPosCalc_F(), val => GetComponent<RectTransform>().anchoredPosition = val, m_AnchoredPosInit, Duration);
        EntryTweenSetEase_F(T_0);
        return T_0;
    }

    public override void ExitInitialize_F()
    {
        GetComponent<RectTransform>().anchoredPosition = m_AnchoredPosInit;   
    }
    
    public override Tweener ExitTween_F()
    {
        Tweener T_0 = DOTween.To(() => m_AnchoredPosInit, val => GetComponent<RectTransform>().anchoredPosition = val, ScreenOutPosCalc_F(), Duration);
        ExitTweenSetEase_F(T_0);
        return T_0;
    }

    private Vector3 ScreenOutPosCalc_F()
    {
        switch(m_SlideInDir)
        {
            case SlideInDir_EN.Right:
                return m_AnchoredPosInit + new Vector3(Screen.width * 2, 0f, 0f);

            case SlideInDir_EN.Left:
                return m_AnchoredPosInit + new Vector3(-Screen.width * 2, 0f, 0f);

            case SlideInDir_EN.Up:
                return m_AnchoredPosInit + new Vector3(0f, Screen.height * 2, 0f);

            case SlideInDir_EN.Down:
                return m_AnchoredPosInit + new Vector3(0f, -Screen.height * 2, 0f);

            default:
                return m_AnchoredPosInit;

        }
    }
}

public enum SlideInDir_EN { Right, Left, Up, Down }
