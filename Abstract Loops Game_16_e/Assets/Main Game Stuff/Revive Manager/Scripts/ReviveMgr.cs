using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveMgr : MonoBehaviour
{
    [SerializeField] private float ReviveTime = 2f;
    private Coroutine ReviveTimer_Corout;
    public System.Action<bool> OnReviveEnd_E;

    private void OnEnable()
    {
        ReviveTimer_Corout = MainGameReferences.s_Instance.mainGameMgr.StartCoroutine(ReviveTimer_IEF());
    }

    private IEnumerator ReviveTimer_IEF()
    {
        yield return new WaitForSeconds(ReviveTime);
        OnReviveEnd_E?.Invoke(false);
        gameObject.SetActive(false);
    }

    public void Revive_BEF()
    {
        StopCoroutine(ReviveTimer_Corout);
        OnReviveEnd_E?.Invoke(true);
        gameObject.SetActive(false);
    }
}
