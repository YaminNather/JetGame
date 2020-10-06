using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    #region Variables
    private Text FPSLbl;
    #endregion

    private void Awake()
    {
        FPSLbl = GetComponent<Text>();
        StartCoroutine(FPSRefresh_IEF());
    }

    private IEnumerator FPSRefresh_IEF()
    {
        WaitForSeconds WFS_0 = new WaitForSeconds(0.5f);
        while (true)
        {
            float value = 1f / Time.deltaTime;
            FPSLbl.text = "" + value;
            yield return WFS_0;
        }
    }
}
