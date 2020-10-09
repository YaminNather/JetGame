using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlayer : MonoBehaviour
{
    private JetMovementComponent m_jmc;

    private void Awake()
    {
        m_jmc = GetComponent<JetMovementComponent>();
        StartCoroutine(RandomJetSway_IEF());
    }

    private void Update()
    {
        m_jmc.InputVectorAdd_F(new Vector3(0f, 0f, 1f));
    }

    private IEnumerator RandomJetSway_IEF()
    {        
        while(true)
        {
            float time = Random.Range(0f, 0.5f);            
            Vector3 input = new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), 0f);
            //Debug.Log($"<color=yellow>Time = {time}, dir = {input}</color>");
            while (time > 0f)
            {
                m_jmc.InputVectorAdd_F(input);
                time -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
