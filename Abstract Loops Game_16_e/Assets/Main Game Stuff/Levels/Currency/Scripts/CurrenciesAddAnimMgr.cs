using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class CurrenciesAddAnimMgr : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject m_SpritePrefab;
    private int m_PoolCountInit;
    [SerializeField] private CurrencySpritesAnimQueue m_Pool;
    [SerializeField] private Transform CurrencyLblTrans;
    #endregion

    private void Awake()
    {
        m_PoolCountInit = 10;
        PoolInit_F();
    }

    private void PoolInit_F()
    {
#if UNITY_EDITOR
        if(m_SpritePrefab == null)
        {
            Debug.LogError("CurrencyAddAnim m_SpritePrefab is null!!");
            return;
        }
#endif
        m_Pool = new CurrencySpritesAnimQueue(this);
        for (int i = 0; i < m_PoolCountInit; i++)
        {
            GameObject GObj_0 = Instantiate(m_SpritePrefab);            
            m_Pool.Enqueue(GObj_0.transform);
        }
    }

    public void AnimDo_F(Vector3 pos)
    {
        m_Pool.Dequeue(pos);
    }



    [Serializable]
    private class CurrencySpritesAnimQueue
    {
        #region Variables
        private CurrenciesAddAnimMgr m_CurrenciesAddAnimMgr;

        private readonly Vector3 POOLPOS = new Vector3(-999f, -999f, -999f);
        [SerializeField] private List<Transform> m_List;
        #endregion

        public CurrencySpritesAnimQueue(CurrenciesAddAnimMgr m_CurrenciesAddAnimMgr)
        {
            this.m_CurrenciesAddAnimMgr = m_CurrenciesAddAnimMgr;
            m_List = new List<Transform>();
        }

        public void Enqueue(Transform trans)
        {            
            trans.position = POOLPOS;
            m_List.Add(trans);
        }

        public Transform Dequeue(Vector3 pos)
        {
            if (m_List.Count == 0)
            {
                throw new Exception("Queue Underflowed!!");
            }
            Transform r = m_List[0];
            r.position = pos;
            r.DOMove(m_CurrenciesAddAnimMgr.CurrencyLblTrans.position, 1f).SetEase(Ease.InOutBack).OnComplete(() => Enqueue(r));
            m_List.RemoveAt(0);
            return r;
        }
    }
}


