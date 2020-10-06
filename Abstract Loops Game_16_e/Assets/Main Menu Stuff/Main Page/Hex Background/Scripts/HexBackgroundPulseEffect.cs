using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HexBackgroundPulseEffect : MonoBehaviour
{
    private Image Hex_Image;

    private void Awake()
    {
        Hex_Image = GetComponent<Image>();
    }

    private void Start()
    {
        Sequence Seq_0 = DOTween.Sequence();
        Seq_0.Append(DOTween.To(() => 0f, val => Hex_Image.color = Hex_Image.color.With(r:val - 0.1f, g:val), 1f, 2f));
        Seq_0.SetLoops(-1);
    }
}
