using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSDisplayComponent : MonoBehaviour
{
    private Text m_Text;

    private void Awake()
    {
        m_Text = GetComponent<Text>();
        StartCoroutine(LabelUpdate_IEF());
    }

    private IEnumerator LabelUpdate_IEF()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.1f);
        while (true)
        {
            m_Text.text = "" + (1.0f / Time.deltaTime);
            yield return wfs;
        }
    }
}
