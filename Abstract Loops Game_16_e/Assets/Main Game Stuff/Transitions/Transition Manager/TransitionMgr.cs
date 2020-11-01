using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionMgr : MonoBehaviour
{
    private Image m_Image;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    public void TransitionDo_F(Color from, Color to, float duration, System.Action onComplete = null)
    {
        transform.parent.gameObject.SetActive(true);
        DOTween.To(() => from, val => m_Image.color = val, to,duration).OnComplete(() =>
        {
            transform.parent.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void TransitionDo_F(Color to, float duration, System.Action onComplete = null) =>
        TransitionDo_F(m_Image.color, to, duration, onComplete);
    
    public void TransitionDo_F(float from, float to, float duration, System.Action onComplete = null) => 
        TransitionDo_F(m_Image.color.With(a: from), m_Image.color.With(a: to), duration, onComplete);

    public void TransitionDo_F(float to, float duration, System.Action onComplete = null) => 
        TransitionDo_F(m_Image.color.a, to, duration, onComplete);

    public void ColorSet_F(Color color) => m_Image.color = color;
}
